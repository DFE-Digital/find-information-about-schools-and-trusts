namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

public record DayMonth(int Day, int Month)
{
    public DateTime ToDateTime(int year)
    {
        return new DateTime(year, Month, Day, 0, 0, 0, DateTimeKind.Utc);
    }
}
