using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    public class FeeOpeningBalanceService: IFeeOpeningBalanceService
    {
        private readonly IConfiguration _configuration;
        public FeeOpeningBalanceService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =========================================================
        // GET
        // =========================================================
        public async Task<List<StudentFeeOpeningBalanceResponse>>
            GetOpeningBalanceAsync(
                StudentFeeOpeningBalanceGetRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@STUDENTID", request.STUDENTID);
                parameters.Add("@COMPANYID", request.COMPANYID);
                parameters.Add("@SESSIONID", request.SESSIONID);

                var result = await conn.QueryAsync<StudentFeeOpeningBalanceResponse>(
                    "SP_STUDENT_FEE_OPENING_BALANCE_GET",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch
            {
                return new List<StudentFeeOpeningBalanceResponse>();
            }
        }


        // =========================================================
        // SAVE
        // =========================================================
        public async Task<StudentFeeOpeningBalanceSaveResponse>
            SaveOpeningBalanceAsync(
                StudentFeeOpeningBalanceSaveRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@STUDENTID", request.STUDENTID);
                parameters.Add("@COMPANYID", request.COMPANYID);
                parameters.Add("@SESSIONID", request.SESSIONID);

                // Convert Lines to JSON
                var linesJson = JsonSerializer.Serialize(request.LINES);

                parameters.Add("@LINESJSON", linesJson);

                parameters.Add("@USERID", request.USERID);

                var result =
                    await conn.QueryFirstOrDefaultAsync<StudentFeeOpeningBalanceSaveResponse>(
                        "SP_STUDENT_FEE_OPENING_BALANCE_SAVE",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                return result ?? new StudentFeeOpeningBalanceSaveResponse
                {
                    RESULT = 0,
                    MESSAGE = "No response received from database."
                };
            }
            catch (Exception ex)
            {
                return new StudentFeeOpeningBalanceSaveResponse
                {
                    RESULT = 0,
                    MESSAGE = "Unable to save opening balance.",
                    TECHNICALMESSAGE = ex.Message
                };
            }
        }


        // =========================================================
        // DELETE
        // =========================================================
        public async Task<StudentFeeOpeningBalanceSaveResponse>
            DeleteOpeningBalanceAsync(
                StudentFeeOpeningBalanceDeleteRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@STUDENTID", request.STUDENTID);
                parameters.Add("@COMPANYID", request.COMPANYID);
                parameters.Add("@SESSIONID", request.SESSIONID);
                parameters.Add("@USERID", request.USERID);

                var result =
                    await conn.QueryFirstOrDefaultAsync<StudentFeeOpeningBalanceSaveResponse>(
                        "SP_STUDENT_FEE_OPENING_BALANCE_DELETE",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                return result ?? new StudentFeeOpeningBalanceSaveResponse
                {
                    RESULT = 0,
                    MESSAGE = "No response received from database."
                };
            }
            catch (Exception ex)
            {
                return new StudentFeeOpeningBalanceSaveResponse
                {
                    RESULT = 0,
                    MESSAGE = "Unable to delete opening balance.",
                    TECHNICALMESSAGE = ex.Message
                };
            }
        }

        // =========================================================
        // INSTALLMENT DROPDOWN
        // =========================================================
        public async Task<List<FeeInstallmentDropdownResponse>>
            GetInstallmentDropdownAsync(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add(
                    "@COMPANYID",
                    companyId,
                    DbType.Int32);

                parameters.Add(
                    "@SESSIONID",
                    sessionId,
                    DbType.Int32);

                var result =
                    await conn.QueryAsync<FeeInstallmentDropdownResponse>(
                        "SP_FEE_INSTALLMENT_LABEL_DROPDOWN",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch
            {
                return new List<FeeInstallmentDropdownResponse>();
            }
        }


        // =========================================================
        // POSITION OF STUDENT FEE
        // =========================================================
        public async Task<List<PositionOfStudentFeeResponse>>
            GetPositionOfStudentFeeAsync(
                PositionOfStudentFeeRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add(
                    "@COMPANYID",
                    request.COMPANYID,
                    DbType.Int32);

                parameters.Add(
                    "@SESSIONID",
                    request.SESSIONID,
                    DbType.Int32);

                parameters.Add(
                    "@CLASSIDS",
                    request.CLASSIDS,
                    DbType.String);

                parameters.Add(
                    "@INSTALLMENTLABELS",
                    request.INSTALLMENTLABELS,
                    DbType.String);

                parameters.Add(
                    "@FEEHEADIDS",
                    request.FEEHEADIDS,
                    DbType.String);

                parameters.Add(
                    "@TRANSPORTFEEHEADIDS",
                    request.TRANSPORTFEEHEADIDS,
                    DbType.String);

                var result =
                    await conn.QueryAsync<PositionOfStudentFeeResponse>(
                        "SP_POSITION_OF_STUDENT_FEE",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch
            {
                return new List<PositionOfStudentFeeResponse>();
            }
        }
    }
}
