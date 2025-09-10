using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Data.UnitTests;

public class AnnualStatisticsTests
{
    [Theory]
    [InlineData(2025, 1)]
    [InlineData(2026, 2)]
    [InlineData(2027, 3)]
    [InlineData(2028, 4)]
    [InlineData(2029, 5)]
    [InlineData(2030, 6)]
    public void AnnualStatistics_can_use_int_as_key(int year, object expected)
    {
        var annualStatistics = new AnnualStatistics<object>
        {
            { year, expected }
        };

        annualStatistics[year].Should().Be(expected);
    }
    
    [Theory]
    [InlineData(2025, 1)]
    [InlineData(2026, 2)]
    [InlineData(2027, 3)]
    [InlineData(2028, 4)]
    [InlineData(2029, 5)]
    [InlineData(2030, 6)]
    public void AnnualStatistics_can_use_CensusYear_as_key(int year, object expected)
    {
        var censusYear = new CensusYear(year);
        var annualStatistics = new AnnualStatistics<object>
        {
            { censusYear, expected }
        };

        annualStatistics[censusYear].Should().Be(expected);
    }
}
