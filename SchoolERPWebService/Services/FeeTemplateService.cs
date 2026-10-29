using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Data;
using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Services
{
    public class FeeTemplateService: IFeeTemplateService
    {
        private readonly IConfiguration _configuration;
        public FeeTemplateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //==========================================================
        // HEADER UPSERT
        //==========================================================
        public async Task<FeeTemplateResponse> UpsertFeeTemplateAsync(
            FeeTemplateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@FEETEMPLATEID", request.FEETEMPLATEID);
                parameters.Add("@COMPANYID", request.COMPANYID);
                parameters.Add("@SESSIONID", request.SESSIONID);
                parameters.Add("@CLASSID", request.CLASSID);
                parameters.Add("@SECTIONID", request.SECTIONID);
                parameters.Add("@INSTALLMENTID", request.INSTALLMENTID);
                parameters.Add("@ADMISSIONTYPE", request.ADMISSIONTYPE);
                parameters.Add("@FEECATEGORYID", request.FEECATEGORYID);
                parameters.Add("@CATEGORYID", request.CATEGORYID);
                parameters.Add("@NOOFMONTHS", request.NOOFMONTHS);
                parameters.Add("@LATEFEEDATE", request.LATEFEEDATE);
                parameters.Add("@LATEFEEAMOUNT", request.LATEFEEAMOUNT);
                parameters.Add("@REMARK", request.REMARK);
                parameters.Add("@ISACTIVE", request.ISACTIVE);
                parameters.Add("@USERID", request.USERID);
                parameters.Add("@IPADDRESS", request.IPADDRESS);

                var result = await conn.QueryFirstOrDefaultAsync<FeeTemplateResponse>(
                    "SP_FEETEMPLATE_UPSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result ?? new FeeTemplateResponse
                {
                    RESULT = 0,
                    MESSAGE = "No response received from database."
                };
            }
            catch (Exception ex)
            {
                return new FeeTemplateResponse
                {
                    RESULT = 0,
                    MESSAGE = "Unable to save fee template.",
                    TECHNICALMESSAGE = ex.Message
                };
            }
        }


        //==========================================================
        // DETAIL GRID UPSERT
        //==========================================================
        public async Task<ApiResponse> UpsertFeeTemplateDetailsAsync(
            int templateId,
            int companyId,
            int sessionId,
            List<FeeTemplateDetailRequest> details,
            int userId,
            string? ipAddress)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                // Create DataTable matching TVP_FEETEMPLATEDETAIL
                var table = new DataTable();

                table.Columns.Add("DETAILID", typeof(int));
                table.Columns.Add("SNO", typeof(int));
                table.Columns.Add("FEEHEADID", typeof(int));
                table.Columns.Add("FEEAMOUNT", typeof(decimal));
                table.Columns.Add("DISCOUNTPERCENT", typeof(decimal));
                table.Columns.Add("AMOUNT", typeof(decimal));
                table.Columns.Add("ADMISSIONTYPE", typeof(string));
                table.Columns.Add("ISDELETE", typeof(bool));

                foreach (var item in details)
                {
                    table.Rows.Add(
                        item.DETAILID,
                        item.SNO,
                        item.FEEHEADID,
                        item.FEEAMOUNT,
                        item.DISCOUNTPERCENT,
                        item.AMOUNT,
                        item.ADMISSIONTYPE,
                        item.ISDELETE
                    );
                }

                var parameters = new DynamicParameters();

                parameters.Add("@TEMPLATEID", templateId);
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);

                parameters.Add(
                    "@DETAILS",
                    table.AsTableValuedParameter("dbo.TVP_FEETEMPLATEDETAIL"));

                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);

                var result = await conn.QueryFirstOrDefaultAsync<ApiResponse>(
                    "SP_FEETEMPLATEDETAIL_UPSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result ?? new ApiResponse
                {
                    Result = 0,
                    Message = "No response received from database."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Result = 0,
                    Message = "Unable to save fee template details.",
                    TechnicalMessage = ex.Message
                };
            }
        }

        public async Task<FeeTemplateGetByIdResponse> GetFeeTemplateByIdAsync(
    int feeTemplateId,
    int companyId,
    int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@FEETEMPLATEID", feeTemplateId);
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);

                using var multi = await conn.QueryMultipleAsync(
    "SP_FEETEMPLATE_GETBYID",
    parameters,
    commandType: CommandType.StoredProcedure);

                // Result Set 1 - Header
                FeeTemplateHeaderResponse? header = null;
                if (!multi.IsConsumed)
                {
                    header = await multi.ReadFirstOrDefaultAsync<FeeTemplateHeaderResponse>();
                }

                // Result Set 2 - Details
                List<FeeTemplateDetailResponse> details = new();
                if (!multi.IsConsumed)
                {
                    details = (await multi.ReadAsync<FeeTemplateDetailResponse>()).ToList();
                }

                // Result Set 3 - Totals
                FeeTemplateTotalResponse? total = null;
                if (!multi.IsConsumed)
                {
                    total = await multi.ReadFirstOrDefaultAsync<FeeTemplateTotalResponse>();
                }
                return new FeeTemplateGetByIdResponse
                {
                    RESULT = header != null ? 1 : 0,
                    MESSAGE = header != null ? "FEE TEMPLATE FETCHED SUCCESSFULLY" : "FEE TEMPLATE NOT FOUND",
                    HEADER = header,
                    DETAILS = details,
                    TOTAL = null // computed client-side already
                };
            }
            catch (Exception ex)
            {
                return new FeeTemplateGetByIdResponse
                {
                    RESULT = 0,
                    MESSAGE = "UNABLE TO FETCH FEE TEMPLATE.",
                    TECHNICALMESSAGE = ex.Message
                };
            }
        }

        public async Task<FeeTemplatePageApiResponse> GetFeeTemplatePageAsync(
    FeeTemplatePageRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@COMPANYID", request.COMPANYID);
                parameters.Add("@SESSIONID", request.SESSIONID);
                parameters.Add("@CLASSID", request.CLASSID);
                parameters.Add("@SECTIONID", request.SECTIONID);
                parameters.Add("@INSTALLMENTID", request.INSTALLMENTID);
                parameters.Add("@SEARCHTEXT", request.SEARCHTEXT);
                parameters.Add("@ISACTIVE", request.ISACTIVE);

                parameters.Add(
                    "@PAGEINDEX",
                    request.PAGEINDEX <= 0 ? 1 : request.PAGEINDEX);

                parameters.Add(
                    "@PAGESIZE",
                    request.PAGESIZE <= 0 ? 10 : request.PAGESIZE);

                parameters.Add(
                    "@SORTCOLUMN",
                    string.IsNullOrWhiteSpace(request.SORTCOLUMN)
                        ? "CLASSNAME"
                        : request.SORTCOLUMN);

                parameters.Add(
                    "@SORTDIRECTION",
                    string.IsNullOrWhiteSpace(request.SORTDIRECTION)
                        ? "ASC"
                        : request.SORTDIRECTION);

                parameters.Add(
                    "@TOTALRECORDS",
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output);

                var data = (await conn.QueryAsync<FeeTemplateViewModel>(
                    "SP_FEETEMPLATE_GETPAGEINDEX",
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                int totalRecords = parameters.Get<int>("@TOTALRECORDS");

                return new FeeTemplatePageApiResponse
                {
                    RESULT = 1,
                    MESSAGE = "FEE TEMPLATE LIST FETCHED SUCCESSFULLY",
                    DATA = data,
                    TOTALRECORDS = totalRecords,
                    PAGEINDEX = request.PAGEINDEX,
                    PAGESIZE = request.PAGESIZE
                };
            }
            catch (Exception ex)
            {
                return new FeeTemplatePageApiResponse
                {
                    RESULT = 0,
                    MESSAGE = "UNABLE TO FETCH FEE TEMPLATE LIST.",
                    DATA = new List<FeeTemplateViewModel>(),
                    TOTALRECORDS = 0,
                    PAGEINDEX = request.PAGEINDEX,
                    PAGESIZE = request.PAGESIZE,
                    TECHNICALMESSAGE = ex.Message
                };
            }
        }

        public (bool success, string message) DeleteFeeTemplet(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string feeTempletId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@FEETEMPLATEID", feeTempletId);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_FEETEMPLATE_DELETE",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<FeeTemplateGetByClassSectionResponse>>
    GetFeeTemplateByClassSectionAsync(FeeTemplateGetByClassSectionRequest request)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@CLASSID", request.CLASSID);
            parameters.Add("@SECTIONID", request.SECTIONID);
            parameters.Add("@COMPANYID", request.COMPANYID);
            parameters.Add("@SESSIONID", request.SESSIONID);
            parameters.Add("@CATEGORYID", request.CATEGORYID);
            parameters.Add("@ADMISSIONTYPE", request.ADMISSIONTYPE);
            parameters.Add("@INSTALLMENTID", request.INSTALLMENTID);

            var result = (await conn.QueryAsync<FeeTemplateGetByClassSectionResponse>(
            "SP_FEETEMPLATE_GETBYCLASSSECTION",
            parameters,
            commandType: CommandType.StoredProcedure
        )).ToList();

            var response = result.Select(x => new FeeTemplateGetByClassSectionResponse
            {
                FEETEMPLATEID = x.FEETEMPLATEID,
                CLASSID = x.CLASSID,
                CLASSNAME = x.CLASSNAME,
                SECTIONID = x.SECTIONID,
                SECTIONNAME = x.SECTIONNAME,
                INSTALLMENTID = x.INSTALLMENTID,
                ADMISSIONTYPE = x.ADMISSIONTYPE,
                FEECATEGORYID = x.FEECATEGORYID,
                FEECATEGORYNAME = x.FEECATEGORYNAME,
                CATEGORYID = x.CATEGORYID,
                StudentCategoryName = x.StudentCategoryName,
                NOOFMONTHS = x.NOOFMONTHS,
                LATEFEEDATE = x.LATEFEEDATE,
                LATEFEEAMOUNT = x.LATEFEEAMOUNT,
                REMARK = x.REMARK,
                ISACTIVE = x.ISACTIVE,
                TOTALAMOUNT = x.TOTALAMOUNT                
            }).ToList();

            return result;
        }


        public async Task<StudentFeeTemplateDetailsResponse?> GetFeeTemplateDetailsAsync(
    int feeTemplateId)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@FeeTemplateId", feeTemplateId, DbType.Int32);

            using var multi = await connection.QueryMultipleAsync(
                "dbo.SP_FeeTemplate_GetDetails",
                parameters,
                commandType: CommandType.StoredProcedure);

            // Result Set 1
            var header = await multi.ReadFirstOrDefaultAsync<StudentFeeTemplateHeaderResponse>();

            // Result Set 2
            var details = (await multi.ReadAsync<StudentFeeTemplateDetailResponse>())
                .ToList();

            if (header == null)
                return null;

            return new StudentFeeTemplateDetailsResponse
            {
                Header = header,
                Details = details
            };
        }


        public async Task<List<StudentInstallmentSummary>>
    GetStudentInstallmentSummary(int studentId,int companyId,int sessionId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@StudentID", studentId);
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SessionID", sessionId);

            var result = (await conn.QueryAsync<StudentInstallmentSummary>(
            "usp_GetStudentInstallmentSummary",
            parameters,
            commandType: CommandType.StoredProcedure
        )).ToList();

            

            return result;
        }

        public async Task<List<PaymentModeSummary>>
    GetPaymentModeSummary(StudentFeeCollectionRequest request)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@CompanyId", request.CompanyId);
            parameters.Add("@SessionId", request.SessionId);
            parameters.Add("@FromDate", request.FromDate);
            parameters.Add("@ToDate", request.ToDate);
            parameters.Add("@UserId", request.UserId);

            var result = (await conn.QueryAsync<PaymentModeSummary>(
            "usp_GetPaymentModeSummary",
            parameters,
            commandType: CommandType.StoredProcedure
        )).ToList();



            return result;
        }
    }
}

