using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Data;

public class ToDoAppDbContext : DbContext
{
    public ToDoAppDbContext(DbContextOptions<ToDoAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Models.Task> Tasks { get; set; }
    public DbSet<Subtask> Subtasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Role)
                .HasConversion<int>();
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(e => e.Username)
                .IsUnique();
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .IsRequired();
        });

        // Configure Task entity
        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId);
            entity.Property(e => e.TaskId)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.TaskName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Subtask entity
        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId);
            entity.Property(e => e.SubtaskId)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Description)
                .IsRequired();
            entity.Property(e => e.CreatedDate)
                .IsRequired();
            entity.HasOne(e => e.Task)
                .WithMany(t => t.Subtasks)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

