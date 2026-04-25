using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<LibraryManagement.Api.Repositories.IBookRepository, LibraryManagement.Api.Repositories.BookRepository>();
builder.Services.AddScoped<LibraryManagement.Api.Services.IBookService, LibraryManagement.Api.Services.BookService>();

builder.Services.AddScoped<LibraryManagement.Api.Repositories.IMemberRepository, LibraryManagement.Api.Repositories.MemberRepository>();
builder.Services.AddScoped<LibraryManagement.Api.Services.IMemberService, LibraryManagement.Api.Services.MemberService>();

builder.Services.AddScoped<LibraryManagement.Api.Repositories.IBorrowRecordRepository, LibraryManagement.Api.Repositories.BorrowRecordRepository>();
builder.Services.AddScoped<LibraryManagement.Api.Services.IBorrowRecordService, LibraryManagement.Api.Services.BorrowRecordService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();