using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IExamReportService
    {
        Task<ApiResponse<ExamTermReportCardResponse>>
    GetExamTermReportCardAsync(ExamTermReportCardRequest request);
    }
}
