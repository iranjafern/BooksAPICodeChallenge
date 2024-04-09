using Books.API.Services.IRepositories;
using Books.API.Extensions;
using Books.API.GoogleBooksApi;
using Books.API.Security;
using Books.DataAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Books.API.Services.Repositories;
using Books.API.ExceptionManager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<ExceptionManager>();
builder.Services.AddDbContext<BooksDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BooksConnection"));
});

//Add logging using Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddScoped<IBookSearchHistoryRepository, BookSearchHistoryRepository>();
builder.Services.Configure<OktaJwtVerificationOptions>(builder.Configuration.GetSection("Okta"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGoogleApiManager, GoogleApiManager>();
builder.Services.Configure<GoogleBooksAPI>(builder.Configuration.GetSection("GoogleBooksAPI"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.AddSwagger();
builder.AddAllowedOrigins();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(o => { });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
