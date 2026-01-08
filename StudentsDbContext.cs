using Microsoft.EntityFrameworkCore;

public class StudentsDbContext : DbContext
{
  public StudentsDbContext(DbContextOptions options) : base(options)
  {
  }

  protected StudentsDbContext()
  {
  }

  public DbSet<Student> Students { get; set; } = default!;
  public DbSet<Course> Courses { get; set; } = default!;
}