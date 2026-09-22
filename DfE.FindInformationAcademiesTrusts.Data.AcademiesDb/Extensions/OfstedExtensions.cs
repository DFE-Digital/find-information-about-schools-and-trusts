namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;

public static class OfstedExtensions
{
    public static OfstedRatingScore ConvertOverallEffectivenessToOfstedRatingScore(this string? rating)
    {
        // Check if it is 'Not judged' all other gradings are int based
        if (rating?.ToLower().Equals("not judged") ?? false)
        {
            return OfstedRatingScore.SingleHeadlineGradeNotAvailable;
        }

        return ConvertNullableStringToOfstedRatingScore(rating);
    }

    public static OfstedRatingScore ToOfstedRatingScore(this string? rating)
    {
        if (rating is null)
            return OfstedRatingScore.NotInspected;
        
        var ofstedRating = int.Parse(rating);
        
        if (Enum.IsDefined(typeof(OfstedRatingScore), ofstedRating))
            return (OfstedRatingScore)ofstedRating;

        return OfstedRatingScore.Unknown;
    }

    public static OfstedRatingScore ConvertNullableStringToOfstedRatingScore(this string? rating)
    {
        if (rating is null)
            return OfstedRatingScore.NotInspected;

        // Attempt to parse the string as an integer then cast to enum
        if (int.TryParse(rating, out var intRating) && Enum.IsDefined(typeof(OfstedRatingScore), intRating))
        {
            return (OfstedRatingScore)intRating;
        }

        return OfstedRatingScore.Unknown;
    }

    public static CategoriesOfConcern ToCategoriesOfConcern(this string? input)
    {
        return input switch
        {
            null => CategoriesOfConcern.NotInspected,
            "" => CategoriesOfConcern.NoConcerns,
            "SM" => CategoriesOfConcern.SpecialMeasures,
            "SWK" => CategoriesOfConcern.SeriousWeakness,
            "NTI" => CategoriesOfConcern.NoticeToImprove,
            _ => CategoriesOfConcern.Unknown
        };
    }

    public static SafeguardingScore ToSafeguardingScore(this string? input)
    {
        return input switch
        {
            null or "NULL" => SafeguardingScore.NotInspected,
            "Yes" => SafeguardingScore.Yes,
            "No" => SafeguardingScore.No,
            "9" => SafeguardingScore.NotRecorded,
            _ => SafeguardingScore.Unknown
        };
    }
}
