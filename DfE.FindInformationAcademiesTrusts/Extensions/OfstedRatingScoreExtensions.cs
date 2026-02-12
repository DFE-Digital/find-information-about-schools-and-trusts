using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Extensions;

public static class OfstedRatingScoreExtensions
{
    public static int ToDataSortValue(this OfstedRatingScore rating)
    {
        return rating switch
        {
            OfstedRatingScore.Outstanding => 1,
            OfstedRatingScore.Good => 2,
            OfstedRatingScore.RequiresImprovement => 3,
            OfstedRatingScore.Inadequate => 4,
            OfstedRatingScore.InsufficientEvidence => 5,
            OfstedRatingScore.SingleHeadlineGradeNotAvailable => 6,
            OfstedRatingScore.DoesNotApply => 7,
            OfstedRatingScore.NotInspected => 8,
            _ => -1
        };
    }

    public static string ToDisplayString(this OfstedRatingScore rating, bool isCurrentRating)
    {
        return (rating, isCurrentRating) switch
        {
            (OfstedRatingScore.Outstanding, _) => "Outstanding",
            (OfstedRatingScore.Good, _) => "Good",
            (OfstedRatingScore.RequiresImprovement, _) => "Requires improvement",
            (OfstedRatingScore.Inadequate, _) => "Inadequate",
            (OfstedRatingScore.InsufficientEvidence, _) => "Insufficient evidence",
            (OfstedRatingScore.SingleHeadlineGradeNotAvailable, _) => "Not available",
            (OfstedRatingScore.DoesNotApply, _) => "Does not apply",
            (OfstedRatingScore.NotInspected, true) => "Not inspected",
            (OfstedRatingScore.NotInspected, false) => "Not inspected",
            _ => "Unknown"
        };
    }
}
