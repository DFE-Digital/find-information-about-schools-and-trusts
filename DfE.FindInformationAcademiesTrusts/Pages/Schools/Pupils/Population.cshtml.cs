using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;

public class PopulationModel(
    ISchoolPupilService schoolPupilService,
    IDateTimeProvider dateTimeProvider,
    IDataSourceService dataSourceService,
    ISchoolPupilsExportService schoolPupilsExportService,
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolNavMenu schoolNavMenu
) : PupilsAreaModel(dateTimeProvider, dataSourceService, schoolPupilsExportService, schoolService, trustService, schoolNavMenu)
{
    public const string SubPageName = "Population";
    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

    public List<PopulationDataViewModel> PopulationData { get; set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var statistics = await schoolPupilService.GetSchoolPopulationStatisticsAsync(
            Urn,
            CensusYear.Previous(DateTimeProvider, Census.Spring, 3),
            CensusYear.Next(DateTimeProvider, Census.Spring)
        );

        PopulationData = statistics.OrderByDescending(kvp => kvp.Key.Value)
            .Select(kvp => PopulationDataViewModel.FromSchoolPopulation(kvp.Key, kvp.Value))
            .ToList();

        return pageResult;
    }
}

public record PopulationDataViewModel(
    CensusYear Year,
    string NumberOfPupilsOnRoleDisplay,
    string NumberOfPupilsOnRoleSort,
    string EligiblePupilsWithEhcPlanDisplay,
    string EligiblePupilsWithEhcPlanSort,
    string EligiblePupilsWithSenSupportDisplay,
    string EligiblePupilsWithSenSupportSort,
    string EnglishAsAnAdditionalLanguageDisplay,
    string EnglishAsAnAdditionalLanguageSort,
    string EligibleForFreeSchoolMealsDisplay,
    string EligibleForFreeSchoolMealsSort
)
{
    public static PopulationDataViewModel FromSchoolPopulation(CensusYear censusYear, SchoolPopulation schoolPopulation)
    {
        return new PopulationDataViewModel(
            censusYear,
            schoolPopulation.PupilsOnRole.DisplayValue(),
            schoolPopulation.PupilsOnRole.SortValue(),
            schoolPopulation.PupilsWithEhcPlan
                .DisplayValueWithPercentage(schoolPopulation.PupilsWithEhcPlanPercentage),
            schoolPopulation.PupilsWithEhcPlan.SortValue(),
            schoolPopulation.PupilsWithSenSupport
                .DisplayValueWithPercentage(schoolPopulation.PupilsWithSenSupportPercentage),
            schoolPopulation.PupilsWithSenSupport.SortValue(),
            schoolPopulation.PupilsWithEnglishAsAdditionalLanguage
                .DisplayValueWithPercentage(schoolPopulation.PupilsWithEnglishAsAdditionalLanguagePercentage),
            schoolPopulation.PupilsWithEnglishAsAdditionalLanguage.SortValue(),
            schoolPopulation.PupilsEligibleForFreeSchoolMeals
                .DisplayValueWithPercentage(schoolPopulation.PupilsEligibleForFreeSchoolMealsPercentage),
            schoolPopulation.PupilsEligibleForFreeSchoolMeals.SortValue()
        );
    }
}
