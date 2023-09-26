using SupFAmof.Service.DTO.Request;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface ISendMailService
    {
       Task<bool> SendEmailToUser(MailRequest request);
    }
}
