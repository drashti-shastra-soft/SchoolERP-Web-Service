using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IFeeOpeningBalanceService
    {
        Task<List<StudentFeeOpeningBalanceResponse>>
        GetOpeningBalanceAsync(
            StudentFeeOpeningBalanceGetRequest request);

        Task<StudentFeeOpeningBalanceSaveResponse>
            SaveOpeningBalanceAsync(
                StudentFeeOpeningBalanceSaveRequest request);

        Task<StudentFeeOpeningBalanceSaveResponse>
            DeleteOpeningBalanceAsync(
                StudentFeeOpeningBalanceDeleteRequest request);

        Task<List<FeeInstallmentDropdownResponse>>
        GetInstallmentDropdownAsync(int companyId, int sessionId);

        Task<List<PositionOfStudentFeeResponse>>
            GetPositionOfStudentFeeAsync(
                PositionOfStudentFeeRequest request);
    }
}
