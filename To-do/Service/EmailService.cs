using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using To_do.Data;
using To_do.Controllers;

namespace To_do.Service
{
    public class EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

        public async Task SendTodoEmailAsync(string toEmail, List<HomeController.TodoItem> items)
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSSL
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = "Your Selected ToDo Items",
                IsBodyHtml = true,
                Body = BuildEmailBody(items)
            };

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        private static string BuildEmailBody(List<HomeController.TodoItem> items)
        {
            var sb = new StringBuilder();
            sb.Append("<h3>Your Selected ToDo Items</h3><ul>");
            foreach (var item in items)
            {
                sb.Append($"<li><strong>ID:</strong> {item.Id} | <strong>Description:</strong> {item.Description} | <strong>Status:</strong> {item.Status}</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
    }
}