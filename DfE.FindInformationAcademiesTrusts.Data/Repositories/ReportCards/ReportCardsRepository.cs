using System.Globalization;
using Dfe.AcademiesApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards
{
    public class ReportCardsRepository(IEstablishmentsV5Client establishmentsClient) : IReportCardsRepository
    {
        public async Task<(EstablishmentReportCard? LatestReportCard, EstablishmentReportCard? PreviousReportCard)>
            GetReportCardAsync(int urn)
        {
            var result =
                await establishmentsClient.SearchEstablishmentsWithMockReportCardsAsync(null, null, urn.ToString(),
                    null, null);

            var establishmentData = result.FirstOrDefault(x => x.Urn == urn.ToString());

            if (establishmentData is not null)
            {
                var latestReportCard = MapLatestReportCard(establishmentData.ReportCard!);
                var previousReportCard = MapPreviousReportCard(establishmentData.ReportCard!);

                return (latestReportCard, previousReportCard);
            }

            return (null, null);
        }

        private EstablishmentReportCard? MapLatestReportCard(ReportCardDto reportCardDto)
        {
            if(reportCardDto.LatestInspectionDate is null)
            {
                return null;
            }

            return new EstablishmentReportCard(
                ParseDate(reportCardDto.LatestInspectionDate),
                reportCardDto.WebLink,
                reportCardDto.LatestCurriculumAndTeaching,
                reportCardDto.LatestAttendanceAndBehaviour,
                reportCardDto.LatestPersonalDevelopmentAndWellbeing,
                reportCardDto.LatestLeadershipAndGovernance,
                reportCardDto.LatestInclusion,
                reportCardDto.LatestAchievement,
                reportCardDto.LatestEarlyYearsProvision,
                reportCardDto.LatestSafeguarding);
        }

        private EstablishmentReportCard? MapPreviousReportCard(ReportCardDto reportCardDto)
        {
            if (reportCardDto.PreviousInspectionDate is null)
            {
                return null;
            }

            return new EstablishmentReportCard(
                ParseDate(reportCardDto.PreviousInspectionDate),
                reportCardDto.WebLink,
                reportCardDto.PreviousCurriculumAndTeaching,
                reportCardDto.PreviousAttendanceAndBehaviour,
                reportCardDto.PreviousPersonalDevelopmentAndWellbeing,
                reportCardDto.PreviousLeadershipAndGovernance,
                reportCardDto.PreviousInclusion,
                reportCardDto.PreviousAchievement,
                reportCardDto.PreviousEarlyYearsProvision,
                reportCardDto.PreviousSafeguarding);
        }

        private DateOnly ParseDate(string dateString)
        {
            return DateOnly.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

    }
}
