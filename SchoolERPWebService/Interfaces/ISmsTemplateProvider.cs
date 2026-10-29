using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ISmsTemplateProvider
    {
        SmsGatewayTemplate? GetBySmsTypeName(string smsTypeName);
    }
}
