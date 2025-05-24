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
            sb.Append(@"
        <div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;background:#f9f9f9;padding:24px;border-radius:8px;border:1px solid #e0e0e0;'>
            <h2 style='color:#2e6caf;margin-top:0;'>Your Selected To-Do Items</h2>
            <ul style='list-style:none;padding:0;margin:0;'>
    ");
            foreach (var item in items)
            {
                sb.Append($@"
            <li style='background:#fff;margin-bottom:12px;padding:16px 12px;border-radius:6px;box-shadow:0 1px 3px rgba(44,62,80,0.06);'>
                <div><strong style='color:#2e6caf;'>ID:</strong> {item.Id}</div>
                <div><strong style='color:#555;'>Description:</strong> {item.Description}</div>
                <div><strong style='color:#555;'>Status:</strong> <span style='color:{(item.Status == "Completed" ? "#388e3c" : "#e67c12")};'>{item.Status}</span></div>
            </li>
        ");
            }
            sb.Append(@"
            </ul>
            <div style='margin-top:24px;font-size:13px;color:#999;text-align:center;'>
                <em>Sent by StudentERP Team &middot; Have a productive day!</em>
            </div>
        </div>
    ");
            return sb.ToString();
        }
    }
}