using Microsoft.AspNetCore.Mvc;
using TaskBlaster.TaskManagement.API.Services.Interfaces;
using TaskBlaster.TaskManagement.Models.Dtos;

namespace TaskBlaster.TaskManagement.API.Controllers;

[Route("[controller]")]
[ApiController]
public class PrioritiesController : ControllerBase
{
    private readonly IPriorityService _priorityService;

    public PrioritiesController(IPriorityService priorityService)
    {
        _priorityService = priorityService;
    }

    /// <summary>
    /// Returns a list of all priorities
    /// </summary>
    /// <returns>A list of all priorities</returns>
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<PriorityDto>>> GetAllPriorities()
    {
        var priorities = await _priorityService.GetAllPrioritiesAsync();

        if (priorities == null || !priorities.Any())
        {
            return Ok(new List<PriorityDto>());
        }

        return Ok(priorities);
    }
}