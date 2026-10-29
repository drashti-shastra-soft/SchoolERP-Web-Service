using Microsoft.Extensions.Options;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Services
{
    public class SmsTemplateProvider: ISmsTemplateProvider
    {
        private readonly SmsGatewayTemplateOptions _options;
        public SmsTemplateProvider(IOptions<SmsGatewayTemplateOptions> options) => _options = options.Value;

        public SmsGatewayTemplate? GetBySmsTypeName(string smsTypeName) =>
            _options.SmsGatewayTemplates.FirstOrDefault(t =>
                string.Equals(t.SmsType, smsTypeName, StringComparison.OrdinalIgnoreCase));
    }
}
