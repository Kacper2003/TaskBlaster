using Microsoft.AspNetCore.Mvc;
using TaskBlaster.TaskManagement.API.Services.Interfaces;
using TaskBlaster.TaskManagement.Models.Dtos;

namespace TaskBlaster.TaskManagement.API.Controllers;

[Route("[controller]")]
[ApiController]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }

    /// <summary>
    /// Returns a list of all statuses
    /// </summary>
    /// <returns>A list of all statuses</returns>
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<StatusDto>>> GetAllStatuses()
    {
        var statuses = await _statusService.GetAllStatusesAsync();

        if (statuses == null || !statuses.Any())
        {
            return Ok(new List<StatusDto>());
        }

        return Ok(statuses);
    }
}