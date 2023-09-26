using SupFAmof.Service.DTO.Request;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IMailService
    {
       Task<bool> SendEmail(MailRequest request);
    }
}
