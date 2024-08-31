using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using MimeKit;
using MR_Backend.Models;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace MR_Backend.EmailService
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

		public async Task SendEmailNotificationAsync(string toEmail, string subject, string body)
		{
			var emailMessage = new MimeMessage();
			var from = new MailboxAddress("Fantastic Four Team", _configuration["EmailSettings1:From"]);
			var to = new MailboxAddress("Recipient Name", toEmail); // Use the recipient's name or leave it empty if not needed
			emailMessage.From.Add(from);
			emailMessage.To.Add(to);
			emailMessage.Subject = subject;

			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = body
			};


			using (var client = new MailKit.Net.Smtp.SmtpClient())
			{
				try
				{
					await client.ConnectAsync(_configuration["EmailSettings1:SmtpServer"], int.Parse(_configuration["EmailSettings1:Port"]), SecureSocketOptions.StartTls);
					await client.AuthenticateAsync(_configuration["EmailSettings1:Username"], _configuration["EmailSettings1:Password"]);
					await client.SendAsync(emailMessage);

				}
				catch (Exception ex)
				{
					throw new ApplicationException("Failed to send the email. " + ex.Message);
				}
				finally
				{
					await client.DisconnectAsync(true);
				}
			}
		}

		public void SendEmail(EmailModel emailModel)
		{
			var emailMessage = new MimeMessage();
			var from = _configuration["EmailSettings:From"];
			emailMessage.From.Add(new MailboxAddress("Fantastic Four Team", from));
			emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
			emailMessage.Subject = emailModel.Subject;
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = string.Format(emailModel.Content)
			};

			using (var client = new MailKit.Net.Smtp.SmtpClient())
			{
				try
				{
					client.Connect(_configuration["EmailSettings:SmtpServer"], 465, true);
					client.Authenticate(_configuration["EmailSettings:From"], _configuration["EmailSettings:Password"]);
					client.Send(emailMessage);
				}
				catch (Exception ex)
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
}

