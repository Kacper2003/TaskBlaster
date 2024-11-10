using Microsoft.EntityFrameworkCore;
using TaskBlaster.TaskManagement.DAL.Entities;

namespace TaskBlaster.TaskManagement.DAL;

public class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
         : base(options) { }

    public DbSet<Entities.Task> Tasks { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TaskTag> TaskTags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Priority> Priorities { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TaskNotification> TaskNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite key for TaskTag
        modelBuilder.Entity<TaskTag>()
            .HasKey(tt => new { tt.TaskId, tt.TagId });

        // Configure relationships between Task and User for AssignedTo and CreatedBy
        modelBuilder.Entity<Entities.Task>()
            .HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Entities.Task>()
            .HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskNotification>()
            .HasOne(tn => tn.Task)
            .WithMany(t => t.TaskNotifications)
            .HasForeignKey(tn => tn.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

    }


}

