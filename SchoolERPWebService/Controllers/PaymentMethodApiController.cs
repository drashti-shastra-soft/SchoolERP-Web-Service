using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;
using SchoolERPWebService.Interfaces;
using System.ComponentModel.Design;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing payment methods (like cash, check, or online) through the API.
    /// </summary>
    public class PaymentMethodApiController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Settings";

        public PaymentMethodApiController(IPaymentMethodService paymentService, IUserMenuPermissionService menuPerm)
        {
            _paymentService = paymentService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Gets the full list of all payment methods (like Razorpay) configured in the system.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            // Step 1: Ask the payment service to fetch all the different payment ways from the database.
            var data = _paymentService.GetAllPaymentMethods(includeDeleted);
            
            // Step 2: Send the list back to the requester.
            return Ok(new ApiResponse<List<MstPaymentMethodViewModel>> { Success = true, Data = data });
        }

        /// <summary>
        /// Gets the details of one specific payment method using its unique ID number.
        /// </summary>
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var data = _paymentService.GetPaymentMethodById(id);
            if (data == null) return Ok(new ApiResponse<MstPaymentMethodViewModel> { Success = false, Message = "Billing Integration Not Found" });
            return Ok(new ApiResponse<MstPaymentMethodViewModel> { Success = true, Data = data });
        }

        /// <summary>
        /// Gets the payment method configuration for a specific company.
        /// </summary>
        [HttpGet("GetByCompany/{companyId}")]
        public IActionResult GetByCompany(int companyId)
        {
            var data = _paymentService.GetPaymentMethodByCompany(companyId);
            if (data == null) return Ok(new ApiResponse<MstPaymentMethodViewModel> { Success = false, Message = "Billing Integration Not Found" });
            return Ok(new ApiResponse<MstPaymentMethodViewModel> { Success = true, Data = data });
        }

        /// <summary>
        /// Saves a new payment method or updates an existing one with the details you provided.
        /// </summary>
        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MstPaymentMethodUpsertRequest request)
        {
            // Step 1: Check if this is a new setup or an update to an old one.
            var isCreate = request.PaymentId <= 0;
            
            // Step 2: Verify if the user has the 'Add' or 'Edit' permission for settings.
            if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add payment methods." });
            if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit payment methods." });
 
            // Step 3: Identify which user is performing this action.
            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 1;
            
            // Step 4: Send the details to the database to be saved.
            var (success, message) = _paymentService.UpsertPaymentMethod(request, userId);
            
            // Step 5: Inform the user if the save was successful.
            return Ok(new ApiResponse<object> { Success = success, Message = message });
        }

        /// <summary>
        /// Permanently removes a payment method from the system's records.
        /// </summary>
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete payment methods." });

            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int idVal) ? idVal : 1;
            var (success, message) = _paymentService.DeletePaymentMethod(id, userId);
            return Ok(new ApiResponse<object> { Success = success, Message = message });
        }

        /// <summary>
        /// Turns a payment method on or off, determining if it can be used for new payments.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change status." });

            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int idVal) ? idVal : 1;
            var (success, message) = _paymentService.TogglePaymentMethodStatus(id, isActive, userId);
            return Ok(new ApiResponse<object> { Success = success, Message = message });
        }
    }
}
