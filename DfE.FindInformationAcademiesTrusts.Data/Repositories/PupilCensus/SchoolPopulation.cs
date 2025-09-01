namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

public record SchoolPopulation(
    Statistic<int> PupilsOnRole,
    Statistic<int> PupilsWithEhcPlan,
    Statistic<decimal> PupilsWithEhcPlanPercentage,
    Statistic<int> PupilsWithSenSupport,
    Statistic<decimal> PupilsWithSenSupportPercentage,
    Statistic<int> PupilsWithEnglishAsAdditionalLanguage,
    Statistic<decimal> PupilsWithEnglishAsAdditionalLanguagePercentage,
    Statistic<int> PupilsEligibleForFreeSchoolMeals,
    Statistic<decimal> PupilsEligibleForFreeSchoolMealsPercentage
)
{
    public static readonly SchoolPopulation Unknown = new(
        Statistic<int>.NotAvailable,
        Statistic<int>.NotAvailable,
        Statistic<decimal>.NotAvailable,
        Statistic<int>.NotAvailable,
        Statistic<decimal>.NotAvailable,
        Statistic<int>.NotAvailable,
        Statistic<decimal>.NotAvailable,
        Statistic<int>.NotAvailable,
        Statistic<decimal>.NotAvailable
    );

    public static readonly SchoolPopulation NotYetSubmitted = new(
        Statistic<int>.NotYetSubmitted,
        Statistic<int>.NotYetSubmitted,
        Statistic<decimal>.NotYetSubmitted,
        Statistic<int>.NotYetSubmitted,
        Statistic<decimal>.NotYetSubmitted,
        Statistic<int>.NotYetSubmitted,
        Statistic<decimal>.NotYetSubmitted,
        Statistic<int>.NotYetSubmitted,
        Statistic<decimal>.NotYetSubmitted
    );
}
