using DevEncurtaUrl.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DevEncurtaUrl - API",
        Description = "Api para encurtamento de links.",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Gabriel Silva",
            Email = "gabrielgbr.contato@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/gbrgabriel/")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    opt.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DevEncurtaUrlDbContext>(opt =>
    opt.UseSqlServer(connectionString)
);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    Serilog.Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.MSSqlServer(connectionString, "Logs", autoCreateSqlTable: true)
        .WriteTo.Console()
        .CreateLogger();
}).UseSerilog();

var app = builder.Build();

if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
