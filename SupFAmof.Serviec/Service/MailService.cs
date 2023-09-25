using MimeKit;
using MailKit;
using MailKit.Security;
using Castle.Core.Configuration;
using SupFAmof.Service.DTO.Request;
using Microsoft.Extensions.Configuration;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.Service
{
    public class MailService : ServiceInterface.IMailService
    {

        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public MailService(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configuration = configuration;
        }

    public async Task SendVerificationEmail(string email)
        {
            MailSettings mailSettings = GetMailSettings();
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Verification Your Account";
            string htmlBody = System.IO.File.ReadAllText("..\\SupFAmof.Serviec\\MailTemplate\\VeryficationEmailTemplate.html");
            // Create a multipart/alternative message body to support both plain text and HTML
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlBody;

            message.Body = bodyBuilder.ToMessageBody();
            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await message.WriteToAsync(emailsavefile);


            }

            smtp.Disconnect(true);


        }

        private MailSettings GetMailSettings()
        {
            var mailSettings = new MailSettings();
        _configuration.GetSection("MailSettings").Bind(mailSettings);
            return mailSettings;
        }
    }
}

