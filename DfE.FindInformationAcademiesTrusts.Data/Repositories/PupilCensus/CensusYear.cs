namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

public record CensusYear(int Value)
{
    private static readonly Dictionary<Census, DayMonth> AvailabilityDeadlines = new()
    {
        [Census.Spring] = new DayMonth(31, 10),
        [Census.Autumn] = new DayMonth(30, 4)
    };

    public static implicit operator CensusYear(int year) => new(year);

    public static CensusYear Current(IDateTimeProvider provider, Census census)
    {
        var today = provider.Today;
        var currentYearsSpringCensusAvailabilityDeadline = AvailabilityDeadlines[census].ToDateTime(today.Year);

        var censusYear = today.Year;
        if (today < currentYearsSpringCensusAvailabilityDeadline)
        {
            censusYear--;
        }

        return censusYear;
    }

    public static CensusYear Next(IDateTimeProvider provider, Census census, int years = 1) =>
        Current(provider, census).Value + years;

    public static CensusYear Previous(IDateTimeProvider provider, Census census, int years = 1) =>
        Current(provider, census).Value - years;

    public override string ToString() => Value.ToString();
}
