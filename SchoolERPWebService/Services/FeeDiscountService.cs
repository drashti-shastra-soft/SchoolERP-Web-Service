using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class FeeDiscountService: IFeeDiscountService
    {
        private readonly IConfiguration _configuration;
        public FeeDiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<DropdownModel>> GetFeeConcessionDropdownAsync(
    int? companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId, DbType.Int32);

                var result = await conn.QueryAsync<DropdownModel>(
                    "SP_FEE_CONCESSION_DROPDOWN",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch
            {
                return new List<DropdownModel>();
            }
        }

        public async Task<List<DropdownModel>> GetFeeDiscountByUserDropdownAsync(
      int? companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId, DbType.Int32);

                var result = await conn.QueryAsync<DropdownModel>(
                    "SP_FEE_DISCOUNT_BY_DROPDOWN",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch
            {
                return new List<DropdownModel>();
            }
        }

        public async Task<List<StudentFeeDiscountRowResponse>>
    GetStudentFeeDiscountRowsAsync(
        StudentFeeDiscountRowsRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@STUDENTID", request.STUDENTID);
                parameters.Add("@COMPANYID", request.COMPANYID);
                parameters.Add("@SESSIONID", request.SESSIONID);

                var result = await conn.QueryAsync<StudentFeeDiscountRowResponse>(
                    "SP_STUDENT_FEE_DISCOUNT_GET_ROWS",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch
            {
                return new List<StudentFeeDiscountRowResponse>();
            }
        }

        public async Task<StudentFeeDiscountHeaderResponse?>
    GetStudentFeeDiscountHeaderAsync(
        StudentFeeDiscountHeaderRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@STUDENTID", request.STUDENTID);
                parameters.Add("@COMPANYID", request.COMPANYID);
                parameters.Add("@SESSIONID", request.SESSIONID);
                parameters.Add("@FEEHEADID", request.FEEHEADID);

                return await conn.QueryFirstOrDefaultAsync<StudentFeeDiscountHeaderResponse>(
                    "SP_STUDENT_FEE_DISCOUNT_GET_HEADER",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return null;
            }
        }

        public async Task<StudentFeeDiscountSaveResponse>
    SaveStudentFeeDiscountAsync(
        StudentFeeDiscountSaveRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@STUDENTID", request.STUDENTID);
                parameters.Add("@COMPANYID", request.COMPANYID);
                parameters.Add("@SESSIONID", request.SESSIONID);
                parameters.Add("@CONCESSIONID", request.CONCESSIONID);
                parameters.Add("@FROMTEMPLATEID", request.FROMTEMPLATEID);
                parameters.Add("@FEEHEADID", request.FEEHEADID);
                parameters.Add("@DISCOUNTTYPE", request.DISCOUNTTYPE);
                parameters.Add("@DISCOUNTVALUE", request.DISCOUNTVALUE);
                parameters.Add("@DISCOUNTBYUSERID", request.DISCOUNTBYUSERID);
                parameters.Add("@ATTACHMENTPATH", request.ATTACHMENTPATH);
                parameters.Add("@REMARK", request.REMARK);

                var linesJson = System.Text.Json.JsonSerializer.Serialize(
                    request.LINES);

                parameters.Add("@LINESJSON", linesJson);

                parameters.Add("@USERID", request.USERID);

                var result = await conn.QueryFirstOrDefaultAsync<StudentFeeDiscountSaveResponse>(
                    "SP_STUDENT_FEE_DISCOUNT_SAVE",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result ?? new StudentFeeDiscountSaveResponse
                {
                    RESULT = 0,
                    MESSAGE = "No response received from database.",
                    TOTALDISCOUNT = 0
                };
            }
            catch (Exception ex)
            {
                return new StudentFeeDiscountSaveResponse
                {
                    RESULT = 0,
                    MESSAGE = "Unable to save student fee discount.",
                    TOTALDISCOUNT = 0
                };
            }
        }
    }
}
