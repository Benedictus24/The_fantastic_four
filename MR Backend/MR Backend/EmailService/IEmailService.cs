using MR_Backend.Models;
using System.Threading.Tasks;


namespace MR_Backend.EmailService
{
    public interface IEmailService
    {
		Task SendEmailNotificationAsync(string toEmail, string subject, string body);
		void SendEmail(EmailModel emailModel);

	}
}
