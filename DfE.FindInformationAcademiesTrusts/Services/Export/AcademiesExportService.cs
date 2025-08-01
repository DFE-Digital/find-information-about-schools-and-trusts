using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using static DfE.FindInformationAcademiesTrusts.Services.Export.ExportColumns;

namespace DfE.FindInformationAcademiesTrusts.Services.Export;

public interface IAcademiesExportService
{
    Task<byte[]> BuildAsync(string uid);
}

public class AcademiesExportService(ITrustService trustService, IAcademyService academyService)
    : ExportBuilder("Academies"), IAcademiesExportService
{
    private readonly List<string> headers =
    [
        CommonColumnNames.SchoolName,
        CommonColumnNames.Urn,
        CommonColumnNames.LocalAuthority,
        "Type",
        "Rural or Urban",
        CommonColumnNames.DateJoined,
        "Current Ofsted Rating",
        CommonColumnNames.BeforeOrAfterJoiningHeader,
        "Date of Current Ofsted",
        "Previous Ofsted Rating",
        CommonColumnNames.BeforeOrAfterJoiningHeader,
        "Date of Previous Ofsted",
        "Phase of Education",
        CommonColumnNames.AgeRange,
        "Pupil Numbers",
        "Capacity",
        "% Full",
        "Pupils eligible for Free School Meals",
        "LA average pupils eligible for Free School Meals",
        "National average pupils eligible for Free School Meals"
    ];

    public async Task<byte[]> BuildAsync(string uid)
    {
        var trustSummary = await trustService.GetTrustSummaryAsync(uid);

        if (trustSummary is null)
        {
            throw new DataIntegrityException($"Trust summary not found for UID {uid}");
        }

        var academiesDetails = await academyService.GetAcademiesInTrustDetailsAsync(uid);
        var academiesOfstedRatings = await academyService.GetAcademiesInTrustOfstedAsync(uid);
        var academiesPupilNumbers = await academyService.GetAcademiesInTrustPupilNumbersAsync(uid);
        var academiesFreeSchoolMeals = await academyService.GetAcademiesInTrustFreeSchoolMealsAsync(uid);

        return WriteTrustInformation(trustSummary)
            .WriteHeaders(headers)
            .WriteRows(() =>
            {
                WriteRows(academiesDetails, academiesOfstedRatings, academiesPupilNumbers,
                    academiesFreeSchoolMeals);
            })
            .Build();
    }

    private void WriteRows(AcademyDetailsServiceModel[] academies, SchoolOfstedServiceModel[] academiesOfstedRatings,
        AcademyPupilNumbersServiceModel[] academiesPupilNumbers,
        AcademyFreeSchoolMealsServiceModel[] academiesFreeSchoolMeals)
    {
        foreach (var details in academies)
        {
            var ofstedData = academiesOfstedRatings.SingleOrDefault(x => x.Urn == details.Urn);
            var pupilData = academiesPupilNumbers.SingleOrDefault(x => x.Urn == details.Urn);
            var freeMealsData = academiesFreeSchoolMeals.SingleOrDefault(x => x.Urn == details.Urn);

            GenerateAcademyRow(details, ofstedData, pupilData, freeMealsData);
        }
    }

    private void GenerateAcademyRow(
        AcademyDetailsServiceModel academy,
        SchoolOfstedServiceModel? ofstedData,
        AcademyPupilNumbersServiceModel? pupilNumbersData,
        AcademyFreeSchoolMealsServiceModel? freeSchoolMealsData)
    {
        var previousRating = ofstedData?.PreviousOfstedRating;
        var currentRating = ofstedData?.CurrentOfstedRating;
        var percentageFull = pupilNumbersData?.PercentageFull ?? 0;

        SetTextCell(AcademyColumns.SchoolName, academy.EstablishmentName ?? string.Empty);
        SetTextCell(AcademyColumns.Urn, academy.Urn);
        SetTextCell(AcademyColumns.LocalAuthority, academy.LocalAuthority ?? string.Empty);
        SetTextCell(AcademyColumns.Type, academy.TypeOfEstablishment ?? string.Empty);
        SetTextCell(AcademyColumns.RuralOrUrban, academy.UrbanRural ?? string.Empty);

        SetDateCell(AcademyColumns.DateJoined, ofstedData?.DateAcademyJoinedTrust);

        SetTextCell(AcademyColumns.CurrentOfstedRating,
            currentRating?.OverallEffectiveness.ToDisplayString(true) ?? string.Empty);
        SetTextCell(AcademyColumns.CurrentBeforeAfterJoining,
            ofstedData?.WhenDidCurrentInspectionHappen == BeforeOrAfterJoining.NotYetInspected
                ? string.Empty
                : ofstedData?.WhenDidCurrentInspectionHappen.ToDisplayString() ?? string.Empty);
        SetDateCell(AcademyColumns.DateOfCurrentInspection, currentRating?.InspectionDate);

        SetTextCell(AcademyColumns.PreviousOfstedRating,
            previousRating?.OverallEffectiveness.ToDisplayString(false) ?? string.Empty);

        SetTextCell(AcademyColumns.PreviousBeforeAfterJoining,
            ofstedData?.WhenDidPreviousInspectionHappen == BeforeOrAfterJoining.NotYetInspected
                ? string.Empty
                : ofstedData?.WhenDidPreviousInspectionHappen.ToDisplayString() ?? string.Empty);

        SetDateCell(AcademyColumns.DateOfPreviousInspection, previousRating?.InspectionDate);

        SetTextCell(AcademyColumns.PhaseOfEducation, pupilNumbersData?.PhaseOfEducation ?? string.Empty);

        SetTextCell(AcademyColumns.AgeRange, pupilNumbersData?.AgeRange.ToTabularDisplayString() ?? string.Empty);

        SetTextCell(AcademyColumns.PupilNumbers, pupilNumbersData?.NumberOfPupils?.ToString() ?? string.Empty);
        SetTextCell(AcademyColumns.Capacity, pupilNumbersData?.SchoolCapacity?.ToString() ?? string.Empty);
        SetTextCell(AcademyColumns.PercentFull, percentageFull > 0 ? $"{percentageFull}%" : string.Empty);
        SetTextCell(AcademyColumns.PupilsEligibleFreeSchoolMeals,
            freeSchoolMealsData is { PercentageFreeSchoolMeals: not null }
                ? $"{freeSchoolMealsData.PercentageFreeSchoolMeals}%"
                : string.Empty
        );

        SetTextCell(AcademyColumns.LaPupilsEligibleFreeSchoolMeals,
            freeSchoolMealsData is { LaAveragePercentageFreeSchoolMeals: > 0 }
                ? $"{Math.Round(freeSchoolMealsData.LaAveragePercentageFreeSchoolMeals, 1)}%"
                : string.Empty
        );
        SetTextCell(AcademyColumns.NationalPupilsEligibleFreeSchoolMeals,
            freeSchoolMealsData is { NationalAveragePercentageFreeSchoolMeals: > 0 }
                ? $"{Math.Round(freeSchoolMealsData.NationalAveragePercentageFreeSchoolMeals, 1)}%"
                : string.Empty
        );

        CurrentRow++;
    }
}
