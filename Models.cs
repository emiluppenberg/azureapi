public class Student
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
  public int Age { get; set; }
  public int CourseId { get; set; }
  public virtual Course? Course { get; set; }
}

public class Course
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
}