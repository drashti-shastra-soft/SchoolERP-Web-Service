using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class StaffIDCardService: IStaffIDCardService
    {
        private readonly IConfiguration _configuration;
        public StaffIDCardService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all staff ID card templates for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <returns>List of staff ID card templates.</returns>
        public List<StaffIDCardViewModel> GetAll(int companyId, int sessionId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<StaffIDCardViewModel>(
                "sp_StaffIDCard_GetAll",
                new
                {
                    CompanyID = companyId,
                    SessionID = sessionId
                },
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        /// <summary>
        /// Retrieves a staff ID card template by its ID.
        /// </summary>
        /// <param name="id">ID card template ID.</param>
        /// <returns>Staff ID card template details.</returns>
        public StaffIDCardViewModel GetByID(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<StaffIDCardViewModel>(
                "sp_StaffIDCard_GetByID",
                new
                {
                    IDCardID = id
                },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Creates or updates a staff ID card template.
        /// </summary>
        /// <param name="request">Staff ID card request model.</param>
        /// <param name="userId">User identifier.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <returns>Operation result and message.</returns>
        public (int Result, string Message) Upsert(StaffIDCardUpsertRequest request, int userId, int companyId, int sessionId)
        {
            if (request.IDCardID > 0)
            {
                var existing = GetByID(request.IDCardID);

                if (existing != null)
                {
                    if (request.BackgroundImage == null || request.BackgroundImage.Length == 0)
                    {
                        request.BackgroundImage = existing.BackgroundImage;
                        request.BackgroundImageType = existing.BackgroundImageType;
                        request.BackgroundImageName = existing.BackgroundImageName;
                    }

                    if (request.LogoImage == null || request.LogoImage.Length == 0)
                    {
                        request.LogoImage = existing.LogoImage;
                        request.LogoImageType = existing.LogoImageType;
                        request.LogoImageName = existing.LogoImageName;
                    }

                    if (request.SignatureImage == null || request.SignatureImage.Length == 0)
                    {
                        request.SignatureImage = existing.SignatureImage;
                        request.SignatureImageType = existing.SignatureImageType;
                        request.SignatureImageName = existing.SignatureImageName;
                    }
                }
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                "sp_StaffIDCard_Upsert",
                new
                {
                    request.IDCardID,
                    request.IDCardTitle,
                    request.SchoolName,
                    request.HeaderColor,
                    request.AddressPhoneEmail,

                    request.BackgroundImage,
                    request.BackgroundImageType,
                    request.BackgroundImageName,

                    request.LogoImage,
                    request.LogoImageType,
                    request.LogoImageName,

                    request.SignatureImage,
                    request.SignatureImageType,
                    request.SignatureImageName,

                    request.ShowStaffName,
                    request.ShowDesignation,
                    request.ShowStaffID,
                    request.ShowDepartment,
                    request.ShowDOJ,
                    request.ShowPhone,
                    request.ShowBloodGroup,
                    request.ShowStaffAddress,
                    request.DesignType,
                    request.ShowBarcode,

                    request.IsActive,
                    UserId = userId,
                    CompanyID = companyId,
                    SessionID = sessionId
                },
                commandType: CommandType.StoredProcedure
            );

            return result == default
                ? (0, "Failed to upsert Staff ID Card template.")
                : result;
        }
       
        /// <summary>
        /// Deletes a staff ID card template.
        /// </summary>
        /// <param name="id">ID card template ID.</param>
        /// <param name="userId">User identifier.</param>
        /// <returns>Operation result and message.</returns>
        public (int Result, string Message) Delete(List<int> id, int userId)
        {
            if (id == null || !id.Any())
            {
                return (0, "No students selected for deletion.");
            }

            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            string iDCardIDs = string.Join(",", id);

            var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                "sp_StaffIDCard_Delete",
                new
                {
                    IDCardID = iDCardIDs,
                    UserId = userId
                },
                commandType: CommandType.StoredProcedure
            );

            return result == default
                ? (0, "Failed to delete template.")
                : result;
        }

        /// <summary>
        /// Changes the active status of a staff ID card template.
        /// </summary>
        /// <param name="id">ID card template ID.</param>
        /// <param name="isActive">Status value.</param>
        /// <param name="userId">User identifier.</param>
        /// <returns>Operation result and message.</returns>
        //public (int Result, string Message) ToggleStatus(int id, bool isActive, int userId)
        //{
        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        //    var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
        //        "sp_StaffIDCard_ToggleStatus",
        //        new
        //        {
        //            IDCardID = id,

        //            IsActive = isActive,
        //            UserId = userId
        //        },
        //        commandType: CommandType.StoredProcedure
        //    );

        //    return result == default
        //        ? (0, "Failed to toggle status.")
        //        : result;
        //}

        public (bool success, string message) ToggleStaffIDCardStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@IDCARDIDS", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_StaffIDCard_TOGGLESTATUS",
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
    }
}
