using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBlaster.TaskManagement.Notifications.Models;
using TaskBlaster.TaskManagement.Notifications.Services.Interfaces;


namespace TaskBlaster.TaskManagement.Notifications.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly IMailService _mailService;

    public NotificationsController(IMailService mailService)
    {
        _mailService = mailService;
    }

    /// <summary>
    /// Sends a basic email
    /// </summary>
    /// <param name="inputModel">An input model used to populate the basic email</param>
    [HttpPost("emails/basic")]
    public async Task<ActionResult> SendBasicEmail([FromBody] BasicEmailInputModel inputModel)
    {
        try
        {
            // Here, you would call your email-sending service or logic.
            Console.WriteLine($"Sending basic email to {inputModel.To} with subject {inputModel.Subject} and body {inputModel.Content}");

            await _mailService.SendBasicEmailAsync(inputModel.To, inputModel.Subject, inputModel.Content, inputModel.IsHtml? EmailContentType.Html : EmailContentType.Text);

            return Ok(new { message = "Email sent successfully" });
        }
        catch (Exception ex)
        {
            // Log the exception (using a logging framework in a real scenario)
            Console.WriteLine($"Error sending email: {ex.Message}");
            
            // Return a failure response
            return StatusCode(500, new { error = "An error occurred while sending the email" });
        }
    }


    /// <summary>
    /// Sends a templated email (optional)
    /// </summary>
    /// <param name="inputModel">An input model used to populate the templated email</param>
    [HttpPost("emails/template")]
    public Task<ActionResult> SendTemplatedEmail([FromBody] TemplateEmailInputModel inputModel)
    {
        throw new NotImplementedException();
    }
}