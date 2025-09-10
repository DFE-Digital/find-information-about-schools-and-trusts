using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class StatisticDisplayExtensionsTests
{
    [Theory]
    [InlineData(StatisticKind.Suppressed, "suppressed")]
    [InlineData(StatisticKind.NotPublished, "not-published")]
    [InlineData(StatisticKind.NotApplicable, "not-applicable")]
    [InlineData(StatisticKind.NotAvailable, "not-available")]
    [InlineData(StatisticKind.NotYetSubmitted, "not-yet-submitted")]
    public void SortValue_returns_expected_value_for_statistics_without_value(StatisticKind kind, string expected)
    {
        var statistic = Statistic<int>.FromKind(kind);
        
        statistic.SortValue().Should().Be(expected);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(3.4, "3.4")]
    [InlineData(true, "True")]
    [InlineData("Text", "Text")]
    public void SortValue_returns_expected_value_for_statistics_with_value(object value, string expected)
    {
        var statistic = new Statistic<object>.WithValue(value);
        
        statistic.SortValue().Should().Be(expected);
    }

    [Theory]
    [InlineData(StatisticKind.Suppressed, "Suppressed")]
    [InlineData(StatisticKind.NotPublished, "Not published")]
    [InlineData(StatisticKind.NotApplicable, "Not applicable")]
    [InlineData(StatisticKind.NotAvailable, "Not available")]
    [InlineData(StatisticKind.NotYetSubmitted, "Not yet submitted")]
    public void DisplayValue_returns_expected_value_for_statistics_without_value(StatisticKind kind, string expected)
    {
        var statistic = Statistic<int>.FromKind(kind);
        
        statistic.DisplayValue().Should().Be(expected);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(3.4, "3.4")]
    [InlineData(true, "True")]
    [InlineData("Text", "Text")]
    public void DisplayValue_returns_expected_value_for_statistics_with_value(object value, string expected)
    {
        var statistic = new Statistic<object>.WithValue(value);
        
        statistic.DisplayValue().Should().Be(expected);
    }

    [Theory]
    [InlineData(StatisticKind.Suppressed,  "Suppressed")]
    [InlineData(StatisticKind.NotPublished, "Not published")]
    [InlineData(StatisticKind.NotApplicable, "Not applicable")]
    [InlineData(StatisticKind.NotAvailable, "Not available")]
    [InlineData(StatisticKind.NotYetSubmitted, "Not yet submitted")]
    public void DisplayValueWithPercentage_returns_expected_value_when_first_statistic_does_not_have_a_value(StatisticKind kind, string expected)
    {
        var statistic1 = Statistic<int>.FromKind(kind);
        var statistic2 = new Statistic<decimal>.WithValue(2.0m);
        
        statistic1.DisplayValueWithPercentage(statistic2).Should().Be(expected);
    }

    [Theory]
    [InlineData(StatisticKind.Suppressed,  "Suppressed")]
    [InlineData(StatisticKind.NotPublished, "Not published")]
    [InlineData(StatisticKind.NotApplicable, "Not applicable")]
    [InlineData(StatisticKind.NotAvailable, "Not available")]
    [InlineData(StatisticKind.NotYetSubmitted, "Not yet submitted")]
    public void DisplayValueWithPercentage_returns_expected_value_when_second_statistic_does_not_have_a_value(StatisticKind kind, string expected)
    {
        var statistic1 = new Statistic<int>.WithValue(2);
        var statistic2 = Statistic<decimal>.FromKind(kind);
        
        statistic1.DisplayValueWithPercentage(statistic2).Should().Be(expected);
    }

    [Theory]
    [InlineData(2, "1.0", "2 (1.0%)")]
    [InlineData(566, "17.4", "566 (17.4%)")]
    [InlineData(94, "2.94", "94 (2.94%)")]
    public void DisplayValueWithPercentage_returns_expected_value_when_both_statistics_have_a_value(int value1, string value2, string expected)
    {
        var statistic1 = new Statistic<int>.WithValue(value1);
        var statistic2 = new Statistic<decimal>.WithValue(decimal.Parse(value2));
        
        statistic1.DisplayValueWithPercentage(statistic2).Should().Be(expected);
    }
}
