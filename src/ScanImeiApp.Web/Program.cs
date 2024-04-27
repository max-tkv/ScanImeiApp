using System.Reflection;
using Microsoft.OpenApi.Models;
using ScanImeiApp.Extensions;
using ScanImeiApp.Tesseract;
using ScanImeiApp.Tesseract.Extensions;

public class Program
{
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

        // Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ScanImeiApp.Web API", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        // Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScanImeiApp.Web API V1");
        });

        app.MapDefaultControllerRoute();

        TesseractLinuxLoaderFix.Patch();

        app.Run();
    }
}