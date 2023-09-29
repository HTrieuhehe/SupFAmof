using MimeKit;
using MailKit;
using MailKit.Security;
using Castle.Core.Configuration;
using SupFAmof.Service.DTO.Request;
using Microsoft.Extensions.Configuration;
using SupFAmof.Service.Service.ServiceInterface;
using Microsoft.Extensions.Options;

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

        public async Task<bool> SendEmailToUser(MailRequest request)
        {
            try
            {
                var message = new MimeMessage();
                message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
                message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                message.To.Add(MailboxAddress.Parse(request.Email));
                message.Subject = request.Subject;

                foreach(var path in _mailPaths.Paths)
                {
                    if (path.Key == request.Type.Trim())
                    {
                        string? htmlBody = System.IO.File.ReadAllText(path.Value);

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

                        else if(request.MailBookingRequest != null && request.Code == 0)
                        {
                            bodyBuilder.HtmlBody = htmlBody
                                .Replace("{code}", request.MailBookingRequest.RegistrationCode.ToString())
                                .Replace("{name}", request.MailBookingRequest.PostName.ToString())
                                .Replace("{dateFrom}", request.MailBookingRequest.DateFrom.ToString())
                                .Replace("{dateTo}", request.MailBookingRequest.DateTo.ToString())
                                .Replace("{positionName}", request.MailBookingRequest.PositionName.ToString())
                                .Replace("{TimeFrom}", request.MailBookingRequest.TimeFrom.ToString())
                                .Replace("{Timeto}", request.MailBookingRequest.TimeTo.ToString())
                                .Replace("{schoolName}", request.MailBookingRequest.SchoolName.ToString())
                                .Replace("{location}", request.MailBookingRequest.Location.ToString())
                                .Replace("{note}", request.MailBookingRequest.Note.ToString());

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

