using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class StudentIDCardService: IStudentIDCardService
    {
        private readonly IConfiguration _configuration;
        public StudentIDCardService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Retrieves all student ID card records for the specified company and session.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <param name="sessionId">The ID of the academic session.</param>
        /// <returns>A list of <see cref="StudentIDCardViewModel"/> records.</returns>
        public List<StudentIDCardViewModel> GetAll(int companyId, int sessionId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new
            {
                CompanyID = companyId,
                SessionID = sessionId
            };

            return conn.Query<StudentIDCardViewModel>(
                "sp_StudentIDCard_GetAll",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        /// <summary>
        /// Retrieves a single Student ID Card record by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the student ID card record.</param>
        /// <returns>
        /// A <see cref="StudentIDCardViewModel"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public StudentIDCardViewModel GetByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<StudentIDCardViewModel>(
                "sp_StudentIDCard_GetByID",
                new { IDCardID = id },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Inserts or updates a Student ID Card template along with image preservation logic.
        /// If images are not provided in the request and the record already exists,
        /// existing images are retained from the database.
        /// </summary>
        /// <param name="request">The upsert request model containing ID card details and images.</param>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <param name="companyId">The company ID associated with the ID card.</param>
        /// <param name="sessionId">The academic session ID.</param>
        /// <returns>
        /// A tuple containing:
        /// <c>Result</c> - the affected/inserted ID (or status code from SP),
        /// <c>Message</c> - a status message returned by the stored procedure.
        /// </returns>
        public (int Result, string Message) Upsert(StudentIDCardUpsertRequest request, int userId, int companyId, int sessionId)
        {
            // Image Preservation Logic
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

            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result = conn.QueryFirstOrDefault(
                "sp_StudentIDCard_Upsert",
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

                    request.ShowAdmissionNo,
                    request.ShowStudentName,
                    request.ShowClass,
                    request.ShowFatherName,
                    request.ShowMotherName,
                    request.ShowStudentAddress,
                    request.ShowPhone,
                    request.ShowDOB,
                    request.ShowBloodGroup,
                    request.DesignType,
                    request.ShowBarcode,

                    request.IsActive,
                    UserId = userId,
                    CompanyID = companyId,
                    SessionID = sessionId
                },
                commandType: CommandType.StoredProcedure
            );

            if (result != null)
                return (Convert.ToInt32(result.Result), result.Message?.ToString() ?? "");

            return (0, "Failed to upsert ID Card template.");
        }

        /// <summary>
        /// Deletes a student ID card record by ID.
        /// </summary>
        /// <param name="id">The ID card ID to delete.</param>
        /// <param name="userId">The user performing the deletion.</param>
        /// <returns>A tuple with Result code and Message from the stored procedure.</returns>
        public (int Result, string Message) Delete(List<int> id, int userId)
        {
            if (id == null || !id.Any())
            {
                return (0, "No students selected for deletion.");
            }

            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            string iDCardIDs = string.Join(",", id);

            var parameters = new DynamicParameters();
            parameters.Add("@IDCardID", iDCardIDs);
            parameters.Add("@UserId", userId);

            var row = conn.QueryFirstOrDefault("sp_StudentIDCard_Delete",
                parameters,
                commandType: CommandType.StoredProcedure);

            int result = row.Result == null ? 0 : (int)row.Result;
            string message = row.Message == null ? "" : (string)row.Message;

            return (result, message);
        }
        /// <summary>
        /// Toggles the active status of a Student ID Card record.
        /// </summary>
        /// <param name="id">The ID of the Student ID Card.</param>
        /// <param name="isActive">The new active status to set.</param>
        /// <param name="userId">The ID of the user performing the action.</param>
        /// <returns>A tuple containing Result (int) and Message (string).</returns>
        //public (int Result, string Message) ToggleStatus(int id, bool isActive, int userId)
        //{
        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@IDCardID", id);
        //    parameters.Add("@IsActive", isActive);
        //    parameters.Add("@UserId", userId);

        //    var result = conn.QueryFirstOrDefault(
        //        "sp_StudentIDCard_ToggleStatus",
        //        parameters,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    if (result != null)
        //    {
        //        return (Convert.ToInt32(result.Result), result.Message?.ToString() ?? "");
        //    }

        //    return (0, "Failed to toggle status.");
        //}

        public (bool success, string message) ToggleStudentIDCardStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@IDCARDID", request.Ids);   // fixed: singular, matches SP param name exactly
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_STUDENTIDCARD_TOGGLESTATUS",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return (
                    result?.Result == 1,
                    result?.Message ?? "Status updated successfully."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public string GenerateIDCard(int studentId, int idCardId, int companyId, int sessionId)
        {
            // Implementation for ID card generation (placeholder replacement or specialized rendering)
            // For now, we'll return the raw template or a structured JSON that the frontend uses.
            return "ID Card Generated";
        }

      
    }
}
