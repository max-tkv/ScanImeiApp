using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Services.ModificationImage;
using Xunit;

namespace ScanImeiApp.UnitTests.Services.ModificationImage;

/// <summary>
/// Тесты для класса <see cref="ModifierImageFactory" />.
/// </summary>
public class ModifierImageFactoryTest : BaseUnitTests
{
    [Fact(DisplayName = "Проверяет, что метод Create возвращает правильный объект IModifierImage, " +
                        "когда запрашивается зарегистрированный тип изменения изображения.")]
    public void Create_WhenCalledWithRegisteredType_ReturnsModifierImage()
    {
        // Arrange
        var mockModifierImage = new Mock<IModifierImage>();
        var factory = new ModifierImageFactory();
        factory.AddModifierImage(ModificationImageType.Contrast, () => mockModifierImage.Object);

        // Act
        var result = factory.Create(ModificationImageType.Contrast);

        // Assert
        Assert.Equal(mockModifierImage.Object, result);
    }

    [Fact(DisplayName = "Проверяет, что метод Create бросает исключение InvalidOperationException, " +
                        "когда запрашивается не зарегистрированный тип изменения изображения. " +
                        "Это помогает убедиться, что фабрика корректно обрабатывает запросы на создание обработчиков изображений.")]
    public void Create_WhenCalledWithUnregisteredType_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = new ModifierImageFactory();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => factory.Create(ModificationImageType.Contrast));
        Assert.Equal("Не найден обработчик для изменения изображения. Тип изменения: Contrast", exception.Message);
    }
}