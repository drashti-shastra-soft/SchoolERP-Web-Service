using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing organization information, such as saving campus addresses, contact details, and various system settings (like fine amounts or attendance rules) in the database.
    /// </summary>
    public class OrganisationService: IOrganisationService
    {
        private readonly IConfiguration _configuration;
        public OrganisationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all organisations from the database.
        /// Optionally includes deleted organisations based on the provided flag.
        /// </summary>
        public List<OrganisationViewModel> GetAllOrganisations(bool includeDeleted = false)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<OrganisationViewModel>(
                "sp_Organisations_GetAll",
                new
                {
                    IncludeDeleted = includeDeleted
                },
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        /// <summary>
        /// Retrieves the details of a specific organisation by its unique identifier.
        /// Returns null if the organisation does not exist.
        /// </summary>
        public OrganisationViewModel? GetOrganisationByID(int organisationID)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<OrganisationViewModel>(
                "sp_Organisations_GetByID",
                new
                {
                    OrganisationID = organisationID
                },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Creates a new organisation or updates an existing organisation.
        /// Saves organisation details, configuration settings, contact information,
        /// financial settings, and other system preferences.
        /// </summary>
        public (bool success, string message) UpsertOrganisation(OrganisationUpsertRequest request, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Organisations_Upsert",
                    new
                    {
                        request.OrganisationID,
                        request.ParentOrganisationID,
                        request.OrganisationName,
                        request.CompanyCode,
                        request.FromCode,
                        request.ToCode,
                        request.FinancialYear,
                        request.PreviousFinancialYear,
                        request.CollegeCode,
                        request.Address1,
                        request.Address2,
                        request.City,
                        request.State,
                        request.Mobile,
                        request.Phone,
                        request.Email,
                        request.Website,
                        request.Fax,
                        request.AffiliationNo,
                        request.BoardName,
                        request.SchoolStartDate,
                        request.RenewalUptoDate,
                        request.SenderID,
                        request.SMSApiKey,
                        request.SMSLabel,
                        request.UploadURL,
                        request.SessionID,
                        request.LateFinePerDay,
                        request.LibraryFinePerDay,
                        request.LibraryFineMaxPerBook,
                        request.FormSalePrice,
                        request.PunchBeforeMinute,
                        request.PunchAfterMinute,
                        request.IsFeePayAllowed,
                        request.IsChequeAllowed,
                        request.IsFinalApprovalByHOD,
                        request.IsReceiptSearchAll,
                        request.IsScholarshipEnabled,
                        request.IsAttendanceOnline,
                        request.IsGroupEnabled,
                        request.IsLeaveApplyBackDays,
                        request.IsPartialFeeAllowed,
                        request.IsCourseSemesterTemplate,
                        request.IsLeaveDefaultApproved,
                        request.IsFeeTemplateEnabled,
                        request.IsMultiReceiptCopy,
                        request.IsMultiLanguage,
                        request.IsSubstituteLeaveApproval,
                        request.IsHostelFeeTemplate,
                        request.IsAllowSessionChange,
                        request.IsOTPLogin,
                        request.IsCopyrightActive,
                        request.EnquiryMobileNo,
                        request.EnquiryEmail,
                        request.EnquiryWebsite,
                        request.CopyrightText,
                        request.RunBy,
                        request.PortalUserTypeID,
                        request.IsActive,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes an organization record from the database using the specified organization ID.
        /// Returns the operation status and corresponding message.
        /// </summary>
        /// <param name="organisationID">Unique identifier of the organization to delete.</param>
        /// <param name="userId">Identifier of the user performing the delete operation.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>success</c> - Indicates whether the delete operation was successful.</description></item>
        /// <item><description><c>message</c> - Result message returned from the stored procedure.</description></item>
        /// </list>
        /// </returns>
        public (bool success, string message) DeleteOrganisation(int organisationID, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Organisations_Delete",
                    new
                    {
                        OrganisationID = organisationID,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result?.Result ?? 0) == 1,
                    result?.Message?.ToString() ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Updates the active or inactive status of an organization.
        /// Returns the operation status and corresponding message.
        /// </summary>
        /// <param name="organisationID">Unique identifier of the organization.</param>
        /// <param name="isActive">New active status value.</param>
        /// <param name="userId">Identifier of the user performing the update.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>success</c> - Indicates whether the status update was successful.</description></item>
        /// <item><description><c>message</c> - Result message returned from the stored procedure.</description></item>
        /// </list>
        /// </returns>
        public (bool success, string message) ToggleOrganisationStatus(int organisationID, bool isActive, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Organisations_ToggleStatus",
                    new
                    {
                        OrganisationID = organisationID,
                        IsActive = isActive,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result?.Result ?? 0) == 1,
                    result?.Message?.ToString() ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
