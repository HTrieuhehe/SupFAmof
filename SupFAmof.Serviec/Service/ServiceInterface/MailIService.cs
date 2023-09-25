namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IMailService
    {
        Task SendVerificationEmail(string email);
    }
}
