using System.Diagnostics;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using To_do.Service;
using Microsoft.Extensions.Logging;
using To_do.Models;

namespace To_do.Controllers
{
    public class HomeController(ILogger<HomeController> logger, EmailService emailService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class TodoItem
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
        }

        public class SendMailRequest
        {
            public string ToEmail { get; set; }
            public List<TodoItem> Items { get; set; }
        }

        [HttpPost]
        [Route("Home/SendMail")]
        public async Task<IActionResult> SendMail([FromBody] SendMailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail))
            {
                logger.LogWarning("Email address is empty or null.");
                return BadRequest("Email address is required.");
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                logger.LogWarning("No todo items provided in the request.");
                return BadRequest("No todo items provided.");
            }

            try
            {
                await emailService.SendTodoEmailAsync(request.ToEmail, request.Items);
                logger.LogInformation("Email sent successfully to {ToEmail}.", request.ToEmail);
                return Ok("Email sent successfully.");
            }
            catch (SmtpException ex)
            {
                logger.LogError(ex, "SMTP error while sending email to {ToEmail}.", request.ToEmail);
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while sending email to {ToEmail}.", request.ToEmail);
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }
    }
}