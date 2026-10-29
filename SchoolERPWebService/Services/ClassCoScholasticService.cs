using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class ClassCoScholasticService: IClassCoScholasticService
    {
        private readonly IConfiguration _configuration;
        public ClassCoScholasticService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<ClassCoScholasticTreeItemViewModel>> GetTreeAsync(int companyId, int sessionId, int classId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var rows = conn.Query<ClassCoScholasticTreeItemViewModel>(
                       "USP_ClassCoScholastic_GetList",
                       new { CompanyID = companyId, SessionID = sessionId, ClassID = classId },
                       commandType: CommandType.StoredProcedure
                   ).ToList();

                return rows.AsList();
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<int> SaveAsync(ClassCoScholasticSaveRequest request, int userId, string? userName)
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                foreach (var id in request.CheckedIds.Distinct())
                    table.Rows.Add(id);

                var idsParam = table.AsTableValuedParameter("dbo.IntIdList");

                using var conn = new SqlConnection(
                        _configuration.GetConnectionString("DefaultConnection"));

                var rowsChanged = await conn.ExecuteScalarAsync<int>(
                    "dbo.USP_ClassCoScholastic_Save",
                    new
                    {
                        CompanyID = request.CompanyID,
                        SessionID = request.SessionID,
                        ClassID = request.ClassID,
                        CoScholasticIDs = idsParam,
                        UserID = userId,
                        UserName = userName
                    },
                    commandType: CommandType.StoredProcedure);

                return rowsChanged;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<List<DropdownModel>> GetClassCoScholasticListAsync(int companyId, int sessionId, int classId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var rows = conn.Query<DropdownModel>(
                       "SP_ClassCoScholastic_GetBySubject",
                       new { CompanyID = companyId, SessionID = sessionId, ClassID = classId },
                       commandType: CommandType.StoredProcedure
                   ).ToList();

                return rows.AsList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}

