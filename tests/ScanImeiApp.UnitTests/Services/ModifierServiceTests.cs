using AutoFixture;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Services;
using Xunit;

namespace ScanImeiApp.UnitTests.Services;

/// <summary>
/// Тесты для класса <see cref="ModifierService" />.
/// </summary>
public class ModifierServiceTests : BaseUnitTests
{
    private readonly Mock<IModificationImageFactory> _modificationImageFactoryMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly ModifierService _modifierService;

    public ModifierServiceTests()
    {
        _modificationImageFactoryMock = new Mock<IModificationImageFactory>();
        _imageServiceMock = new Mock<IImageService>();

        _modifierService = new ModifierService(
            _modificationImageFactoryMock.Object,
            _imageServiceMock.Object);
    }

    [Theory(DisplayName = "Проверяет сколько раз для каждого типа модификации выполняется модификация.")]
    [MemberData(nameof(GetModificationImageTypeMemberData))]
    public async Task ApplyModifyImageAsync_CallsModifyImageAsync_ForEachModificationType(
        List<ModificationImageType> modificationImageTypes)
    {
        // Arrange
        _modificationImageFactoryMock
            .Setup(factory => factory.Create(It.IsAny<ModificationImageType>()))
            .Returns(new Mock<IModifierImage>().Object);

        // Act
        await _modifierService.ApplyModifyImageAsync(
            new MemoryStream(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            modificationImageTypes,
            CancellationToken.None);

        // Assert
        _modificationImageFactoryMock
            .Verify(factory => factory.Create(
                It.IsAny<ModificationImageType>()), 
                Times.Exactly(modificationImageTypes.Count(x => x != ModificationImageType.Original)));
    }

    [Theory(DisplayName = "Проверяет сколько раз за модификацию выполняется сохранение изображения.")]
    [MemberData(nameof(GetModificationImageTypeMemberData))]
    public async Task ApplyModifyImageAsync_CallsSaveImageAsync_ForEachModificationType(
        List<ModificationImageType> modificationImageTypes)
    {
        // Arrange
        _modificationImageFactoryMock
            .Setup(factory => factory.Create(It.IsAny<ModificationImageType>()))
            .Returns(new Mock<IModifierImage>().Object);

        // Act
        await _modifierService.ApplyModifyImageAsync(
            new MemoryStream(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            modificationImageTypes,
            CancellationToken.None);

        // Assert
        _imageServiceMock
            .Verify(service => service.SaveImageAsync(
                It.IsAny<MemoryStream>(), 
                It.IsAny<string>(), CancellationToken.None), 
                Times.Once);
    }

    #region MemberDatas

    /// <summary>
    /// Датасет список ModificationImageType для методов тестирования.
    /// </summary>
    /// <returns>Списки ModificationImageType.</returns>
    public static IEnumerable<object[]> GetModificationImageTypeMemberData()
    {
        yield return new object[] { new List<ModificationImageType>
        {
            ModificationImageType.Contrast
        }};
        yield return new object[] { new List<ModificationImageType>
        {
            ModificationImageType.Contrast,
            ModificationImageType.Binaryzation
        }};
        yield return new object[] { new List<ModificationImageType>
        {
            ModificationImageType.Contrast,
            ModificationImageType.Binaryzation,
            ModificationImageType.Resize
        }};
        yield return new object[] { new List<ModificationImageType>
        {
            ModificationImageType.Contrast,
            ModificationImageType.Binaryzation,
            ModificationImageType.Resize,
            ModificationImageType.Sharpness
        }};
        yield return new object[] { new List<ModificationImageType>
        {
            ModificationImageType.Contrast,
            ModificationImageType.Binaryzation,
            ModificationImageType.Resize,
            ModificationImageType.Sharpness,
            ModificationImageType.GaussianBlur
        }};
        yield return new object[] { new List<ModificationImageType>
        {
            ModificationImageType.Contrast,
            ModificationImageType.Binaryzation,
            ModificationImageType.Resize,
            ModificationImageType.Sharpness,
            ModificationImageType.GaussianBlur,
            ModificationImageType.Original
        }};
    }

    #endregion
}