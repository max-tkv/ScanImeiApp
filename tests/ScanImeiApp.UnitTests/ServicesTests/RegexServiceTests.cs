using ScanImeiApp.Services;
using Xunit;

namespace ScanImeiApp.UnitTests.ServicesTests;

public class RegexServiceTests
{
    private string[] patterns =
    {
        "IMEI(\\d{15})(\\d{15})?", // IMEI000000000000000 or IMEI000000000000000111111111111111
        "IMEI(\\d{15}):?(\\d{15})?", // IMEI000000000000000 or IMEI000000000000000:111111111111111
        "L121](\\d{15}):?(\\d{15})?", // L121]000000000000000 or L121]000000000000000111111111111111
        "IME\\|(\\d{15}):?(\\d{15})?", // IME|000000000000000 or IME|000000000000000111111111111111
        "IME1(\\d{15}):?(\\d{15})?", // IME1000000000000000 or IME1000000000000000111111111111111
        "ME\\|(\\d{15}):?(\\d{15})?", // ME|000000000000000 or ME|000000000000000111111111111111
        "ME\\|l(\\d{15}):?(\\d{15})?", // ME|l000000000000000 or ME|000000000000000111111111111111
        "MEI(\\d{15}):?(\\d{15})?", // MEI000000000000000 or MEI000000000000000111111111111111
        "IMEl1(\\d{15}):?(\\d{15})?", // IMEl000000000000000 or IMEl000000000000000111111111111111
        "MEl(\\d{15}):?(\\d{15})?", // MEl000000000000000 or MEl000000000000000111111111111111
        "ME1(\\d{15}):?(\\d{15})?", // ME1000000000000000 or ME1000000000000000111111111111111
        "\\|ME\\|(\\d{15}):?(\\d{15})?", // |ME|000000000000000 or |ME|000000000000000111111111111111
        "1ME1(\\d{15}):?(\\d{15})?", // 1ME1000000000000000 or 1ME1000000000000000111111111111111
        "lMEl(\\d{15}):?(\\d{15})?", // lMEl000000000000000 or lMEl000000000000000111111111111111
        "IMEINO(\\d{15}):?(\\d{15})?", // IMEINO000000000000000 or IMEINO000000000000000111111111111111
        "MEINO(\\d{15}):?(\\d{15})?", // MEINO000000000000000 or MEINO000000000000000111111111111111
        "IMEI\\d*:(\\d{15}):?(\\d{15})?", // IMEI1:000000000000000:11111111111111
        "L121]\\d*:(\\d{15}):?(\\d{15})?", // L121]1:000000000000000:11111111111111
        "IME\\|\\d*:(\\d{15}):?(\\d{15})?", // IME|1:000000000000000:11111111111111
        "ME\\|\\d*:(\\d{15}):?(\\d{15})?", // ME|1:000000000000000:11111111111111
        "ME\\|l:(\\d{15}):?(\\d{15})?", // ME|1:000000000000000:11111111111111
        "IME1\\d*:(\\d{15}):?(\\d{15})?", // IME|1:000000000000000:11111111111111
        "IMEl\\d*:(\\d{15}):?(\\d{15})?", // IMEl:000000000000000:11111111111111
        "IME\\|l\\d*:(\\d{15}):?(\\d{15})?", // IME|l:000000000000000:11111111111111
        "MEl\\d*:(\\d{15}):?(\\d{15})?", // ME|l:000000000000000:11111111111111
        "\\|ME\\|\\d*:(\\d{15}):?(\\d{15})?", // |ME|1:000000000000000:11111111111111
        "ME\\|\\d*:(\\d{15}):?(\\d{15})?", // ME|1:000000000000000:11111111111111
        "lMEl\\d*:(\\d{15}):?(\\d{15})?", // lMEl1:000000000000000:11111111111111
        "1ME1\\d*:(\\d{15}):?(\\d{15})?", // 1ME|1:000000000000000:11111111111111
        "lMEl\\d*:(\\d{15}):?(\\d{15})?", // lME|1:000000000000000:11111111111111
        "MEI\\d*:(\\d{15}):?(\\d{15})?", // MEI1:000000000000000:11111111111111
        "MEl\\d*:(\\d{15}):?(\\d{15})?", // MEl1:000000000000000:11111111111111
        "ME1\\d*:(\\d{15}):?(\\d{15})?", // ME|1:000000000000000:11111111111111
        "MEl\\d*:(\\d{15}):?(\\d{15})?", // ME|1:000000000000000:11111111111111
        "1MEI\\([^)]*\\)(\\d{15}):?(\\d{15})?", // 1MEI(any character set)000000000000000:11111111111111
        "1ME1\\([^)]*\\)(\\d{15}):?(\\d{15})?", // 1ME1(any character set)000000000000000:11111111111111
        "lME1\\([^)]*\\)(\\d{15}):?(\\d{15})?", // lME1(any character set)000000000000000:11111111111111
        "lMEl\\([^)]*\\)(\\d{15}):?(\\d{15})?", // lMEl(any character set)000000000000000:11111111111111
        "lMEI\\([^)]*\\)(\\d{15}):?(\\d{15})?", // lMEI(any character set)000000000000000:11111111111111
        "IMEI\\([^)]*\\)(\\d{15}):?(\\d{15})?", // IMEI(any character set)000000000000000:11111111111111
        "{MEI\\([^)]*\\)(\\d{15}):?(\\d{15})?", // {MEI(any character set)000000000000000:11111111111111
        "}MEI\\([^)]*\\)(\\d{15}):?(\\d{15})?", // }IMEI(any character set)000000000000000:11111111111111
        "}ME}\\([^)]*\\)(\\d{15}):?(\\d{15})?", // }IME}(any character set)000000000000000:11111111111111
        "}ME{\\([^)]*\\)(\\d{15}):?(\\d{15})?", // }IME{(any character set)000000000000000:11111111111111
        "IME{\\([^)]*\\)(\\d{15}):?(\\d{15})?", // IIME{(any character set)000000000000000:11111111111111
        "IME}\\([^)]*\\)(\\d{15}):?(\\d{15})?", // }IME{(any character set)
        "{MEI(\\d{15}):?(\\d{15})?", // {MEI000000000000000 or {MEI000000000000000111111111111111
        "}MEI(\\d{15}):?(\\d{15})?", // }IMEI000000000000000 or }IMEI000000000000000111111111111111
        "}ME}(\\d{15}):?(\\d{15})?", // }IME}000000000000000 or }IME}000000000000000111111111111111
        "}ME{(\\d{15}):?(\\d{15})?", // }IME{000000000000000 or }IME{000000000000000111111111111111
        "IME{(\\d{15}):?(\\d{15})?", // IIME{000000000000000 or IIME{000000000000000111111111111111
        "IME}(\\d{15}):?(\\d{15})?" // }IME{000000000000000 or }IME{000000000000000111111111111111
    };
    
