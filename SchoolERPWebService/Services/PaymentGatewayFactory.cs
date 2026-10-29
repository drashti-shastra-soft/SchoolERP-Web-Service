using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Services
{
    public class PaymentGatewayFactory : IIPaymentGatewayFactory
    {
        private readonly IPaymentMethodService _configRepo;
        private readonly IEnumerable<IPaymentGateway> _gateways;   // one per PaymentCode: Stripe, Razorpay, ...

        public PaymentGatewayFactory(IPaymentMethodService configRepo, IEnumerable<IPaymentGateway> gateways)
        {
            _configRepo = configRepo;
            _gateways = gateways;
        }

        public async Task<IPaymentGateway> GetGatewayForCompanyAsync(int companyId, int? paymentTypeId = null)
        {
            try
            {
                var resolved = await _configRepo.GetConfigAsync(companyId, paymentTypeId)
                ?? throw new InvalidOperationException($"No active payment method configured for CompanyID {companyId}.");

                var gateway = _gateways.FirstOrDefault(g =>
                    string.Equals(g.Name, resolved.PaymentType.PaymentType, StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidOperationException($"No gateway implementation registered for '{resolved.PaymentType.PaymentCode}'.");

                await gateway.ConfigureAsync(resolved.Settings);
                return gateway;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
