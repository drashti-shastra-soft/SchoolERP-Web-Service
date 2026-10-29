namespace SchoolERPWebService.Domain
{
    public class DashboardModel
    {
        public int Result { get; set; }
        public string Message { get; set; }

        public int DashboardID { get; set; }
        public string? DashboardTital { get; set; }   // column name kept as-is from DB
        public string? DashboardURL { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string? RoleName { get; set; }
    }
    // ─────────────────────────────────────────────
    //  REQUEST DTO  –  Insert / Update payload
    // ─────────────────────────────────────────────
    public class DashboardRequestModel
    {
        public int DashboardID { get; set; } = 0;   // 0 = Insert, >0 = Update
        public string DashboardTital { get; set; } = string.Empty;
        public string DashboardURL { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ─────────────────────────────────────────────
    //  TOGGLE STATUS REQUEST
    // ─────────────────────────────────────────────
    public class DashboardToggleRequest
    {
        public int DashboardID { get; set; }
        public bool IsActive { get; set; }
    }

    // ─────────────────────────────────────────────
    //  MULTIPLE DELETE REQUEST
    // ─────────────────────────────────────────────
    public class DashboardDeleteRequest
    {
        public List<int> DashboardIDs { get; set; } = new();
    }

   public class DashboardPageViewModel
    {
        public List<DashboardModel> Dashboard { get; set; } = new();
    }

    public class DashboardViewModel
    {
        public int DashboardID { get; set; }
        public string? DashboardTital { get; set; }   // column name kept as-is from DB
        public string? DashboardURL { get; set; }
        public int RoleId { get; set; }
        public List<MstRoleViewModel> Roles { get; set; } = new();
        public DashboardModel? EditDashboard { get; set; }
        public string ViewType { get; set; } = "list";
    }

    public class ReqAttendanceCountModel
    {
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public int AttendanceStatus { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int? UserId { get; set; }
    }

    public class AttendanceCountModel
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int TotalStudents { get; set; }
        public int PresentStudents { get; set; }
        public int AbsentStudents { get; set; }
    }

    public class UserRoleWiseCountModel
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
    }
}
