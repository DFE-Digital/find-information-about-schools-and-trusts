using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using static DfE.FindInformationAcademiesTrusts.Services.Export.ExportColumns;

namespace DfE.FindInformationAcademiesTrusts.Services.Export
{
    public interface IOfstedTrustDataExportService
    {
        Task<byte[]> BuildAsync(string uid);
    }

    public class OfstedTrustDataExportService(IAcademyService academyService, ITrustService trustService) : ExportBuilder("Ofsted"), IOfstedTrustDataExportService
    {
        private readonly List<string> headers =
        [
            CommonColumnNames.SchoolName,
            CommonColumnNames.DateJoined,
            "Has recent short inspection",
            "Current single headline grade",
            CommonColumnNames.BeforeOrAfterJoiningHeader,
            "Date of Current Inspection",
            "Previous single headline grade",
            CommonColumnNames.BeforeOrAfterJoiningHeader,
            "Date of previous inspection",
            "Quality of Education",
            "Behaviour and Attitudes",
            "Personal Development",
            "Leadership and Management",
            "Early Years Provision",
            "Sixth Form Provision",
            "Previous Quality of Education",
            "Previous Behaviour and Attitudes",
            "Previous Personal Development",
            "Previous Leadership and Management",
            "Previous Early Years Provision",
            "Previous Sixth Form Provision",
            "Effective Safeguarding",
            "Category of Concern"
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

            return WriteTrustInformation(trustSummary)
                .WriteHeaders(headers)
                .WriteRows(() =>  WriteRows(academiesDetails, academiesOfstedRatings))
                .Build();
        }

        private void WriteRows(AcademyDetailsServiceModel[] academies, SchoolOfstedServiceModel[] academiesOfstedRatings)
        {
            foreach (var details in academies)
            {
                var ofstedData = academiesOfstedRatings.SingleOrDefault(x => x.Urn == details.Urn);

                GenerateOfstedRow(details, ofstedData);
            }
        }

        private void GenerateOfstedRow(AcademyDetailsServiceModel academy, SchoolOfstedServiceModel? ofstedData)
        {
            var previousRating = ofstedData?.PreviousOfstedRating ?? OfstedRating.NotInspected;
            var currentRating = ofstedData?.CurrentOfstedRating ?? OfstedRating.NotInspected;

            SetTextCell(OfstedTrustColumns.SchoolName, academy.EstablishmentName ?? string.Empty);
            SetDateCell(OfstedTrustColumns.DateJoined, ofstedData?.DateAcademyJoinedTrust);
            SetBoolCell(OfstedTrustColumns.HasRecentShortInspection, ofstedData?.HasRecentShortInspection);
            SetTextCell(OfstedTrustColumns.CurrentSingleHeadlineGrade, currentRating.OverallEffectiveness.ToDisplayString(true));
            SetTextCell(OfstedTrustColumns.CurrentBeforeAfterJoining, ofstedData?.WhenDidCurrentInspectionHappen.ToDisplayString() ?? string.Empty);
            SetDateCell(OfstedTrustColumns.DateOfCurrentInspection, currentRating.InspectionDate);
            SetTextCell(OfstedTrustColumns.PreviousSingleHeadlineGrade, previousRating.OverallEffectiveness.ToDisplayString(false));
            SetTextCell(OfstedTrustColumns.PreviousBeforeAfterJoining, ofstedData?.WhenDidPreviousInspectionHappen.ToDisplayString() ?? string.Empty);
            SetDateCell(OfstedTrustColumns.DateOfPreviousInspection, previousRating.InspectionDate);
            SetTextCell(OfstedTrustColumns.CurrentQualityOfEducation, currentRating.QualityOfEducation.ToDisplayString(true));
            SetTextCell(OfstedTrustColumns.CurrentBehaviourAndAttitudes, currentRating.BehaviourAndAttitudes.ToDisplayString(true)); 
            SetTextCell(OfstedTrustColumns.CurrentPersonalDevelopment, currentRating.PersonalDevelopment.ToDisplayString(true)); 
            SetTextCell(OfstedTrustColumns.CurrentLeadershipAndManagement, currentRating.EffectivenessOfLeadershipAndManagement.ToDisplayString(true));
            SetTextCell(OfstedTrustColumns.CurrentEarlyYearsProvision, currentRating.EarlyYearsProvision.ToDisplayString(true)); 
            SetTextCell(OfstedTrustColumns.CurrentSixthFormProvision, currentRating.SixthFormProvision.ToDisplayString(true));

 
            SetTextCell(OfstedTrustColumns.PreviousQualityOfEducation, previousRating.QualityOfEducation.ToDisplayString(false));
            SetTextCell(OfstedTrustColumns.PreviousBehaviourAndAttitudes, previousRating.BehaviourAndAttitudes.ToDisplayString(false));
            SetTextCell(OfstedTrustColumns.PreviousPersonalDevelopment, previousRating.PersonalDevelopment.ToDisplayString(false));
            SetTextCell(OfstedTrustColumns.PreviousLeadershipAndManagement, previousRating.EffectivenessOfLeadershipAndManagement.ToDisplayString(false));
            SetTextCell(OfstedTrustColumns.PreviousEarlyYearsProvision, previousRating.EarlyYearsProvision.ToDisplayString(false));
            SetTextCell(OfstedTrustColumns.PreviousSixthFormProvision, previousRating.SixthFormProvision.ToDisplayString(false));

            SetTextCell(OfstedTrustColumns.EffectiveSafeguarding, currentRating.SafeguardingIsEffective.ToDisplayString());

            SetTextCell(OfstedTrustColumns.CategoryOfConcern, currentRating.CategoryOfConcern.ToDisplayString());

            CurrentRow++;
        }
    }
}
