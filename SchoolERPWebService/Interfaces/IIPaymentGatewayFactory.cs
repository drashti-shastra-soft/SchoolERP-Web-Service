namespace SchoolERPWebService.Interfaces
{
    public interface IIPaymentGatewayFactory
    {
        Task<IPaymentGateway> GetGatewayForCompanyAsync(int companyId, int? paymentTypeId = null);
    }
}
