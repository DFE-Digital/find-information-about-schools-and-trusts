using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Data.UnitTests;

public class SchoolPopulationTests
{
    [Fact]
    public void Unknown_has_NotAvailable_statistic_for_all_parameters()
    {
        SchoolPopulation.Unknown.PupilsOnRole
            .Should().Be(Statistic<int>.NotAvailable);
        SchoolPopulation.Unknown.PupilsWithEhcPlan
            .Should().Be(Statistic<int>.NotAvailable);
        SchoolPopulation.Unknown.PupilsWithEhcPlanPercentage
            .Should().Be(Statistic<decimal>.NotAvailable);
        SchoolPopulation.Unknown.PupilsWithSenSupport
            .Should().Be(Statistic<int>.NotAvailable);
        SchoolPopulation.Unknown.PupilsWithSenSupportPercentage
            .Should().Be(Statistic<decimal>.NotAvailable);
        SchoolPopulation.Unknown.PupilsWithEnglishAsAdditionalLanguage
            .Should().Be(Statistic<int>.NotAvailable);
        SchoolPopulation.Unknown.PupilsWithEnglishAsAdditionalLanguagePercentage
            .Should().Be(Statistic<decimal>.NotAvailable);
        SchoolPopulation.Unknown.PupilsEligibleForFreeSchoolMeals
            .Should().Be(Statistic<int>.NotAvailable);
        SchoolPopulation.Unknown.PupilsEligibleForFreeSchoolMealsPercentage
            .Should().Be(Statistic<decimal>.NotAvailable);
    }

    [Fact]
    public void NotYetSubmitted_has_NotYetSubmitted_statistic_for_all_parameters()
    {
        SchoolPopulation.NotYetSubmitted.PupilsOnRole
            .Should().Be(Statistic<int>.NotYetSubmitted);
        SchoolPopulation.NotYetSubmitted.PupilsWithEhcPlan
            .Should().Be(Statistic<int>.NotYetSubmitted);
        SchoolPopulation.NotYetSubmitted.PupilsWithEhcPlanPercentage
            .Should().Be(Statistic<decimal>.NotYetSubmitted);
        SchoolPopulation.NotYetSubmitted.PupilsWithSenSupport
            .Should().Be(Statistic<int>.NotYetSubmitted);
        SchoolPopulation.NotYetSubmitted.PupilsWithSenSupportPercentage
            .Should().Be(Statistic<decimal>.NotYetSubmitted);
        SchoolPopulation.NotYetSubmitted.PupilsWithEnglishAsAdditionalLanguage
            .Should().Be(Statistic<int>.NotYetSubmitted);
        SchoolPopulation.NotYetSubmitted.PupilsWithEnglishAsAdditionalLanguagePercentage
            .Should().Be(Statistic<decimal>.NotYetSubmitted);
        SchoolPopulation.NotYetSubmitted.PupilsEligibleForFreeSchoolMeals
            .Should().Be(Statistic<int>.NotYetSubmitted);
        SchoolPopulation.NotYetSubmitted.PupilsEligibleForFreeSchoolMealsPercentage
            .Should().Be(Statistic<decimal>.NotYetSubmitted);
    }
}
