using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ExamReportViewModel
    {
    }

    public class ExamTermReportCardRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int ExamGroupID { get; set; }
        public int? StudentID { get; set; }
        public bool UseWeightage { get; set; } = false;
        public bool ShowPromotion { get; set; } = false;
    }
    public class ExamTermReportHeaderResponse
    {
        public string? CompanyName { get; set; }
        public string? SchoolCode { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public string? ClassTeacher { get; set; }
        public string? ExamGroupName { get; set; }
        public string? SessionName { get; set; }
        
        public string? SignOfTeacher { get; set; }
        public string? SignOfParents { get; set; }
        public string? SignOfPrincipal { get; set; }

        public string? PrintLogo { get; set; }
        public string? AdminLogo { get; set; }
        public string? AdminSmallLogo { get; set; }
        public string? WaterMarkLogo { get; set; }
    }
    public class ExamTermReportStudentResponse
    {
        public int StudentID { get; set; }
        public string? RollNo { get; set; }
        public string? SRCode { get; set; }
        public string? StudentName { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public DateTime? DOB { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? PhotoFile { get; set; }
    }
    public class ExamTermReportExamResponse
    {
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public decimal? MaxMark { get; set; }
    }
    public class ExamTermReportMarksResponse
    {
        public int StudentID { get; set; }
        public int SubjectID { get; set; }
        public int ExamID { get; set; }
        public decimal? Score { get; set; }
        public bool IsAbsent { get; set; }
    }
    public class ExamTermReportSubjectTotalResponse
    {
        public int StudentID { get; set; }
        public int SubjectID { get; set; }
        public string? SubjectName { get; set; }
        public decimal? TotalObtained { get; set; }
        public decimal? TotalMax { get; set; }
        public decimal? Percentage { get; set; }
        public string? Grade { get; set; }
    }
    public class ExamTermReportCoScholasticResponse
    {
        public int StudentID { get; set; }
        public string? ParentTitle { get; set; }
        public string? Title { get; set; }
        public string? Grade { get; set; }
    }
    public class ExamTermReportRemarksResponse
    {
        public int StudentID { get; set; }
        public string? Remark { get; set; }
    }
    public class ExamTermReportPromotionResponse
    {
        public int StudentID { get; set; }
        public string? Result { get; set; }
        public string? PromotedToClass { get; set; }
        public string? PromotedToSection { get; set; }
    }
    public class AttendanceViewModel
    {
        public int StudentID { get; set; }
        public decimal? Attendance { get; set; }
        public decimal? SchoolDays { get; set; }
    }
    public class ExamTermReportCardResponse
    {
        public int Result { get; set; }
        public ExamTermReportHeaderResponse? Header { get; set; }

        public List<ExamTermReportStudentResponse> Students { get; set; }
            = new();

        public List<ExamTermReportExamResponse> Exams { get; set; }
            = new();

        public List<ExamTermReportMarksResponse> Marks { get; set; }
            = new();

        public List<ExamTermReportSubjectTotalResponse> SubjectTotals { get; set; }
            = new();

        public List<ExamTermReportCoScholasticResponse> CoScholastic { get; set; }
            = new();

        public List<AttendanceViewModel> Attendance { get; set; }
            = new();

        public List<ExamTermReportRemarksResponse> Remarks { get; set; }
            = new();

        public List<ExamTermReportPromotionResponse> Promotion { get; set; }
            = new();
    }
}
