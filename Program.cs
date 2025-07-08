using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Serilog;
using HostalIslaAzul.Infrastructure;
using HostalIslaAzul.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() 
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) 
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console() 
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) 
    .CreateLogger();

builder.Host.UseSerilog(); // Usar Serilog como proveedor de logging

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true; 
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar DbContext
builder.Services.AddDbContext<HostalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure())); 

// Registro de servicios
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<HabitacionService>();
builder.Services.AddScoped<AmaDeLlavesService>();
builder.Services.AddScoped<ReservaHostedService>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:9000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); 
    });
});

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowOrigin"); 
app.MapControllers();

// Manejo global de excepciones
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"Error interno del servidor. Por favor, intenta de nuevo.\"}");
        Log.Error("Error no manejado en la solicitud: {RequestPath}", context.Request.Path);
    });
});

try
{
    Log.Information("Iniciando la aplicación HostalIslaAzul...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciarse.");
    throw; // Relanzar para que el sistema operativo registre el error
}
finally
{
    Log.CloseAndFlush(); 
}