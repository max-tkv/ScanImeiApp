using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using ScanImeiApp.Contracts.Models;
using ScanImeiApp.Exceptions;
using ScanImeiApp.Web.Filters;
using SixLabors.ImageSharp;
using Xunit;

namespace ScanImeiApp.UnitTests.Filters;

/// <summary>
/// Тесты для атрибута фильтра <see cref="HandleExceptionsFilter" />.
/// </summary>
public class HandleExceptionsFilterTests : BaseUnitTests
{
    private readonly Mock<HttpContext> _httpContextMock;
    private readonly HandleExceptionsFilter _filter;

    public HandleExceptionsFilterTests()
    {
        var loggerMock = new Mock<ILogger<HandleExceptionsFilter>>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ILogger<HandleExceptionsFilter>)))
            .Returns(loggerMock.Object);
        
        _httpContextMock = new Mock<HttpContext>();
        _httpContextMock
            .SetupGet(c => c.RequestServices)
            .Returns(serviceProviderMock.Object);

        _filter = new HandleExceptionsFilter();    
    }
    
    [Fact(DisplayName = "ArgumentNullException")]
    public void OnException_NullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            _filter.OnException(null!);
        });
    }

    [Fact(DisplayName = "Не поддерживаемый формат изображения.")]
    public void OnException_UnknownImageFormatException_ReturnsBadRequestWithCorrectMessage()
    {
        // Arrange
        var context = new ExceptionContext(
            new ActionContext(_httpContextMock.Object, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>());
        context.Exception = new UnknownImageFormatException(Arg.Any<string>());
        
        // Act
        _filter.OnException(context);
        var result = context.Result as BadRequestObjectResult;
        var errorResponse = result?.Value as ErrorResponse;
        
        // Assert
        Assert.Equal("Не поддерживаемый формат изображения.", errorResponse?.Error);
    }

    [Fact(DisplayName = "Не удалось найти IMEI.")]
    public void OnException_NotFoundImeiException_ReturnsBadRequestWithCorrectMessage()
    {
        // Arrange
        var context = new ExceptionContext(
            new ActionContext(_httpContextMock.Object, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>());
        context.Exception = new NotFoundImeiException();
        
        // Act
        _filter.OnException(context);
        var result = context.Result as BadRequestObjectResult;
        var errorResponse = result?.Value as ErrorResponse;
        
        // Assert
        Assert.Equal("Не удалось найти IMEI. Пожалуйста настройте приложение.", errorResponse?.Error);
    }
    
    [Fact(DisplayName = "Не удалось получить настройки приложения.")]
    public void OnException_NotFoundAppOptionsException_ReturnsBadRequestWithCorrectMessage()
    {
        // Arrange
        var context = new ExceptionContext(
            new ActionContext(_httpContextMock.Object, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>());
        context.Exception = new NotFoundAppOptionsException();
        
        // Act
        _filter.OnException(context);
        var result = context.Result as BadRequestObjectResult;
        var errorResponse = result?.Value as ErrorResponse;
        
        // Assert
        Assert.Equal("Не удалось получить настройки приложения.", errorResponse?.Error);
    }
    
    [Fact(DisplayName = "Не удалось загрузить Tesseract.")]
    public void OnException_DllNotFoundException_ReturnsBadRequestWithCorrectMessage()
    {
        // Arrange
        var errorMessage = _fixture.Create<string>();
        var context = new ExceptionContext(
            new ActionContext(_httpContextMock.Object, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>());
        context.Exception = new DllNotFoundException(errorMessage);
        
        // Act
        _filter.OnException(context);
        var result = context.Result as BadRequestObjectResult;
        var errorResponse = result?.Value as ErrorResponse;
        
        // Assert
        Assert.Equal($"Не удалось загрузить Tesseract. Ошибка: {errorMessage}", errorResponse?.Error);
    }
    
    [Fact(DisplayName = "Произошло необработанное исключение.")]
    public void OnException_OutOfMemoryException_ReturnsBadRequestWithCorrectMessage()
    {
        // Arrange
        var context = new ExceptionContext(
            new ActionContext(_httpContextMock.Object, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>());
        context.Exception = new OutOfMemoryException();
        
        // Act
        _filter.OnException(context);
        var result = context.Result as BadRequestObjectResult;
        var errorResponse = result?.Value as ErrorResponse;
        
        // Assert
        Assert.Contains("Произошло необработанное исключение.", errorResponse?.Error);
    }
}