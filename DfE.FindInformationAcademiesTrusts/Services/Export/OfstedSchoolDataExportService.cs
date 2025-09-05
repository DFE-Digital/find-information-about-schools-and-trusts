using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.Services.Export;

public interface IOfstedSchoolDataExportService
{
    Task<byte[]> BuildAsync(int urn);
}

public class OfstedSchoolDataExportService(ISchoolService schoolService, ISchoolOverviewDetailsService schoolOverviewDetailsService) : ExportBuilder("Ofsted"), IOfstedSchoolDataExportService
{
    private const string NotAvailable = "Not available";

    private readonly List<string> _headers =
    [
        "Inspection type",
        "Date of inspection",
        "Grade",
        "Quality of education",
        "Behaviour and attitudes",
        "Personal development",
        "Leadership and management",
        "Early years provision",
        "Sixth form provision",
        "Effective safeguarding",
        "Category of concern",
        "Before or after joining the trust",
    ];

    public async Task<byte[]> BuildAsync(int urn)
    {
        var schoolDetails = await schoolService.GetSchoolSummaryAsync(urn);

        if (schoolDetails is null)
        {
            throw new DataIntegrityException($"School summary not found for URN: {urn}");
        }
        
        var schoolOverview =
            await schoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(urn, schoolDetails.Category);
        var ofstedRatings = await schoolService.GetSchoolOfstedRatingsAsync(urn);
        return WriteSchoolInformation(schoolOverview, schoolDetails.Category)
            .WriteHeaders(_headers)
            .WriteRows(() => WriteRows(ofstedRatings))
            .Build();
    }
    
    private void WriteRows(SchoolOfstedServiceModel ofstedRatings)
    {
        if (ofstedRatings.HasRecentShortInspection)
        {
            var whenDidShortInspectionHappen = ofstedRatings.DateAcademyJoinedTrust switch
            {
                null => BeforeOrAfterJoining.NotApplicable,
                var d when d <= ofstedRatings.ShortInspection.InspectionDate => BeforeOrAfterJoining.After,
                var d when d > ofstedRatings.ShortInspection.InspectionDate => BeforeOrAfterJoining.Before,
                _ => BeforeOrAfterJoining.NotApplicable
            };
            WriteRecentShortInspectionRow(ofstedRatings.ShortInspection, whenDidShortInspectionHappen);
        }
        WriteFullInspectionRow(ofstedRatings.CurrentOfstedRating, true, ofstedRatings.WhenDidCurrentInspectionHappen);
        WriteFullInspectionRow(ofstedRatings.PreviousOfstedRating, false, ofstedRatings.WhenDidPreviousInspectionHappen);
    }

    private void WriteRecentShortInspectionRow(OfstedShortInspection shortInspection, BeforeOrAfterJoining beforeOrAfterJoiningTrust)
    {
        SetTextCell(ExportColumns.OfstedSchoolColumns.InspectionType, "Recent short inspection");
        SetDateCell(ExportColumns.OfstedSchoolColumns.DateOfInspection, shortInspection.InspectionDate);
        SetTextCell(ExportColumns.OfstedSchoolColumns.Grade, shortInspection.InspectionOutcome ?? string.Empty);
        SetTextCell(ExportColumns.OfstedSchoolColumns.QualityOfEducation, NotAvailable);
        SetTextCell(ExportColumns.OfstedSchoolColumns.BehaviourAndAttitudes, NotAvailable);
        SetTextCell(ExportColumns.OfstedSchoolColumns.PersonalDevelopment, NotAvailable);
        SetTextCell(ExportColumns.OfstedSchoolColumns.LeadershipAndManagement, NotAvailable);
        SetTextCell(ExportColumns.OfstedSchoolColumns.EarlyYearsProvision, NotAvailable);
        SetTextCell(ExportColumns.OfstedSchoolColumns.SixthFormProvision, NotAvailable);
        SetTextCell(ExportColumns.OfstedSchoolColumns.EffectiveSafeguarding, NotAvailable);
        SetTextCell(ExportColumns.OfstedSchoolColumns.CategoryOfConcern, NotAvailable);
        SetTextCell(ExportColumns.OfstedSchoolColumns.BeforeOrAfterJoiningTrust, beforeOrAfterJoiningTrust.ToDisplayString());
        
        CurrentRow++;
    }

    private void WriteFullInspectionRow(OfstedRating ofstedRating, bool isCurrent, BeforeOrAfterJoining beforeOrAfterJoiningTrust)
    {
        SetTextCell(ExportColumns.OfstedSchoolColumns.InspectionType, isCurrent ? "Current inspection" : "Previous inspection");
        SetDateCell(ExportColumns.OfstedSchoolColumns.DateOfInspection, ofstedRating.InspectionDate);
        SetTextCell(ExportColumns.OfstedSchoolColumns.Grade, ofstedRating.OverallEffectiveness.ToDisplayString(isCurrent));
        SetTextCell(ExportColumns.OfstedSchoolColumns.QualityOfEducation, ofstedRating.QualityOfEducation.ToDisplayString(isCurrent));
        SetTextCell(ExportColumns.OfstedSchoolColumns.BehaviourAndAttitudes, ofstedRating.BehaviourAndAttitudes.ToDisplayString(isCurrent));
        SetTextCell(ExportColumns.OfstedSchoolColumns.PersonalDevelopment, ofstedRating.PersonalDevelopment.ToDisplayString(isCurrent));
        SetTextCell(ExportColumns.OfstedSchoolColumns.LeadershipAndManagement, ofstedRating.EffectivenessOfLeadershipAndManagement.ToDisplayString(isCurrent));
        SetTextCell(ExportColumns.OfstedSchoolColumns.EarlyYearsProvision, ofstedRating.EarlyYearsProvision.ToDisplayString(isCurrent));
        SetTextCell(ExportColumns.OfstedSchoolColumns.SixthFormProvision, ofstedRating.SixthFormProvision.ToDisplayString(isCurrent));
        SetTextCell(ExportColumns.OfstedSchoolColumns.EffectiveSafeguarding, ofstedRating.SafeguardingIsEffective.ToDisplayString());
        SetTextCell(ExportColumns.OfstedSchoolColumns.CategoryOfConcern, ofstedRating.CategoryOfConcern.ToDisplayString());
        SetTextCell(ExportColumns.OfstedSchoolColumns.BeforeOrAfterJoiningTrust, beforeOrAfterJoiningTrust.ToDisplayString());
        
        CurrentRow++;
    }
}
