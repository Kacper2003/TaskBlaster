using Microsoft.EntityFrameworkCore;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.DAL.Entities;
using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Models.InputModels;
using Task = System.Threading.Tasks.Task;

namespace TaskBlaster.TaskManagement.DAL.Implementations;

public class UserRepository : IUserRepository
{
    private readonly TaskManagementDbContext _dbContext;

    public UserRepository(TaskManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateUserIfNotExists(UserInputModel inputModel)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress);

        if (existingUser == null)
        {
            var user = new User
            {
                FullName = inputModel.FullName,
                EmailAddress = inputModel.EmailAddress,
                ProfileImageUrl = inputModel.ProfileImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(); // Await the async operation to ensure it completes
        } else {
            // Update the existing user
            existingUser.FullName = inputModel.FullName;
            existingUser.ProfileImageUrl = inputModel.ProfileImageUrl;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _dbContext.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                EmailAddress = u.EmailAddress,
                ProfileImageUrl = u.ProfileImageUrl
            })
            .ToListAsync();

        return users;
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                EmailAddress = u.EmailAddress,
                ProfileImageUrl = u.ProfileImageUrl
            })
            .FirstOrDefaultAsync();

        return user;
    }
}
