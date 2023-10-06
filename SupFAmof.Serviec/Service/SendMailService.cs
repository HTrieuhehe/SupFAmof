using MimeKit;
using MailKit;
using MailKit.Security;
using Org.BouncyCastle.Cms;
using NTQ.Sdk.Core.Utilities;
using Castle.Core.Configuration;
using SupFAmof.Service.DTO.Request;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.Service
{
    public class SendMailService : ISendMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly MailPaths _mailPaths;

        public SendMailService(IOptions<MailSettings> mailSettings, IOptions<MailPaths> mailPaths)
        {
            _mailSettings = mailSettings.Value;
            _mailPaths = mailPaths.Value;
        }

        public async Task<bool> SendEmailBooking(List<MailBookingRequest> request)
        {
            try
            {
                var messages = new List<MimeMessage>();

                foreach (var recipient in request)
                {
                    var message = new MimeMessage();
                    message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
                    message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                    message.Subject = "Booking Confirmation";
                    message.To.Add(MailboxAddress.Parse(recipient.Email));

                    if (_mailPaths.Paths != null && _mailPaths.Paths.TryGetValue(EmailTypeEnum.BookingMail.GetDisplayName(), out string value))
                    {
                        string? htmlBody = System.IO.File.ReadAllText(value);

                        var bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = htmlBody
                            .Replace("{code}", recipient.RegistrationCode.ToString())
                            .Replace("{name}", recipient.PostName.ToString())
                            .Replace("{dateFrom}", recipient.DateFrom.ToString())
                            .Replace("{dateTo}", recipient.DateTo.ToString())
                            .Replace("{positionName}", recipient.PositionName.ToString())
                            .Replace("{TimeFrom}", recipient.TimeFrom.ToString())
                            .Replace("{Timeto}", recipient.TimeTo.ToString())
                            .Replace("{schoolName}", recipient.SchoolName.ToString())
                            .Replace("{location}", recipient.Location.ToString())
                            .Replace("{note}", recipient.Note.ToString());

                        message.Body = bodyBuilder.ToMessageBody();
                        messages.Add(message);
                    }
                }

                // Use a single SmtpClient to send all messages
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                try
                {
                    smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                    foreach (var message in messages)
                    {
                        await smtp.SendAsync(message);
                    }

                    smtp.Disconnect(true);
                }
                catch (Exception ex)
                {
                    // Handle exceptions if sending any of the emails fails
                    System.IO.Directory.CreateDirectory("mailssave");
                    var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                    await messages.First().WriteToAsync(emailsavefile);
                    smtp.Disconnect(true);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<bool> SendEmailVerification(MailVerificationRequest request)
        {
            try
            {
                var message = new MimeMessage();
                message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
                message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                message.To.Add(MailboxAddress.Parse(request.Email));
                message.Subject = request.Subject;

                if (_mailPaths.Paths != null && _mailPaths.Paths.TryGetValue(EmailTypeEnum.VerificationMail.GetDisplayName(), out string value))
                {
                    string? htmlBody = await System.IO.File.ReadAllTextAsync(value);

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
                            return true;
                        }
                        catch (Exception ex)
                        {
                            // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                            System.IO.Directory.CreateDirectory("mailssave");
                            var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                            await message.WriteToAsync(emailsavefile);
                            smtp.Disconnect(true);
                            return false;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

