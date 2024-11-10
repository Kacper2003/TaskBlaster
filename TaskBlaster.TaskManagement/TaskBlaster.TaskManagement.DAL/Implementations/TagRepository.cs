using Microsoft.EntityFrameworkCore;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Models.InputModels;
using TaskBlaster.TaskManagement.DAL.Entities;

namespace TaskBlaster.TaskManagement.DAL.Implementations;

public class TagRepository : ITagRepository
{
    private readonly TaskManagementDbContext _dbContext;

    public TagRepository(TaskManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public System.Threading.Tasks.Task CreateNewTagAsync(TagInputModel inputModel)
    {
        var newTag = new Tag
        {
            Name = inputModel.Name,
            Description = inputModel.Description
        };

        _dbContext.Tags.Add(newTag);
        return _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        return await _dbContext.Tags
            .Select(u => new TagDto
            {
                Id = u.Id,
                Description = u.Description,
                Name = u.Name
            })
            .ToListAsync();
    }
}