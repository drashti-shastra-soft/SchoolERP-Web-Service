using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IMenuService
    {
        /// <summary>
        /// Gets a list of all menus available in the system.
        /// </summary>
        Task<List<MenuViewModel>> GetAllMenus();
        Task<PagedResult<MenuViewModel>> GetAllMenusWithPage(SearchRequest req);
        /// <summary>
        /// Finds the details of a specific menu item using its ID.
        /// </summary>
        Task<MenuViewModel?> GetMenuById(int menuId);

        /// <summary>
        /// Adds a new menu item or updates an existing one.
        /// </summary>
        Task <(int Result, string Message)> UpsertMenu(MenuUpsertRequest request, int userId, int mainAccountId, int sessionId, string ipAddress);

        /// <summary>
        /// Turns a menu item on or off (hides or shows it).
        /// </summary>
        Task<(int Result, string Message)> ToggleMenuStatus(StatusUpdateRequest request, int mainAccountId, int sessionId, string ipAddress);

        /// <summary>
        /// Removes a menu item from the system.
        /// </summary>
        (int Result, string Message) DeleteMenu(List<int> menuId, int userId, string ipAddress);

        /// <summary>
        /// Changes the order in which menu items appear (e.g., moving one item above another).
        /// </summary>
        (int Result, string Message) UpdateMenuOrder(List<MenuOrderRequest> orders, int userId, string ipAddress);

    }
}
