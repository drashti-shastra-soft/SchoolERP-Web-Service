using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IForgotPasswordService
    {
        Task<(bool Success, string Message)> GenerateTokenAsync(string email, int companyId, int sessionId, string baseUrl);
        ValidateTokenResult ValidateToken(string guid);
        (bool Success, string Message) ResetPassword(string guid, string newPassword);
    }
}
