using Microsoft.EntityFrameworkCore;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.Models.Dtos;

namespace TaskBlaster.TaskManagement.DAL.Implementations;

public class StatusRepository : IStatusRepository
{
    private readonly TaskManagementDbContext _dbContext;

    public StatusRepository(TaskManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<StatusDto>> GetAllStatusesAsync()
    {
        return await _dbContext.Statuses
            .Select(u => new StatusDto
            {
                Id = u.Id,
                Name = u.Name,
                Description = u.Description
            })
            .ToListAsync();
    }
}