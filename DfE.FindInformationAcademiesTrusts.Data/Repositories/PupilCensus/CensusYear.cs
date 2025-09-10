namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

public record CensusYear(int Value)
{
    private const int SpringCensusAvailabilityDeadlineMonth = 10;
    private const int SpringCensusAvailabilityDeadlineDay = 31;
    
    public static implicit operator CensusYear(int year) => new(year);

    public static CensusYear Current(IDateTimeProvider provider)
    {
        var today = provider.Today;
        var currentYearsSpringCensusAvailabilityDeadline = new DateTime(
            today.Year,
            SpringCensusAvailabilityDeadlineMonth,
            SpringCensusAvailabilityDeadlineDay,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        var censusYear = today.Year;
        if (today < currentYearsSpringCensusAvailabilityDeadline)
        {
            censusYear--;
        }

        return censusYear;
    }

    public static CensusYear Next(IDateTimeProvider provider, int years = 1) => Current(provider).Value + years;
    public static CensusYear Previous(IDateTimeProvider provider, int years = 1) => Current(provider).Value - years;
    
    public override string ToString() => Value.ToString();
}
