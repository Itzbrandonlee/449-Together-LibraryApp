using LibraryManagement.Api.Data;
using LibraryManagement.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<LibraryManagement.Api.Repositories.IBookRepository, LibraryManagement.Api.Repositories.BookRepository>();
builder.Services.AddScoped<LibraryManagement.Api.Services.IBookService, LibraryManagement.Api.Services.BookService>();
builder.Services.AddScoped<LibraryManagement.Api.Repositories.IMemberRepository, LibraryManagement.Api.Repositories.MemberRepository>();
builder.Services.AddScoped<LibraryManagement.Api.Services.IMemberService, LibraryManagement.Api.Services.MemberService>();
builder.Services.AddScoped<LibraryManagement.Api.Repositories.IBorrowRecordRepository, LibraryManagement.Api.Repositories.BorrowRecordRepository>();
builder.Services.AddScoped<LibraryManagement.Api.Services.IBorrowRecordService, LibraryManagement.Api.Services.BorrowRecordService>();

var app = builder.Build();

// Global exception handling — catches unhandled exceptions from any layer
// and returns a consistent { "error": "..." } response
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();