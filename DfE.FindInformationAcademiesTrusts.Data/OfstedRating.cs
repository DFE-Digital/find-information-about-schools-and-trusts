namespace DfE.FindInformationAcademiesTrusts.Data;

public record OfstedRating(
    OfstedRatingScore OverallEffectiveness,
    OfstedRatingScore QualityOfEducation,
    OfstedRatingScore BehaviourAndAttitudes,
    OfstedRatingScore PersonalDevelopment,
    OfstedRatingScore EffectivenessOfLeadershipAndManagement,
    OfstedRatingScore EarlyYearsProvision,
    OfstedRatingScore SixthFormProvision,
    CategoriesOfConcern CategoryOfConcern,
    SafeguardingScore SafeguardingIsEffective,
    DateTime? InspectionDate)
{

    public static readonly OfstedRating NotInspected = new(OfstedRatingScore.NotInspected,
        OfstedRatingScore.NotInspected, OfstedRatingScore.NotInspected, OfstedRatingScore.NotInspected,
        OfstedRatingScore.NotInspected, OfstedRatingScore.NotInspected, OfstedRatingScore.NotInspected,
        CategoriesOfConcern.NotInspected, SafeguardingScore.NotInspected, null);

    public static readonly OfstedRating Unknown = new(OfstedRatingScore.Unknown, OfstedRatingScore.Unknown,
        OfstedRatingScore.Unknown, OfstedRatingScore.Unknown, OfstedRatingScore.Unknown, OfstedRatingScore.Unknown,
        OfstedRatingScore.Unknown, CategoriesOfConcern.Unknown, SafeguardingScore.Unknown, null);

    public OfstedRating(int? overallEffectiveness, DateTime? inspectionDate)
        : this(
            (OfstedRatingScore?)overallEffectiveness ?? OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            CategoriesOfConcern.NotInspected,
            SafeguardingScore.NotInspected,
            inspectionDate
        )
    {
    }

    public bool HasAnyUnknownRating => OverallEffectiveness == OfstedRatingScore.Unknown
                                       || QualityOfEducation == OfstedRatingScore.Unknown
                                       || BehaviourAndAttitudes == OfstedRatingScore.Unknown
                                       || PersonalDevelopment == OfstedRatingScore.Unknown
                                       || EffectivenessOfLeadershipAndManagement == OfstedRatingScore.Unknown
                                       || EarlyYearsProvision == OfstedRatingScore.Unknown
                                       || SixthFormProvision == OfstedRatingScore.Unknown
                                       || CategoryOfConcern == CategoriesOfConcern.Unknown
                                       || SafeguardingIsEffective == SafeguardingScore.Unknown;
}

public enum OfstedRatingScore
{
    Unknown = -99,
    NotInspected = -1,
    InsufficientEvidence = 0,
    Outstanding = 1,
    Good = 2,
    RequiresImprovement = 3,
    Inadequate = 4,
    DoesNotApply = 8,
    SingleHeadlineGradeNotAvailable = 9
}

public enum SafeguardingScore
{
    Unknown = -99,
    NotInspected = -1,
    Yes,
    No,
    NotRecorded = 9
}

public enum CategoriesOfConcern
{
    Unknown = -99,
    NotInspected = -1,
    DoesNotApply,
    NoConcerns,
    SpecialMeasures,
    SeriousWeakness,
    NoticeToImprove
}
