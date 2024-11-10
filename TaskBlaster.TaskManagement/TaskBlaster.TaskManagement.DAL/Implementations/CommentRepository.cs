using Microsoft.EntityFrameworkCore;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.DAL.Entities;
using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Models.InputModels;
using Task = System.Threading.Tasks.Task;
using TaskBlaster.TaskManagement.Models.Exceptions;

namespace TaskBlaster.TaskManagement.DAL.Implementations;

public class CommentRepository : ICommentRepository
{
    private readonly TaskManagementDbContext _dbContext;

    public CommentRepository(TaskManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddCommentToTaskAsync(int taskId, CommentInputModel comment, string email)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == email)
            ?? throw new ResourceNotFoundException($"User with email {email} not found");
            
        var task = await _dbContext.Tasks.FindAsync(taskId) 
                    ?? throw new ResourceNotFoundException($"Task with id {taskId} not found");

        var newComment = new Comment
        {
            ContentAsMarkdown = comment.ContentAsMarkdown,
            Author = email,
            CreatedDate = DateTime.UtcNow,
            TaskId = taskId
        };

        _dbContext.Comments.Add(newComment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsAssociatedWithTaskAsync(int taskId)
    {
        var task = await _dbContext.Tasks.FindAsync(taskId)
            ?? throw new ResourceNotFoundException($"Task with id {taskId} not found");

        var comments = await _dbContext.Comments
            .Where(c => c.TaskId == taskId)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Author = c.Author,
                ContentAsMarkdown = c.ContentAsMarkdown,
                CreatedDate = c.CreatedDate
            })
            .ToListAsync();

        return comments;
    }

    public async Task RemoveCommentFromTaskAsync(int taskId, int commentId)
    {
        var task = await _dbContext.Tasks.FindAsync(taskId)
            ?? throw new ResourceNotFoundException($"Task with id {taskId} not found");
            
        var comment = await _dbContext.Comments
            .Where(c => c.TaskId == taskId && c.Id == commentId)
            .FirstOrDefaultAsync()
                    ?? throw new ResourceNotFoundException($"Comment with id {commentId} not found for task {taskId}");

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
}
