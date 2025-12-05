using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class AgeRangeExtensionsTests
{
    [Theory]
    [InlineData(0, 99, "0 to 99")]
    [InlineData(2, 9, "2 to 9")]
    [InlineData(11, 18, "11 to 18")]
    [InlineData(null, 18, "Not Available")]
    [InlineData(18, null, "Not Available")]
    [InlineData(null, null, "Not Available")]
    public void ToFullDisplayString_should_use_min_and_max(int? min, int? max, string expected)
    {
        new AgeRange(min, max).ToFullDisplayString().Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 99, "0-99")]
    [InlineData(2, 9, "2-9")]
    [InlineData(11, 18, "11-18")]
    [InlineData(null, 18, "Not Available")]
    [InlineData(18, null, "Not Available")]
    [InlineData(null, null, "Not Available")]
    public void ToTabularDisplayString_should_use_min_and_max(int? min, int? max, string expected)
    {
        new AgeRange(min, max).ToTabularDisplayString().Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 99, "0099")]
    [InlineData(2, 9, "0209")]
    [InlineData(11, 18, "1118")]
    public void ToDataSortValue_should_use_min_and_max(int min, int max, string expected)
    {
        new AgeRange(min, max).ToDataSortValue().Should().Be(expected);
    }

    [Fact]
    public void ToDataSortValue_should_return_negative_1_when_null()
    {
        AgeRange? nullAge = null;
        nullAge.ToDataSortValue().Should().Be("-1");
    }
}
