using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for managing different types of money (currencies) in the system.
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
     /// Gets a list of all currencies currently in the system.
     /// </summary>
        List<MstCurrencyViewModel> GetAllCurrencies(bool includeDeleted = false);

        /// <summary>
        /// Finds the details of a specific currency using its unique ID.
        /// </summary>
        MstCurrencyViewModel? GetCurrencyByID(int currencyId);

        /// <summary>
        /// Adds a new currency or updates an existing one with new rates or details.
        /// </summary>
        (bool success, string message) UpsertCurrency(MstCurrencyUpsertRequest request, int userId);

        /// <summary>
        /// Removes a currency from the system.
        /// </summary>
        (bool success, string message) DeleteCurrency(List<int> currencyId, int userId);

        /// <summary>
        /// Turns a currency's active status on or off.
        /// </summary>
        (bool success, string message) ToggleCurrencyStatus(StatusUpdateRequest request);

        Task<PagedResult<MstCurrencyViewModel>> GetAllCurrencyWithPage(SearchRequest req);
        CurrencyDetailsModel? GetCurrencyByCompanyId(int companyId);
    }
}
