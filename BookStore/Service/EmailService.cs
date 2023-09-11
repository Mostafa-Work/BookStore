using BookStore.ConfigModels;
using BookStore.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BookStore.Service
{
    public class EmailService : IEmailService
    {
        private const string templatePath = @"EmailTemplates/{0}.html";
        private readonly SMTPConfig smtpConfig;
        public EmailService(IOptions<SMTPConfig> smtpConfigOptions)
        {
            this.smtpConfig = smtpConfigOptions.Value;
        }
        public async Task SendEmailForEmailConfirmation(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("Hello {{UserName}}, Confirm your email", userEmailOptions.PlaceHolders);
            userEmailOptions.Body=UpdatePlaceHolders(GetEmailBody("EmailConfirm"), userEmailOptions.PlaceHolders);
            await SendEmail(userEmailOptions);
        }
        public async Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("Hello {{UserName}}, reset your password", userEmailOptions.PlaceHolders);
            userEmailOptions.Body = UpdatePlaceHolders(GetEmailBody("ForgotPassword"), userEmailOptions.PlaceHolders);
            await SendEmail(userEmailOptions);
        }
        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            MailMessage mailMessage = new MailMessage
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new MailAddress(smtpConfig.SenderAddress,smtpConfig.SenderDisplayName),
                IsBodyHtml = smtpConfig.IsBodyHTML,
                BodyEncoding=Encoding.Default
            };
            
            foreach (string toMailAdress in userEmailOptions.ToEmails)
            {
                mailMessage.To.Add(toMailAdress);
            }
            NetworkCredential networkCredential = new NetworkCredential(smtpConfig.UserName,smtpConfig.Password);

            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtpConfig.Host,
                Port = smtpConfig.Port,
                EnableSsl=smtpConfig.EnableSSL,
                UseDefaultCredentials =smtpConfig.UseDefaultCredentials,
                Credentials= networkCredential,
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
        private string UpdatePlaceHolders(string text, List<KeyValuePair<string, string>> keyValuePair)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePair != null)
            {
                foreach (KeyValuePair<string, string> placeHolder in keyValuePair)
                {
                    if (text.Contains(placeHolder.Key))
                        text = text.Replace(placeHolder.Key, placeHolder.Value);
                }
            }
            return text;
        }
        private string GetEmailBody(string templateName)
        {
            return File.ReadAllText(string.Format(templatePath,templateName));
        }
    }
}
