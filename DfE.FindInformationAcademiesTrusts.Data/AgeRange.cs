namespace DfE.FindInformationAcademiesTrusts.Data;

public record AgeRange(int? Minimum, int? Maximum)
{
    public AgeRange(string? Minimum, string? Maximum)
        : this(
            string.IsNullOrWhiteSpace(Minimum) ? null : int.Parse(Minimum),
            string.IsNullOrWhiteSpace(Maximum) ? null : int.Parse(Maximum)
        )
    {
    }
}
