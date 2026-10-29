using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPermissionsController : Controller
    {
        private readonly IUserManagementService _userManagementService;

        public UserPermissionsController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }


        [HttpGet("GetUserPermissions")]
        public async Task<IActionResult> GetUserPermissions(int userId,int roleId)
        {
            var result = await _userManagementService.GetUserPermissionsAsync(userId, roleId);
            return Ok(result);
        }
    }


}
