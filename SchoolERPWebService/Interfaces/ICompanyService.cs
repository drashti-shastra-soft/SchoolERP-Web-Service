using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{ 
    /// <summary>
    /// This interface defines the rules for managing school companies or branches.
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// Gets a list of all companies registered in the system.
        /// </summary>
        List<MstCompanyViewModel> GetAllCompanies(bool includeDeleted = false, int? userId = null);

        /// <summary>
        /// Gets a list of companies assigned to a specific user.
        /// </summary>
        List<MstCompanyViewModel> GetCompaniesByUserId(int userId);

        /// <summary>
        /// Finds and returns the details of a specific company using its ID number.
        /// </summary>
        MstCompanyViewModel? GetCompanyByID(int companyId);

        /// <summary>
        /// Adds a new company or updates an existing one with new information.
        /// </summary>
        (bool success, string message) UpsertCompany(MstCompanyUpsertRequest request, int userId);

        /// <summary>
        /// Removes a company from the system.
        /// </summary>
        (bool success, string message) DeleteCompany(List<int> companyId, int userId);

        /// <summary>
        /// Turns a company's active status on or off (e.g., to temporarily disable it).
        /// </summary>
        (bool success, string message) ToggleStatus(StatusUpdateRequest request);

        /// <summary>
        /// Updates which company a specific user is currently working in.
        /// </summary>
        (bool success, string message) UpdateUserCurrentCompany(int userId, int companyId);

        /// <summary>
        /// Checks which company a user is currently using.
        /// </summary>
        int? GetUserCurrentCompany(int userId);

        Task<PagedResult<MstCompanyViewModel>> GetAllCompanyWithPage(SearchRequest req);
        /// <summary>
        /// Upload company logo and image
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="imageType"></param>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<(bool Success, string Message, string FileName)> UpdateCompanyImageAsync(int companyId, string imageType, IFormFile file, int userId);

        (bool success, string message) SaveCompanyLogoAsync(CompanyLogoRequest request);
        List<CompanyLogoResponse> GetAllCompanyLogoById(int? companyID = null);
    }
}
