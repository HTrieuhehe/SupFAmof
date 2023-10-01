using SupFAmof.Service.DTO.Request;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface ISendMailService
    {
       Task<bool> SendEmailVerification(MailVerificationRequest request);
       Task<bool> SendEmailBooking(List<MailBookingRequest> request);
    }
}