    [Theory(DisplayName = "Проверяет поиск IMEI из текста по шаблонам.")]
    [InlineData("IMEI000000000000000", "000000000000000")]
    [InlineData("IMEI000000000000000:111111111111111", "111111111111111,000000000000000")]
    [InlineData("IMEI000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("IME|000000000000000", "000000000000000")]
    [InlineData("IME|000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("IME1000000000000000", "000000000000000")]
    [InlineData("IME1000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("ME1000000000000000", "000000000000000")]
    [InlineData("ME1000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("IMEl000000000000000", "000000000000000")]
    [InlineData("MEl000000000000000", "000000000000000")]
    [InlineData("MEl000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("|ME|000000000000000", "000000000000000")]
    [InlineData("|ME|000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("ME|000000000000000", "000000000000000")]
    [InlineData("ME|000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("1ME1000000000000000", "000000000000000")]
    [InlineData("1ME1000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("lMEl000000000000000", "000000000000000")]
    [InlineData("lMEl000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("IMEINO000000000000000", "000000000000000")]
    [InlineData("IMEINO000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("MEINO000000000000000", "000000000000000")]
    [InlineData("MEINO000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("IMEI1:000000000000000", "000000000000000")]
    [InlineData("IME|1:000000000000000", "000000000000000")]
    [InlineData("ME|1:000000000000000", "000000000000000")]
    [InlineData("IME|l:000000000000000", "000000000000000")]
    [InlineData("ME|l:000000000000000", "000000000000000")]
    [InlineData("|ME|1:000000000000000", "000000000000000")]
    [InlineData("lMEl1:000000000000000", "000000000000000")]
    [InlineData("1MEI(any character set)000000000000000", "000000000000000")]
    [InlineData("1ME1(any character set)000000000000000", "000000000000000")]
    [InlineData("lME1(any character set)000000000000000", "000000000000000")]
    [InlineData("lMEl(any character set)000000000000000", "000000000000000")]
    [InlineData("lMEI(any character set)000000000000000", "000000000000000")]
    [InlineData("IMEI(any character set)000000000000000", "000000000000000")]
    [InlineData("{MEI(any character set)000000000000000", "000000000000000")]
    [InlineData("}IMEI(any character set)000000000000000", "000000000000000")]
    [InlineData("}IME}(any character set)000000000000000", "000000000000000")]
    [InlineData("}IME{(any character set)000000000000000", "000000000000000")]
    [InlineData("IIME{(any character set)000000000000000", "000000000000000")]
    [InlineData("}IME{(any character set)", "")]
    [InlineData("{MEI000000000000000", "000000000000000")]
    [InlineData("}MEI000000000000000", "000000000000000")]
    [InlineData("}ME}000000000000000", "000000000000000")]
    [InlineData("}ME{000000000000000", "000000000000000")]
    [InlineData("IME{000000000000000", "000000000000000")]
    [InlineData("{MEI000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("}MEI000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("}ME}000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("}ME{000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("IME{000000000000000111111111111111", "111111111111111,000000000000000")]
    [InlineData("232MIMEI9112437000577759112437001597790", "911243700159779,911243700057775")]
    [InlineData("IMEI1:867468061537628/78", "867468061537628")]
    [InlineData("IMEI:353081080078473:353081080078481", "353081080078481,353081080078473")]
    public async Task FindAndExtractedByPatterns_ReturnsExpectedValues(string recognizedText, string expected)
    {
        // Arrange
        var regexService = new RegexService();

        // Act
        var result = await regexService.FindAndExtractedByPatternsAsync(
            recognizedText, 
            patterns,
            CancellationToken.None);

        // Assert
        Assert.Equal(expected, string.Join(",", result));
    }

    [Theory(DisplayName = "Проверяет удаление слеша(/) и все после него в одной строке.")]
    [InlineData("example/text", "example")]
    [InlineData("example/text\nexample2/text2\nexample3/text3\n", "example\nexample2\nexample3\n")]
    public void RemoveAfterSlash_RemovesTextAfterSlash(string text, string expected)
    {
        // Arrange
        var regexService = new RegexService();

        // Act
        var result = regexService.RemoveAfterSlash(text);

        // Assert
        Assert.Equal(expected, result);
    }
}