using AutoFixture;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;
using ScanImeiApp.Services.ModificationImage;
using Xunit;

namespace ScanImeiApp.UnitTests.Services.ModificationImage;

/// <summary>
/// Тесты для класса <see cref="ModifierImageBinaryzation" />.
/// </summary>
public class ModifierImageBinaryzationTest : BaseUnitTests
{
    [Fact(DisplayName = "Проверяет, что метод ModifyImageAsync вызывает метод BinaryzationAsync с правильными параметрами.")]
    public async Task ModifyImageAsync_BinaryzationAsync_WithCorrectParameters()
    {
        // Arrange
        var mockImageService = new Mock<IImageService>();
        var appOptions = _fixture.Create<AppOptions>();
        var modifier = new ModifierImageBinaryzation(appOptions, mockImageService.Object);
        var memoryStreamImage = new MemoryStream();
        var imageName = "testImage.jpg";
        var cancellationToken = CancellationToken.None;
        
        mockImageService.Setup(service => service.BinaryzationAsync(
                memoryStreamImage, 
                imageName, 
                appOptions.ImageOptions!.Binaryzation, 
                cancellationToken))
            .ReturnsAsync(memoryStreamImage)
            .Verifiable("BinaryzationAsync was not called with the correct parameters.");

        // Act
        await modifier.ModifyImageAsync(memoryStreamImage, imageName, cancellationToken);

        // Assert
        mockImageService.Verify();
    }
}