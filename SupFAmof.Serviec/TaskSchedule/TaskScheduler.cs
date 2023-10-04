using MimeKit;
using MailKit.Security;
using Coravel.Invocable;
using NTQ.Sdk.Core.Utilities;
using Microsoft.Extensions.Options;
using SupFAmof.Service.DTO.Request;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.Service.TaskSchedule
{
    public class TaskScheduler : IInvocable
    {
        private readonly MailSettings _mailSettings;
        private readonly MailPaths _mailPaths;

        public TaskScheduler(IOptions<MailSettings> mailSettings, IOptions<MailPaths> mailPaths)
        {
            _mailSettings = mailSettings.Value;
            _mailPaths = mailPaths.Value;
        }

        public async Task Invoke()
        {
            MailVerificationRequest request = new MailVerificationRequest
            {
                Email = "vuvanmanh3012@gmail.com",
                Code = 11111,
                Subject = " Task scheduling for every 1 minute"
            };
            try
            {
                var message = new MimeMessage();
                message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
                message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                message.To.Add(MailboxAddress.Parse(request.Email));
                message.Subject = request.Subject;

                if (_mailPaths.Paths != null && _mailPaths.Paths.TryGetValue(EmailTypeEnum.VerificationMail.GetDisplayName(), out string value))
                {
                    string? htmlBody = await File.ReadAllTextAsync(value);

                    // Create a multipart/alternative message body to support both plain text and HTML
                    var bodyBuilder = new BodyBuilder();

                    if (request.Code > 0)
                    {
                        bodyBuilder.HtmlBody = htmlBody.Replace("{code}", request.Code.ToString());

                        message.Body = bodyBuilder.ToMessageBody();
                        // dùng SmtpClient của MailKit
                        using var smtp = new MailKit.Net.Smtp.SmtpClient();

                        try
                        {
                            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                            await smtp.SendAsync(message);
                            smtp.Disconnect(true);

                        }
                        catch (Exception ex)
                        {
                            // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                            Directory.CreateDirectory("mailssave");
                            var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                            await message.WriteToAsync(emailsavefile);
                            smtp.Disconnect(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
