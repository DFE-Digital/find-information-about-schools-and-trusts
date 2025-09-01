using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Data.UnitTests;

public class StatisticExtensionsTests
{
    [Theory]
    [InlineData(StatisticKind.Suppressed)]
    [InlineData(StatisticKind.NotPublished)]
    [InlineData(StatisticKind.NotApplicable)]
    [InlineData(StatisticKind.NotAvailable)]
    [InlineData(StatisticKind.NotYetSubmitted)]
    public void Compute_for_one_statistic_propagates_kind_when_not_WithValue(StatisticKind kind)
    {
        var statistic = Statistic<int>.FromKind(kind);
        var result = statistic.Compute(s => 2.0 * s);
        result.Kind.Should().Be(kind);
    }

    [Theory]
    [InlineData(1, 2.0)]
    [InlineData(2, 4.0)]
    [InlineData(3, 6.0)]
    public void Compute_for_one_statistic_returns_computed_value_when_WithValue(int value, double expected)
    {
        var statistic = new Statistic<int>.WithValue(value);
        var result = statistic.Compute(s => 2.0 * s);
        result.Kind.Should().Be(StatisticKind.WithValue);
        result.As<Statistic<double>.WithValue>().Value.Should().Be(expected);
    }
    
    [Theory]
    [InlineData(StatisticKind.Suppressed)]
    [InlineData(StatisticKind.NotPublished)]
    [InlineData(StatisticKind.NotApplicable)]
    [InlineData(StatisticKind.NotAvailable)]
    [InlineData(StatisticKind.NotYetSubmitted)]
    public void Compute_for_two_statistics_propagates_kind_of_the_first_when_not_WithValue(StatisticKind kind)
    {
        var statistic1 = Statistic<int>.FromKind(kind);
        var statistic2 = new Statistic<int>.WithValue(1);
        var result = statistic1.Compute(statistic2, (s1, s2) => 2.0 * s1 + s2);
        result.Kind.Should().Be(kind);
    }
    
    [Theory]
    [InlineData(StatisticKind.Suppressed)]
    [InlineData(StatisticKind.NotPublished)]
    [InlineData(StatisticKind.NotApplicable)]
    [InlineData(StatisticKind.NotAvailable)]
    [InlineData(StatisticKind.NotYetSubmitted)]
    public void Compute_for_two_statistics_propagates_kind_of_the_second_when_not_WithValue(StatisticKind kind)
    {
        var statistic1 = new Statistic<int>.WithValue(1);
        var statistic2 = Statistic<int>.FromKind(kind);
        var result = statistic1.Compute(statistic2, (s1, s2) => 2.0 * s1 + s2);
        result.Kind.Should().Be(kind);
    }

    [Fact]
    public void Compute_for_two_statistics_when_both_not_WithValue_propagates_kind_of_the_first_statistic()
    {
        var statistic1 = Statistic<int>.FromKind(StatisticKind.NotPublished);
        var statistic2 = Statistic<int>.FromKind(StatisticKind.Suppressed);
        var result = statistic1.Compute(statistic2, (s1, s2) => 2.0 * s1 + s2);
        result.Kind.Should().Be(StatisticKind.NotPublished);
    }
    
    [Theory]
    [InlineData(1, 2, 4.0)]
    [InlineData(2, 4, 8.0)]
    [InlineData(3, 6, 12.0)]
    public void Compute_for_two_statistics_returns_computed_value_when_both_WithValue(int value1, int value2, double expected)
    {
        var statistic1 = new Statistic<int>.WithValue(value1);
        var statistic2 = new Statistic<int>.WithValue(value2);
        var result = statistic1.Compute(statistic2, (s1, s2) => 2.0 * s1 + s2);
        result.Kind.Should().Be(StatisticKind.WithValue);
        result.As<Statistic<double>.WithValue>().Value.Should().Be(expected);
    }
}
