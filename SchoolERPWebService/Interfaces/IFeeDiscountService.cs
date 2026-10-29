using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IFeeDiscountService
    {
        Task<List<DropdownModel>> GetFeeConcessionDropdownAsync(
    int? companyId);
        Task<List<DropdownModel>> GetFeeDiscountByUserDropdownAsync(
      int? companyId);

        Task<List<StudentFeeDiscountRowResponse>>
   GetStudentFeeDiscountRowsAsync(
       StudentFeeDiscountRowsRequest request);

        Task<StudentFeeDiscountHeaderResponse?>
    GetStudentFeeDiscountHeaderAsync(
        StudentFeeDiscountHeaderRequest request);

        Task<StudentFeeDiscountSaveResponse>
    SaveStudentFeeDiscountAsync(
        StudentFeeDiscountSaveRequest request);
    }
}
