namespace SchoolERPWebService.Domain
{
    // ---------------- Fee list (SP_STUDENT_FEE_TEMPLATE_LIST) ----------------
    public class StudentFeeListRequest
    {
        public int StudentId { get; set; }
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
    }

    public class StudentFeeTemplateListItem
    {
        public int MapStudentsFeeId { get; set; }
        public int FeeTemplateId { get; set; }
        public string? FeesGroup { get; set; }
        public string? FeesCode { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? LateFeeDate { get; set; }
        public decimal LateFeeAmount { get; set; }
        public string? Status { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentId { get; set; }
        public string? PaymentMode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
    }

    // (same shape you already use in the view)
    //public class StudentFeeTemplateTotalResponse
    //{
    //    public decimal TotalAmount { get; set; }
    //    public decimal TotalDiscount { get; set; }
    //    public decimal TotalFine { get; set; }
    //    public decimal TotalPaid { get; set; }
    //    public decimal TotalBalance { get; set; }
    //}

    //public class StudentFeeTemplateListResponse
    //{
    //    public int Result { get; set; }
    //    public string? Message { get; set; }
    //    public string? TechnicalMessage { get; set; }
    //    public List<StudentFeeTemplateListItem> Data { get; set; } = new();
    //    public StudentFeeTemplateTotalResponse? Total { get; set; }
    //}

    // ---------------- Collect (offline) ----------------

    public class FeeCollectRequest
    {
        public int StudentId { get; set; }
        public int CompanyId { get; set; }                    // set in the MVC controller
        public int SessionId { get; set; }                    // set in the MVC controller
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; } = "Cash";
        public string? Note { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string? BankName { get; set; }
        public string? ReferenceNo { get; set; }

        // Matches the frontend's collectPayload.FeesJson exactly: a JSON-encoded
        // string of fee-line objects, passed straight through to @FEESJSON
        // (NVARCHAR(MAX)). Do NOT re-derive this from any other field.
        public string? FeesJson { get; set; }

        public int? UserId { get; set; }

        // NOTE: MapStudentsFeeIds removed from this path — it was never
        // populated by this caller (the frontend doesn't send it), and the
        // service was building @FEESJSON from it, which is what produced the
        // empty "[]" payload. If some OTHER caller in this codebase genuinely
        // needs a plain ID-list shape, that caller should populate its own
        // FeesJson string (e.g. "[1,2,3]") rather than reintroducing a second,
        // separately-bound field — the SP already accepts that scalar-array
        // shape as a fallback.
    }

    public class FeeCollectResponse
    {
        public int Result { get; set; }                       // > 0 = CollectionId
        public string? Message { get; set; }
        public string? ReceiptNo { get; set; }
        public decimal TotalPaid { get; set; }
        public string? TechnicalMessage { get; set; }
    }

    // ---------------- Online ----------------
    public class OnlineStartRequest
    {
        public int StudentId { get; set; }
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public int UserId { get; set; }                       // set by API
        public string? IpAddress { get; set; }                // set by API
        public string? GatewayName { get; set; }              // null = Payments:DefaultGateway
        public string? PayerName { get; set; }
        public string? PayerEmail { get; set; }
        public string? PayerMobile { get; set; }
        public string ReturnBaseUrl { get; set; } = "";       // e.g. https://school.example.com (the CLIENT site)
        public List<int> MapStudentsFeeIds { get; set; } = new();
        public List<int> DetailIds { get; set; } = new();
    }

    public class OnlineStartResponse
    {
        public int Result { get; set; }
        public string? Message { get; set; }
        public string? RedirectUrl { get; set; }
        public int OnlinePaymentId { get; set; }
        public string? TechnicalMessage { get; set; }
    }

    public class OnlineFinalizeRequest
    {
        public string? GatewayName { get; set; }
        public string OrderId { get; set; } = "";             // Stripe Checkout Session id
    }

    public class OnlineCancelRequest
    {
        public int OnlinePaymentId { get; set; }
        public int UserId { get; set; }                       // set by API
    }

    // internal SP row (name differs from your existing SpResult on purpose)
    public class FeeCollectionSpResult
    {
        public int Result { get; set; }
        public string? Message { get; set; }
        public string? ReceiptNo { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }



    // ---- App-level request/response (used by the service + both controllers) ---

    public class RefundRequest
    {
        // The receipt id (tbl_StudentFeeCollection.CollectionId), NOT an "online
        // payment id" — offline payments never get an OnlinePaymentId row at all,
        // so CollectionId is the only identifier every payment (offline or online)
        // is guaranteed to have.
        public int CollectionId { get; set; }

        // Null = refund the full paid amount.
        public decimal? Amount { get; set; }

        public string? Reason { get; set; }

        // Set server-side from the authenticated user; never trust it from the client.
        public int UserId { get; set; }
        public int CompanyId { get; set; }
    }

    public class RefundResponse
    {
        public int Result { get; set; }
        public string? Message { get; set; }
        public string? RefundId { get; set; }
        public decimal? RefundedAmount { get; set; }
        public string? TechnicalMessage { get; set; }
    }

    // Internal projection used by FeeCollectionService.RefundAsync to look up
    // how the original payment was made before deciding whether a gateway
    // call is needed. Maps 1:1 to the SP_STUDENT_FEE_PAYMENT_GET result set.
    public class OnlinePaymentLookup
    {
        public string Status { get; set; } = "";
        public decimal Amount { get; set; }
        public string? GatewayName { get; set; }
        public string? GatewayPaymentId { get; set; }
    }

    public class StudentFeeGetAllResponse
    {
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int StudentId { get; set; }

        public int? ClassID { get; set; }
        public string? ClassName { get; set; }

        public int? SectionID { get; set; }
        public string? SectionName { get; set; }

        public string? Name { get; set; }
        public string? FatherName { get; set; }
        public string? MobileNo { get; set; }
        public string? AdmissionNo { get; set; }

        public string? RollNo { get; set; }
        public DateTime? DOB { get; set; }
    }
}