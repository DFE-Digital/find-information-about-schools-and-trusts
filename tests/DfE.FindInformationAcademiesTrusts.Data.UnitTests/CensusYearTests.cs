using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using NSubstitute;

namespace DfE.FindInformationAcademiesTrusts.Data.UnitTests;

public class CensusYearTests
{
    [Theory]
    [InlineData(2025)]
    [InlineData(2026)]
    [InlineData(2027)]
    public void CensusYear_can_be_implicitly_converted_from_int(int year)
    {
        CensusYear censusYear = year;
        
        censusYear.Value.Should().Be(year);
    }
    
    [Theory]
    [InlineData(2025, 3, 15, 2024)]
    [InlineData(2026, 1, 1, 2025)]
    [InlineData(2027, 10, 30, 2026)]
    public void Current_returns_previous_year_when_date_is_on_or_before_october_30th(int year, int month, int day, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Current(dateTimeProvider);

        censusYear.Value.Should().Be(expectedYear);
    }

    [Theory]
    [InlineData(2025, 10, 31, 2025)]
    [InlineData(2026, 12, 1, 2026)]
    [InlineData(2027, 12, 31, 2027)]
    public void Current_returns_current_year_when_date_is_october_31st_or_later(int year, int month, int day, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Current(dateTimeProvider);

        censusYear.Value.Should().Be(expectedYear);
    }

    [Theory]
    [InlineData(2025, 3, 15, 2025)]
    [InlineData(2026, 1, 1, 2026)]
    [InlineData(2027, 10, 30, 2027)]
    public void Next_returns_current_year_when_date_is_on_or_before_october_30th(int year, int month, int day, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Next(dateTimeProvider);

        censusYear.Value.Should().Be(expectedYear);
    }

    [Theory]
    [InlineData(2025, 10, 31, 2026)]
    [InlineData(2026, 12, 1, 2027)]
    [InlineData(2027, 12, 31, 2028)]
    public void Next_returns_next_year_when_date_is_october_31st_or_later(int year, int month, int day, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Next(dateTimeProvider);

        censusYear.Value.Should().Be(expectedYear);
    }
    
    [Theory]
    [InlineData(2025, 3, 15, 1, 2025)]
    [InlineData(2026, 1, 1, 2, 2027)]
    [InlineData(2027, 10, 30, 3, 2029)]
    public void Next_returns_correct_year_for_years_parameter_when_date_is_on_or_before_october_30th(int year, int month, int day, int addYears, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Next(dateTimeProvider, addYears);

        censusYear.Value.Should().Be(expectedYear);
    }

    [Theory]
    [InlineData(2025, 10, 31, 1, 2026)]
    [InlineData(2026, 12, 1, 2, 2028)]
    [InlineData(2027, 12, 31, 3, 2030)]
    public void Next_returns_correct_year_for_years_parameter_when_date_is_october_31st_or_later(int year, int month, int day, int addYears, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Next(dateTimeProvider, addYears);

        censusYear.Value.Should().Be(expectedYear);
    }

    [Theory]
    [InlineData(2025, 3, 15, 2023)]
    [InlineData(2026, 1, 1, 2024)]
    [InlineData(2027, 10, 30, 2025)]
    public void Previous_returns_year_before_last_when_date_is_on_or_before_october_30th(int year, int month, int day, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Previous(dateTimeProvider);

        censusYear.Value.Should().Be(expectedYear);
    }

    [Theory]
    [InlineData(2025, 10, 31, 2024)]
    [InlineData(2026, 12, 1, 2025)]
    [InlineData(2027, 12, 31, 2026)]
    public void Previous_returns_previous_year_when_date_is_october_31st_or_later(int year, int month, int day, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Previous(dateTimeProvider);

        censusYear.Value.Should().Be(expectedYear);
    }
    
    [Theory]
    [InlineData(2025, 3, 15, 1, 2023)]
    [InlineData(2026, 1, 1, 2, 2023)]
    [InlineData(2027, 10, 30, 3, 2023)]
    public void Previous_returns_correct_year_for_years_parameter_when_date_is_on_or_before_october_30th(int year, int month, int day, int subtractYears, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Previous(dateTimeProvider, subtractYears);

        censusYear.Value.Should().Be(expectedYear);
    }

    [Theory]
    [InlineData(2025, 10, 31, 1, 2024)]
    [InlineData(2026, 12, 1, 2, 2024)]
    [InlineData(2027, 12, 31, 3, 2024)]
    public void Previous_returns_correct_year_for_years_parameter_when_date_is_october_31st_or_later(int year, int month, int day, int subtractYears, int expectedYear)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var censusYear = CensusYear.Previous(dateTimeProvider, subtractYears);

        censusYear.Value.Should().Be(expectedYear);
    }
    
    [Theory]
    [InlineData(2025, "2025")]
    [InlineData(2026, "2026")]
    [InlineData(2027, "2027")]
    public void ToString_returns_year_as_string(int year, string expected)
    {
        var censusYear = new CensusYear(year);
        
        censusYear.ToString().Should().Be(expected);
    }
}