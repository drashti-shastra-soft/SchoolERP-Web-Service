using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class ExamReportService: IExamReportService
    {
        private readonly IConfiguration _configuration;
        public ExamReportService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse<ExamTermReportCardResponse>>
    GetExamTermReportCardAsync(ExamTermReportCardRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@ExamGroupID", request.ExamGroupID);
                parameters.Add("@StudentID", request.StudentID);
                parameters.Add("@UseWeightage", request.UseWeightage);
                parameters.Add("@ShowPromotion", request.ShowPromotion);

                using var multi = await conn.QueryMultipleAsync(
                    "usp_Exam_TermReportCard",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var response = new ExamTermReportCardResponse
                {
                    // 1. Header
                    Header = await multi
                        .ReadFirstOrDefaultAsync<ExamTermReportHeaderResponse>(),

                    // 2. Students
                    Students = (await multi
                        .ReadAsync<ExamTermReportStudentResponse>())
                        ?.ToList() ?? new List<ExamTermReportStudentResponse>(),

                    // 3. Exams
                    Exams = (await multi
                        .ReadAsync<ExamTermReportExamResponse>())
                        ?.ToList() ?? new List<ExamTermReportExamResponse>(),

                    // 4. Marks
                    Marks = (await multi
                        .ReadAsync<ExamTermReportMarksResponse>())
                        ?.ToList() ?? new List<ExamTermReportMarksResponse>(),

                    // 5. Subject Totals
                    SubjectTotals = (await multi
                        .ReadAsync<ExamTermReportSubjectTotalResponse>())
                        ?.ToList() ?? new List<ExamTermReportSubjectTotalResponse>(),

                    // 6. Co-Scholastic
                    CoScholastic = (await multi
                        .ReadAsync<ExamTermReportCoScholasticResponse>())
                        ?.ToList() ?? new List<ExamTermReportCoScholasticResponse>(),

                    // 7. Attendance
                    Attendance = (await multi
                        .ReadAsync<AttendanceViewModel>())
                        ?.ToList() ?? new List<AttendanceViewModel>(),

                    // 8. Remarks
                    Remarks = (await multi
                        .ReadAsync<ExamTermReportRemarksResponse>())
                        ?.ToList() ?? new List<ExamTermReportRemarksResponse>(),

                    // 9. Promotion
                    Promotion = (await multi
                        .ReadAsync<ExamTermReportPromotionResponse>())
                        ?.ToList() ?? new List<ExamTermReportPromotionResponse>()
                };

                return new ApiResponse<ExamTermReportCardResponse>
                {
                    Success = true,
                    Message = "SUCCESS",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ExamTermReportCardResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
