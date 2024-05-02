using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace ScanImeiApp.UnitTests;

/// <summary>
/// Базовый класс для модульных тестов.
/// </summary>
public class BaseUnitTests
{
    protected readonly Fixture _fixture;

    protected BaseUnitTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }
    
    /// <summary>
    /// Создать моки IFormFile.
    /// </summary>
    /// <param name="content">Контент.</param>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Данные IFormFile.</returns>
    protected Mock<IFormFile> CreateIFormFileMock(string content, string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream(_fixture.CreateMany<byte>().ToArray());
        var writer = new StreamWriter(ms);

        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(ms.Length);

        return fileMock;
    }

    /// <summary>
    /// Проверяет количество вызовов записи в логи.
    /// </summary>
    /// <param name="loggerMock">Мок логгера.</param>
    /// <param name="expectedLogLevel">Ожидаемый уровень логирования.</param>
    /// <param name="expectedExactly">Ожидаемое количество записей в логи.</param>
    /// <typeparam name="T">Тип объекта логирования.</typeparam>
    protected void LogVerify<T>(
        Mock<ILogger<T>> loggerMock, 
        LogLevel expectedLogLevel, 
        Times expectedExactly)
    {
        loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == expectedLogLevel),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() != null),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            expectedExactly);
    }

    /// <summary>
    /// Проверяет количество вызовов записи в логи.
    /// </summary>
    /// <param name="loggerMock">Мок логгера.</param>
    /// <param name="expectedLogLevel">Ожидаемый уровень логирования.</param>
    /// <param name="expectedContainsText">Ожидаемый текст лога(частично).</param>
    /// <param name="expectedExactly">Ожидаемое количество записей в логи.</param>
    /// <typeparam name="T">Тип объекта логирования.</typeparam>
    protected void LogVerify<T>(
        Mock<ILogger<T>> loggerMock, 
        LogLevel expectedLogLevel,
        string expectedContainsText,
        Times expectedExactly)
    {
        loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == expectedLogLevel),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedContainsText)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            expectedExactly);
    }
}