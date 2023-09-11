using MailKit.Net.Smtp;
using MimeKit;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Repositories
{
    public class EmailSendRepository : IEmailSendRepository
    {
        private readonly BLEmail _bLEmail;        

        public EmailSendRepository(BLEmail bLEmail) => _bLEmail = bLEmail;

        public void SendEmail(BLNotification notification)
        {
            var emailMessage = CreateEmailMessage(notification);
            Send(emailMessage);
                       
        }

        private MimeMessage CreateEmailMessage(BLNotification notification)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _bLEmail.From));
            emailMessage.To.AddRange(notification.To);
            emailMessage.Subject = notification.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = notification.Body.ToString()
            };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_bLEmail.SmtpServer, _bLEmail.Port, true);
                client.AuthenticationMechanisms.Remove("...");
                client.Authenticate(_bLEmail.UserName, _bLEmail.Password);

                client.Send(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

    }
}
