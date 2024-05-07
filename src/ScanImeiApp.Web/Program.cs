using ScanImeiApp.Extensions;
using ScanImeiApp.Swagger;
using ScanImeiApp.Tesseract;
using ScanImeiApp.Tesseract.Extensions;
using Serilog;

namespace ScanImeiApp.Web;

/// <summary>
/// Основной класс старта приложения.
/// </summary>
public class Program
{
    /// <summary>
    /// Основной метод старта приложения.
    /// </summary>
    /// <param name="args">Параметры.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.ConfigureKestrel(t =>
        {
            t.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);
        });
        
        builder.Services.AddControllersWithViews();
        builder.Services.RegisterDomain(builder.Configuration);
        builder.Services.RegisterTesseract();
        builder.Services.RegisterSwagger();
        builder.Services.AddHealthChecks();

        builder.Host.UseSerilog((context, configuration) => 
            configuration.ReadFrom.Configuration(context.Configuration));
        
        var app = builder.Build();
        
        app.MapHealthChecks("/health");
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        
        // Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScanImeiApp.Web API V1");
        });

        app.UseRouting();
        app.UseAuthorization();
        app.MapDefaultControllerRoute();
        app.UseSerilogRequestLogging();
        
        TesseractLinuxLoaderFix.Patch();
        
        app.Run();
    }
}