using System.Reflection;
using Microsoft.OpenApi.Models;

namespace ScanImeiApp.Swagger;

/// <summary>
/// Класс представляет конфигурацию подключения Swagger.
/// </summary>
public static class SwaggerConfig
{
    /// <summary>
    /// Подключить Swagger.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterSwagger(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ScanImeiApp.Web API", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
    }
}