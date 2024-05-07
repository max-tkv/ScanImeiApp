using System.Collections.ObjectModel;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Models;
using ScanImeiApp.Options;
using ScanImeiApp.Services;
using Xunit;

namespace ScanImeiApp.UnitTests.Services;

/// <summary>
/// Тесты для класса <see cref="ScannerImeiService" />.
/// </summary>
public class ScannerImeiServiceTests : BaseUnitTests
{
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly Mock<ILogger<ScannerImeiService>> _loggerMock;
    private readonly Mock<IImeiService> _imeiServiceMock;
    private readonly ScannerImeiService _scannerImeiService;
    private readonly AppOptions _appOptions;
    private readonly Mock<IModifierService> _modifierServiceMock;
    private readonly Mock<IRecognizerTextService> _recognizerTextServiceMock;

    public ScannerImeiServiceTests()
    {
        _imageServiceMock = new Mock<IImageService>();
        _loggerMock = new Mock<ILogger<ScannerImeiService>>();
        _imeiServiceMock = new Mock<IImeiService>();
        _appOptions = new AppOptions();
        _modifierServiceMock = new Mock<IModifierService>();
        _recognizerTextServiceMock = new Mock<IRecognizerTextService>();
        
        _scannerImeiService = new ScannerImeiService(
            _imageServiceMock.Object, 
            _appOptions,
            _loggerMock.Object,
            _imeiServiceMock.Object,
            _recognizerTextServiceMock.Object,
            _modifierServiceMock.Object);
    }

    [Fact(DisplayName = "Проверяет, что перед распознаванием сохраняется изображение, " +
                        "удаляется альфа-канал, применяются все типы модификации изображения, получает текст с изображения, " +
                        "получает и возвращает список IMEI.")]
    public async Task GetImeiTextFromImageAsync_ReturnsListOfImei()
    {
        // Arrange
        var originalImage = new MemoryStream();
        var imageName = "testImage.jpg";
        var cancellationToken = CancellationToken.None;
        var expectedImeiList = new List<string> { "123456789012345", "987654321098765" };
        _imageServiceMock
            .Setup(x => x.SaveImageAsync(
                It.IsAny<MemoryStream>(), 
                It.IsAny<string>(), 
                cancellationToken))
            .Returns(Task.CompletedTask);
        _imageServiceMock
            .Setup(x => x.RemoveAlphaChannelAsync(
                It.IsAny<MemoryStream>(), 
                cancellationToken))
            .ReturnsAsync(new MemoryStream());
        _imeiServiceMock
            .Setup(x => x.FindImeiToRecognizeResultsAsync(
                It.IsAny<List<RecognizeResult>>(), 
                cancellationToken))
            .ReturnsAsync(expectedImeiList);
        _appOptions.Modifications = CreateOptionsRecognizers();

        // Act
        var result = await _scannerImeiService.GetImeiTextFromImageAsync(
            originalImage, 
            imageName, 
            cancellationToken);

        // Assert
        Assert.Equal(expectedImeiList, result);
        _imageServiceMock
            .Verify(x => x.SaveImageAsync(
                It.IsAny<MemoryStream>(), 
                It.IsAny<string>(), 
                cancellationToken), 
                Times.Once);
        _imageServiceMock
            .Verify(x => x.RemoveAlphaChannelAsync(
                It.IsAny<MemoryStream>(), 
                cancellationToken), 
                Times.Once);
        _imeiServiceMock
            .Verify(x => x.FindImeiToRecognizeResultsAsync(
                It.IsAny<List<RecognizeResult>>(), 
                cancellationToken), 
                Times.Once);
        _modifierServiceMock
            .Verify(x => x.ApplyModifyImageAsync(
                    It.IsAny<MemoryStream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyCollection<ModificationImageType>>(),
                    cancellationToken), 
                Times.Exactly(2));
        _recognizerTextServiceMock
            .Verify(x => x.RecognizeText(
                    It.IsAny<MemoryStream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), 
                Times.Exactly(2));
    }
    
    [Fact(DisplayName = "Проверяет, что если при распознавании одного из типа (из опций приложения) произошла ошибка," +
                        "то процесс продолжится и в итоге получит и возвратит список IMEI.")]
    public async Task GetImeiTextFromImageAsync_ApplyModifyImageAsyncException_ContinuedAndReturnsListOfImei()
    {
        // Arrange
        var originalImage = new MemoryStream();
        var imageName = "testImage.jpg";
        var cancellationToken = CancellationToken.None;
        var expectedImeiList = new List<string> { "123456789012345", "987654321098765" };
        _imageServiceMock
            .Setup(x => x.SaveImageAsync(
                It.IsAny<MemoryStream>(), 
                It.IsAny<string>(), 
                cancellationToken))
            .Returns(Task.CompletedTask);
        _imageServiceMock
            .Setup(x => x.RemoveAlphaChannelAsync(
                It.IsAny<MemoryStream>(), 
                cancellationToken))
            .ReturnsAsync(new MemoryStream());
        _imeiServiceMock
            .Setup(x => x.FindImeiToRecognizeResultsAsync(
                It.IsAny<List<RecognizeResult>>(), 
                cancellationToken))
            .ReturnsAsync(expectedImeiList);
        _modifierServiceMock
            .Setup(x => x.ApplyModifyImageAsync(
                    It.IsAny<MemoryStream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyCollection<ModificationImageType>>(),
                    cancellationToken))
            .Throws<Exception>();
        _appOptions.Modifications = CreateOptionsRecognizers();

        // Act
        var result = await _scannerImeiService.GetImeiTextFromImageAsync(
            originalImage, 
            imageName, 
            cancellationToken);

        // Assert
        Assert.Equal(expectedImeiList, result);
        _imageServiceMock
            .Verify(x => x.SaveImageAsync(
                It.IsAny<MemoryStream>(), 
                It.IsAny<string>(), 
                cancellationToken), 
                Times.Once);
        _imageServiceMock
            .Verify(x => x.RemoveAlphaChannelAsync(
                It.IsAny<MemoryStream>(), 
                cancellationToken), 
                Times.Once);
        _imeiServiceMock
            .Verify(x => x.FindImeiToRecognizeResultsAsync(
                It.IsAny<List<RecognizeResult>>(), 
                cancellationToken), 
                Times.Once);
        _modifierServiceMock
            .Verify(x => x.ApplyModifyImageAsync(
                    It.IsAny<MemoryStream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyCollection<ModificationImageType>>(),
                    cancellationToken), 
                Times.Exactly(2));
        _recognizerTextServiceMock
            .Verify(x => x.RecognizeText(
                    It.IsAny<MemoryStream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
            LogVerify(_loggerMock, LogLevel.Warning, Times.Exactly(2));
    }

    #region Приватные методы

    /// <summary>
    /// Создать опции распознавания.
    /// </summary>
    /// <returns>Опции распознавания.</returns>
    private Collection<ModificationOptions> CreateOptionsRecognizers()
    {
        string[] names = Enum.GetNames(typeof(ModificationImageType));
        return new Collection<ModificationOptions>()
        {
            new()
            {
                Name = _fixture.Create<string>(),
                ModificationTypes = names
                    .Select(name => (ModificationImageType)Enum.Parse(typeof(ModificationImageType), name))
                    .ToList()
            },
            new()
            {
                Name = _fixture.Create<string>(),
                ModificationTypes = names
                    .Select(name => (ModificationImageType)Enum.Parse(typeof(ModificationImageType), name))
                    .ToList()
            }
        };
    }

    #endregion
}