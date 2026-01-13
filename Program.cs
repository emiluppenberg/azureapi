using System.Diagnostics;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (builder.Environment.IsDevelopment())
{
    connectionString = "Server=tcp:azure-students-db.database.windows.net,1433;Initial Catalog=azure-students-db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;";
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        builder =>
        {
            builder.WithOrigins([
                "https://localhost:7230",
                "http://localhost:5034",
                "https://azure-students-evdyadebbyexeqag.swedencentral-01.azurewebsites.net",]) // Your Blazor app URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

builder.Services.AddDbContext<StudentsDbContext>(o =>
        o.UseSqlServer(connectionString,
        sql => sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowBlazorApp");
app.UseHttpsRedirection();

app.MapPost("/add-student", async (Student student, StudentsDbContext dbContext) =>
{
    try
    {
        dbContext.Add(student);
        await dbContext.SaveChangesAsync();
        return Results.Created();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("AddStudent")
.Produces<Student>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.MapGet("/students", async (StudentsDbContext dbContext) =>
{
    try
    {
        var students = await dbContext.Students.Include(x => x.Course).ToListAsync();
        return Results.Ok(students);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex.Message);
    }
})
.WithName("GetStudents")
.Produces<List<Student>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

app.MapGet("/courses", async (StudentsDbContext dbContext) =>
{
    try
    {
        var courses = await dbContext.Courses.ToListAsync();
        return Results.Ok(courses);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Debug.WriteLine(ex.Message);
        return Results.InternalServerError(ex.Message);
    }
})
.WithName("GetCourses")
.Produces<List<Course>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

app.Run();