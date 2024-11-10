using Microsoft.EntityFrameworkCore;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.Models.Dtos;

namespace TaskBlaster.TaskManagement.DAL.Implementations;

public class PriorityRepository : IPriorityRepository
{
    private readonly TaskManagementDbContext _dbContext;

    public PriorityRepository(TaskManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PriorityDto>> GetAllPrioritiesAsync()
    {
        return await _dbContext.Priorities
            .Select(u => new PriorityDto
            {
                Id = u.Id,
                Name = u.Name,
                Description = u.Description
            })
            .ToListAsync();
    }
}