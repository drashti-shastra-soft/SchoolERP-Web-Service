using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class TeacherDashboardService : ITeacherDashboardService
    {
        private readonly IConfiguration _configuration;
        public TeacherDashboardService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<TeacherTimetableDto> GetTimetable(int companyId, int sessionId, int staffId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return conn.Query<TeacherTimetableDto>(
                    "SP_TEACHER_DASHBOARD_TIMETABLE",
                    new { COMPANYID = companyId, SESSIONID = sessionId, STAFFID = staffId },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception)
            {
                return new List<TeacherTimetableDto>();
            }
        }

        public List<TeacherStudentDto> GetStudents(int companyId, int sessionId, int staffId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return conn.Query<TeacherStudentDto>(
                    "SP_TEACHER_DASHBOARD_STUDENTS",
                    new { COMPANYID = companyId, SESSIONID = sessionId, STAFFID = staffId },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception)
            {
                return new List<TeacherStudentDto>();
            }
        }

        public List<TeacherHomeworkDto> GetHomework(int companyId, int sessionId, int staffId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return conn.Query<TeacherHomeworkDto>(
                    "SP_TEACHER_DASHBOARD_HOMEWORK",
                    new { COMPANYID = companyId, SESSIONID = sessionId, STAFFID = staffId },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception)
            {
                return new List<TeacherHomeworkDto>();
            }
        }

        public List<TeacherHWSubmissionDto> GetHWSubmissions(int companyId, int sessionId, int staffId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return conn.Query<TeacherHWSubmissionDto>(
                    "SP_TEACHER_DASHBOARD_HW_SUBMISSIONS",
                    new { COMPANYID = companyId, SESSIONID = sessionId, STAFFID = staffId },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception)
            {
                return new List<TeacherHWSubmissionDto>();
            }
        }

        public List<TeacherAttendanceDto> GetAttendance(int companyId, int sessionId, int staffId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return conn.Query<TeacherAttendanceDto>(
                    "SP_TEACHER_DASHBOARD_ATTENDANCE",
                    new { COMPANYID = companyId, SESSIONID = sessionId, STAFFID = staffId },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception)
            {
                return new List<TeacherAttendanceDto>();
            }
        }

    }
}
