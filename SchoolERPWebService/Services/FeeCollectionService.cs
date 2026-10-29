// ============================================================================
// STEP 4: FeeCollectionService.cs — full file, with refund support added.
// Additions vs. your original are marked with "// >>> NEW".
// ============================================================================
using System.ComponentModel.Design;
using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Services
{
    public class FeeCollectionService : IFeeCollectionService
    {
        private readonly IConfiguration _configuration;
        private readonly IEnumerable<IPaymentGateway> _gateways;
        private readonly IIPaymentGatewayFactory _gatewayFactory;
        private readonly ILogger<FeeCollectionService> _log;

        public FeeCollectionService(IConfiguration configuration, IIPaymentGatewayFactory gatewayFactory, IEnumerable<IPaymentGateway> gateways,
                                    ILogger<FeeCollectionService> log)
        {
            _configuration = configuration;
            _gateways = gateways;
            _gatewayFactory = gatewayFactory;
            _log = log;
        }

        private SqlConnection Conn() => new(_configuration.GetConnectionString("DefaultConnection"));
        private static string FeesJson(IEnumerable<int> ids) =>
            JsonSerializer.Serialize(ids.Distinct().Select(i => new { DetailId = i }));

        private IPaymentGateway? FindGateway(string? name)
        {
            var wanted = string.IsNullOrWhiteSpace(name) ? (_configuration["Payments:DefaultGateway"] ?? "Stripe") : name;
            return _gateways.FirstOrDefault(g => g.Name.Equals(wanted, StringComparison.OrdinalIgnoreCase));
        }

        //==========================================================
        // FEE LIST  (SP_STUDENT_FEE_TEMPLATE_LIST -> rows + total)
        //==========================================================
        public async Task<StudentFeeTemplateListResponse> GetStudentFeeListAsync(StudentFeeListRequest request)
        {
            try
            {
                using var conn = Conn();
                var p = new DynamicParameters();
                p.Add("@STUDENTID", request.StudentId);
                p.Add("@COMPANYID", request.CompanyId);
                p.Add("@SESSIONID", request.SessionId);

                using var multi = await conn.QueryMultipleAsync("SP_STUDENT_FEE_TEMPLATE_LIST", p,
                    commandType: CommandType.StoredProcedure);

                var rows = (await multi.ReadAsync<StudentFeeTemplateResponse>()).ToList();
                var total = await multi.ReadFirstOrDefaultAsync<StudentFeeTemplateTotalResponse>();

                return new StudentFeeTemplateListResponse
                {
                    Result = 1,
                    Message = "STUDENT FEE LIST FETCHED SUCCESSFULLY",
                    Data = rows,
                    Total = total
                };
            }
            catch (Exception ex)
            {
                return new StudentFeeTemplateListResponse
                {
                    Result = 0,
                    Message = "UNABLE TO FETCH STUDENT FEE LIST.",
                    TechnicalMessage = ex.Message
                };
            }
        }

        //==========================================================
        // OFFLINE COLLECT  (popup "Pay": Cash / Cheque / DD / Bank Transfer / UPI / Card)
        //==========================================================

        public async Task<FeeCollectResponse> CollectOfflineAsync(FeeCollectRequest r)
        {
            try
            {
                using var conn = Conn();
                var p = new DynamicParameters();
                p.Add("@STUDENTID", r.StudentId);
                p.Add("@COMPANYID", r.CompanyId);
                p.Add("@SESSIONID", r.SessionId);
                p.Add("@PAYMENTDATE", r.PaymentDate);
                p.Add("@PAYMENTMODE", r.PaymentMode);
                p.Add("@NOTE", r.Note);
                p.Add("@CHEQUENO", r.ChequeNo);
                p.Add("@CHEQUEDATE", r.ChequeDate);
                p.Add("@BANKNAME", r.BankName);
                p.Add("@REFERENCENO", r.ReferenceNo);

                // FIX: pass the client's FeesJson string straight through instead of
                // rebuilding it from a field (MapStudentsFeeIds) that is never populated.
                p.Add("@FEESJSON", r.FeesJson);

                p.Add("@USERID", r.UserId);

                var res = await conn.QueryFirstOrDefaultAsync<FeeCollectionSpResult>("SP_STUDENT_FEE_COLLECT", p,
                    commandType: CommandType.StoredProcedure);

                return ToResponse(res);
            }
            catch (Exception ex)
            {
                return new FeeCollectResponse { Result = 0, Message = "Unable to collect fees.", TechnicalMessage = ex.Message };
            }
        }

        //==========================================================
        // ONLINE: START  (validate -> save attempt -> create gateway checkout -> return redirect URL)
        //==========================================================
        public async Task<OnlineStartResponse> StartOnlineoldAsync(OnlineStartRequest r)
        {
            int onlinePaymentId = 0;
            try
            {
                var gateway = FindGateway(r.GatewayName);
                if (gateway == null)
                    return new OnlineStartResponse { Result = 0, Message = "Payment gateway is not configured." };

                if (!IsAllowedReturnUrl(r.ReturnBaseUrl))
                    return new OnlineStartResponse { Result = 0, Message = "Invalid return URL." };

                using var conn = Conn();

                // 1. server works out the amount and stores the attempt
                var p = new DynamicParameters();
                p.Add("@STUDENTID", r.StudentId);
                p.Add("@COMPANYID", r.CompanyId);
                p.Add("@SESSIONID", r.SessionId);
                p.Add("@GATEWAYNAME", gateway.Name);
                p.Add("@FEESJSON", FeesJson(r.MapStudentsFeeIds));
                p.Add("@PAYERNAME", r.PayerName);
                p.Add("@PAYEREMAIL", r.PayerEmail);
                p.Add("@PAYERMOBILE", r.PayerMobile);
                p.Add("@IPADDRESS", r.IpAddress);
                p.Add("@USERID", r.UserId);

                var init = await conn.QueryFirstOrDefaultAsync<FeeCollectionSpResult>("SP_STUDENT_FEE_ONLINE_INITIATE", p,
                    commandType: CommandType.StoredProcedure);

                if (init == null || init.Result <= 0)
                    return new OnlineStartResponse { Result = 0, Message = init?.Message ?? "Unable to start online payment." };

                onlinePaymentId = init.Result;
                var baseUrl = r.ReturnBaseUrl.TrimEnd('/');

                //var gateways = await gateway.GetGatewayForCompanyAsync(companyId);

                // 2. create the gateway checkout
                var session = await gateway.CreateCheckoutAsync(new GatewayCheckoutRequest(
                    onlinePaymentId, init.Amount, init.Currency ?? "INR", "School fees", r.PayerEmail,
                    $"{baseUrl}/StudentFees/PaymentReturn?session_id={{CHECKOUT_SESSION_ID}}&studentId={r.StudentId}",
                    $"{baseUrl}/StudentFees/PaymentCancelled?onlinePaymentId={onlinePaymentId}&studentId={r.StudentId}"));

                // 3. remember the gateway order id
                await UpdateOnlineAsync(conn, onlinePaymentId, newOrderId: session.OrderId, status: "Pending",
                                        requestJson: session.RequestJson, userId: r.UserId);

                return new OnlineStartResponse
                {
                    Result = 1,
                    Message = "REDIRECT TO PAYMENT PAGE",
                    RedirectUrl = session.RedirectUrl,
                    OnlinePaymentId = onlinePaymentId
                };
            }
            catch (Exception ex)
            {
                if (onlinePaymentId > 0)
                {
                    try
                    {
                        using var conn = Conn();
                        await UpdateOnlineAsync(conn, onlinePaymentId, status: "Failed", failureReason: ex.Message, userId: r.UserId);
                    }
                    catch { /* ignore secondary failure */ }
                }
                return new OnlineStartResponse { Result = 0, Message = "Unable to start online payment.", TechnicalMessage = ex.Message };
            }
        }

        public async Task<OnlineStartResponse> StartOnlineAsync(OnlineStartRequest r)
        {
            int onlinePaymentId = 0;
            try
            {
                // >>> CHANGED — was: FindGateway(r.GatewayName), then the broken
                // "var gateways = await gateway.GetGatewayForCompanyAsync(companyId);" line
                IPaymentGateway gateway;
                try
                {
                    gateway = await _gatewayFactory.GetGatewayForCompanyAsync(r.CompanyId);
                }
                catch (InvalidOperationException ex)
                {
                    return new OnlineStartResponse { Result = 0, Message = ex.Message };
                }

                if (!IsAllowedReturnUrl(r.ReturnBaseUrl))
                    return new OnlineStartResponse { Result = 0, Message = "Invalid return URL." };

                using var conn = Conn();

                var p = new DynamicParameters();
                p.Add("@STUDENTID", r.StudentId);
                p.Add("@COMPANYID", r.CompanyId);
                p.Add("@SESSIONID", r.SessionId);
                p.Add("@GATEWAYNAME", gateway.Name);
                p.Add("@FEESJSON", FeesJson(r.DetailIds));
                p.Add("@PAYERNAME", r.PayerName);
                p.Add("@PAYEREMAIL", r.PayerEmail);
                p.Add("@PAYERMOBILE", r.PayerMobile);
                p.Add("@IPADDRESS", r.IpAddress);
                p.Add("@USERID", r.UserId);

                var init = await conn.QueryFirstOrDefaultAsync<FeeCollectionSpResult>("SP_STUDENT_FEE_ONLINE_INITIATE", p,
                    commandType: CommandType.StoredProcedure);

                if (init == null || init.Result <= 0)
                    return new OnlineStartResponse { Result = 0, Message = init?.Message ?? "Unable to start online payment." };

                onlinePaymentId = init.Result;
                var baseUrl = r.ReturnBaseUrl.TrimEnd('/');

                // (removed the broken "var gateways = ..." line entirely)

                var session = await gateway.CreateCheckoutAsync(new GatewayCheckoutRequest(
                    onlinePaymentId, init.Amount, init.Currency ?? "INR", "School fees", r.PayerEmail,
                    $"{baseUrl}/StudentFees/StudentFeesDetails?session_id={{CHECKOUT_SESSION_ID}}&studentId={r.StudentId}",
                    $"{baseUrl}/StudentFees/StudentFeesDetails?onlinePaymentId={onlinePaymentId}&studentId={r.StudentId}"));

                await UpdateOnlineAsync(conn, onlinePaymentId, newOrderId: session.OrderId, status: "Pending",
                                        requestJson: session.RequestJson, userId: r.UserId);

                return new OnlineStartResponse
                {
                    Result = 1,
                    Message = "REDIRECT TO PAYMENT PAGE",
                    RedirectUrl = session.RedirectUrl,
                    OnlinePaymentId = onlinePaymentId
                };
            }
            catch (Exception ex)
            {
                if (onlinePaymentId > 0)
                {
                    try
                    {
                        using var conn = Conn();
                        await UpdateOnlineAsync(conn, onlinePaymentId, status: "Failed", failureReason: ex.Message, userId: r.UserId);
                    }
                    catch { }
                }
                return new OnlineStartResponse { Result = 0, Message = "Unable to start online payment.", TechnicalMessage = ex.Message };
            }
        }
        //==========================================================
        // ONLINE: FINALIZE (called from the return page; webhook does the same, SP is idempotent)
        //==========================================================
        public async Task<FeeCollectResponse> FinalizeOnlineAsync(OnlineFinalizeRequest r)
        {
            try
            {
                var gateway = FindGateway(r.GatewayName);
                if (gateway == null) return new FeeCollectResponse { Result = 0, Message = "Payment gateway is not configured." };

                var info = await gateway.GetPaymentAsync(r.OrderId);
                if (info == null) return new FeeCollectResponse { Result = 0, Message = "Payment not found." };
                if (!info.IsPaid) return new FeeCollectResponse { Result = 0, Message = "Payment is not completed yet." };

                return await CompleteAsync(info);
            }
            catch (Exception ex)
            {
                return new FeeCollectResponse { Result = 0, Message = "Unable to confirm payment.", TechnicalMessage = ex.Message };
            }
        }

        public async Task<FeeCollectResponse> CancelOnlineAsync(OnlineCancelRequest r)
        {
            try
            {
                using var conn = Conn();
                var res = await UpdateOnlineAsync(conn, r.OnlinePaymentId, status: "Cancelled",
                                                  failureReason: "Payer cancelled at checkout", userId: r.UserId);
                return new FeeCollectResponse { Result = res?.Result ?? 0, Message = res?.Message };
            }
            catch (Exception ex)
            {
                return new FeeCollectResponse { Result = 0, Message = "Unable to cancel.", TechnicalMessage = ex.Message };
            }
        }

        //==========================================================
        // REFUND  >>> NEW
        // Handles both:
        //   - Online payments: calls the gateway, then records the result.
        //   - Offline payments (cash/cheque/DD/bank transfer/UPI/card taken manually):
        //     there's no gateway call, it's a straight bookkeeping reversal.
        //==========================================================
        public async Task<RefundResponse> RefundAsync(RefundRequest r)
        {
            try
            {
                using var conn = Conn();

                var lookup = await conn.QueryFirstOrDefaultAsync<OnlinePaymentLookup>(
                    "SP_STUDENT_FEE_PAYMENT_GET", new { COLLECTIONID = r.CollectionId },
                    commandType: CommandType.StoredProcedure);

                if (lookup == null)
                    return new RefundResponse { Result = 0, Message = "Original payment not found." };

                if (!string.Equals(lookup.Status, "Success", StringComparison.OrdinalIgnoreCase))
                    return new RefundResponse { Result = 0, Message = "Only paid entries can be refunded." };

                if (r.Amount.HasValue && r.Amount.Value <= 0)
                    return new RefundResponse { Result = 0, Message = "Refund amount must be greater than zero." };

                if (r.Amount.HasValue && r.Amount.Value > lookup.Amount)
                    return new RefundResponse { Result = 0, Message = "Refund amount cannot exceed the amount paid." };

                string? refundId = null;
                decimal refundedAmount = r.Amount ?? lookup.Amount;
                string? rawJson = null;

                if (!string.IsNullOrWhiteSpace(lookup.GatewayPaymentId))
                {
                    // Online payment -> money has to move back through the gateway.
                    IPaymentGateway gateway;
                    try
                    {
                        gateway = await _gatewayFactory.GetGatewayForCompanyAsync(r.CompanyId);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return new RefundResponse { Result = 0, Message = ex.Message };
                    }

                    

                    var refund = await gateway.RefundAsync(new GatewayRefundRequest(lookup.GatewayPaymentId, r.Amount, r.Reason));
                    refundId = refund.RefundId;
                    refundedAmount = refund.Amount;
                    rawJson = refund.RawJson;
                }
                // else: offline payment -> nothing to call, just record the reversal below.

                var p = new DynamicParameters();
                p.Add("@COLLECTIONID", r.CollectionId);
                p.Add("@REFUNDID", refundId);
                p.Add("@REFUNDAMOUNT", refundedAmount);
                p.Add("@REASON", r.Reason);
                p.Add("@RESPONSEPAYLOAD", rawJson);
                p.Add("@USERID", r.UserId);

                var res = await conn.QueryFirstOrDefaultAsync<FeeCollectionSpResult>("SP_STUDENT_FEE_REFUND", p,
                    commandType: CommandType.StoredProcedure);

                if (refundId != null && (res == null || res.Result <= 0))
                {
                    // Money already left via the gateway but the DB write failed.
                    // This needs a human, not a silent failure — same pattern as the
                    // "PAID but receipt failed" log in HandleWebhookAsync below.
                    _log.LogError(
                        "Refund {RefundId} succeeded at the gateway for receipt {Id} but recording it failed: {Msg}. MANUAL REVIEW.",
                        refundId, r.CollectionId, res?.Message);
                }

                return new RefundResponse
                {
                    Result = res?.Result ?? 0,
                    Message = res?.Message ?? (refundId != null
                        ? "Refund processed at the gateway but could not be recorded. Please check manually."
                        : "Unable to record refund."),
                    RefundId = refundId,
                    RefundedAmount = refundedAmount
                };
            }
            catch (Exception ex)
            {
                return new RefundResponse { Result = 0, Message = "Unable to process refund.", TechnicalMessage = ex.Message };
            }
        }

        //==========================================================
        // WEBHOOK  (no login; authenticity = gateway signature)
        //==========================================================
        public async Task<int> HandleWebhookAsync(string gatewayName, string body, string signatureHeader)
        {
            var gateway = FindGateway(gatewayName);
            if (gateway == null) return 400;

            GatewayWebhookEvent evt;
            try { evt = gateway.ParseWebhook(body, signatureHeader); }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "{Gateway} webhook signature/parse failed", gateway.Name);
                return 400;
            }

            try
            {
                // >>> NEW: refund events carry RefundInfo instead of Info, so check that
                // branch before the "no Info -> nothing to do" early return.
                if (evt.Kind == "Refunded" && evt.RefundInfo != null)
                {
                    using var conn = Conn();
                    await conn.ExecuteAsync("SP_STUDENT_FEE_REFUND_BY_GATEWAYID", new
                    {
                        GATEWAYPAYMENTID = evt.RefundInfo.GatewayPaymentId,
                        REFUNDID = evt.RefundInfo.RefundId,
                        REFUNDAMOUNT = evt.RefundInfo.AmountRefunded,
                        FULLYREFUNDED = evt.RefundInfo.FullyRefunded,
                        RESPONSEPAYLOAD = evt.RefundInfo.RawJson
                    }, commandType: CommandType.StoredProcedure);
                    return 200;
                }
                // <<< END NEW

                if (evt.Info == null) return 200;

                switch (evt.Kind)
                {
                    case "Paid":
                        var res = await CompleteAsync(evt.Info);
                        if (res.Result <= 0)
                            _log.LogError("{Gateway} payment {Id} was PAID but receipt failed: {Msg}. MANUAL REVIEW / REFUND.",
                                          gateway.Name, evt.Info.OnlinePaymentId, res.Message);
                        break;
                    case "Failed":
                    case "Expired":
                        using (var conn = Conn())
                            await UpdateOnlineAsync(conn, evt.Info.OnlinePaymentId,
                                status: evt.Kind == "Failed" ? "Failed" : "Cancelled",
                                failureReason: evt.Reason, responseJson: evt.Info.RawJson);
                        break;
                }
                return 200;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Webhook processing error");
                return 500;          // gateway retries; SP is idempotent so retries are safe
            }
        }

        //==========================================================
        // helpers
        //==========================================================
        private async Task<FeeCollectResponse> CompleteAsync(GatewayPaymentInfo info)
        {
            using var conn = Conn();
            var p = new DynamicParameters();
            p.Add("@ONLINEPAYMENTID", info.OnlinePaymentId);
            p.Add("@GATEWAYPAYMENTID", info.PaymentId);
            p.Add("@GATEWAYAMOUNT", info.Amount);
            p.Add("@PAYMENTMETHOD", info.Method);
            p.Add("@RESPONSEPAYLOAD", info.RawJson);

            var res = await conn.QueryFirstOrDefaultAsync<FeeCollectionSpResult>("SP_STUDENT_FEE_COLLECT", p,
                commandType: CommandType.StoredProcedure);
            return ToResponse(res);
        }

        private static async Task<FeeCollectionSpResult?> UpdateOnlineAsync(SqlConnection conn, int id,
            string? newOrderId = null, string? status = null, string? failureReason = null,
            string? requestJson = null, string? responseJson = null, int? userId = null)
        {
            var p = new DynamicParameters();
            p.Add("@ONLINEPAYMENTID", id);
            p.Add("@NEWGATEWAYORDERID", newOrderId);
            p.Add("@STATUS", status);
            p.Add("@FAILUREREASON", failureReason);
            p.Add("@REQUESTPAYLOAD", requestJson);
            p.Add("@RESPONSEPAYLOAD", responseJson);
            p.Add("@USERID", userId);
            return await conn.QueryFirstOrDefaultAsync<FeeCollectionSpResult>("SP_STUDENT_FEE_ONLINE_UPDATE", p,
                commandType: CommandType.StoredProcedure);
        }

        private static FeeCollectResponse ToResponse(FeeCollectionSpResult? res) =>
            res == null
                ? new FeeCollectResponse { Result = 0, Message = "No response received from database." }
                : new FeeCollectResponse { Result = res.Result, Message = res.Message, ReceiptNo = res.ReceiptNo, TotalPaid = res.TotalPaid };

        // optional allow-list:  "Payments": { "AllowedReturnBaseUrls": [ "https://school.example.com" ] }
        private bool IsAllowedReturnUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var u) || (u.Scheme != "https" && u.Scheme != "http")) return false;
            var allowed = _configuration.GetSection("Payments:AllowedReturnBaseUrls").Get<string[]>();
            return allowed == null || allowed.Length == 0 ||
                   allowed.Any(a => a.TrimEnd('/').Equals(url.TrimEnd('/'), StringComparison.OrdinalIgnoreCase));
        }

        public async Task<PagedResult<StudentFeeGetAllResponse>> GetAllStudentFeeWithPage(SearchRequest req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                if (req.PageNumber == 0 && req.PageSize == 0)
                {
                    req.PageNumber = 1;
                    req.PageSize = 10;
                }


                param.Add("@COMPANYID", req.CompanyID);
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@CLASSID", req.ClassID);
                param.Add("@SECTIONID", req.SectionID);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<StudentFeeGetAllResponse>(
                "SP_STUDENTFEE_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<StudentFeeGetAllResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentFeeGetAllResponse>
                    {
                        Data = null,
                        TotalRecords = totalRecords,
                        PageNumber = pageIndex,
                        PageSize = pageSize
                    };
                }
                return userModel;

            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }
}