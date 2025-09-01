using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Data.UnitTests;

public class StatisticTests
{
    [Fact]
    public void WithValue_has_kind_of_WithValue()
    {
        var statistic = new Statistic<int>.WithValue(5);
        statistic.Kind.Should().Be(StatisticKind.WithValue);
    }

    [Fact]
    public void Suppressed_has_kind_of_Suppressed()
    {
        Statistic<int>.Suppressed.Kind.Should().Be(StatisticKind.Suppressed);
    }
    
    [Fact]
    public void NotPublished_has_kind_of_NotPublished()
    {
        Statistic<int>.NotPublished.Kind.Should().Be(StatisticKind.NotPublished);
    }
    
    [Fact]
    public void NotApplicable_has_kind_of_NotApplicable()
    {
        Statistic<int>.NotApplicable.Kind.Should().Be(StatisticKind.NotApplicable);
    }
    
    [Fact]
    public void NotAvailable_has_kind_of_NotAvailable()
    {
        Statistic<int>.NotAvailable.Kind.Should().Be(StatisticKind.NotAvailable);
    }
    
    [Fact]
    public void NotYetSubmitted_has_kind_of_NotYetSubmitted()
    {
        Statistic<int>.NotYetSubmitted.Kind.Should().Be(StatisticKind.NotYetSubmitted);
    }

    [Theory]
    [InlineData(StatisticKind.Suppressed)]
    [InlineData(StatisticKind.NotPublished)]
    [InlineData(StatisticKind.NotApplicable)]
    [InlineData(StatisticKind.NotAvailable)]
    [InlineData(StatisticKind.NotYetSubmitted)]
    public void FromKind_returns_statistic_of_correct_kind(StatisticKind kind)
    {
        Statistic<int>.FromKind(kind).Kind.Should().Be(kind);
    }

    [Fact]
    public void FromKind_throws_ArgumentException_when_kind_is_WithValue()
    {
        Action action = () => Statistic<int>.FromKind(StatisticKind.WithValue);
        action.Should().Throw<ArgumentException>().WithMessage(
            "WithValue can't be constructed without a value. Use the WithValue constructor instead of this method."
        );
    }

    [Fact]
    public void FromKind_throws_ArgumentException_when_kind_is_unknown()
    {
        Action action = () => Statistic<int>.FromKind((StatisticKind) 999);
        action.Should().Throw<ArgumentException>().WithMessage("Unknown statistic kind '999'.");
    }

    [Theory]
    [InlineData(StatisticKind.Suppressed)]
    [InlineData(StatisticKind.NotPublished)]
    [InlineData(StatisticKind.NotApplicable)]
    [InlineData(StatisticKind.NotAvailable)]
    [InlineData(StatisticKind.NotYetSubmitted)]
    public void TryGetValue_returns_false_when_kind_is_not_WithValue(StatisticKind kind)
    {
        var statistic = Statistic<int>.FromKind(kind);
        
        statistic.TryGetValue(out _).Should().BeFalse();
    }
    
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public void TryGetValue_returns_true_and_returns_result_in_out_parameter_when_kind_is_WithValue(int value)
    {
        var statistic = new Statistic<int>.WithValue(value);
        statistic.TryGetValue(out var result).Should().BeTrue();
        result.Should().Be(value);
    }
}
