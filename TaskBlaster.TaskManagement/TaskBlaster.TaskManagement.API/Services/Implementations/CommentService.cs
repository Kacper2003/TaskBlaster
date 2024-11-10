using TaskBlaster.TaskManagement.API.Services.Interfaces;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Models.InputModels;

namespace TaskBlaster.TaskManagement.API.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsAssociatedWithTaskAsync(int taskId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Task ID must be greater than 0");
        }

        return await _commentRepository.GetCommentsAssociatedWithTaskAsync(taskId);
    }

    public async Task AddCommentToTaskAsync(int taskId, CommentInputModel comment, string email)
    {
        await _commentRepository.AddCommentToTaskAsync(taskId, comment, email);
    }

    public async Task RemoveCommentFromTaskAsync(int taskId, int commentId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Task ID must be greater than 0");
        }
        if (commentId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(commentId), "Comment ID must be greater than 0");
        }
        await _commentRepository.RemoveCommentFromTaskAsync(taskId, commentId);
    }
}