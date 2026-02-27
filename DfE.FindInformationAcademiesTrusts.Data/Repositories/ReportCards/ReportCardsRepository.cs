using System.Globalization;
using Dfe.AcademiesApi.Client.Contracts;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards
{
    public class ReportCardsRepository(IEstablishmentsV5Client establishmentsClient, ILogger<IReportCardsRepository> logger) : IReportCardsRepository
    {
        public async Task<ReportCardData> GetReportCardAsync(int urn)
        {
            var result = await establishmentsClient.SearchEstablishmentsWithOfstedReportCardsAsync(null, null, urn.ToString(),
                    null, null);

            var establishmentData = result.FirstOrDefault(x => x.Urn == urn.ToString());

            if (establishmentData is null)
            {
                return new ReportCardData
                {
                    Urn = urn,
                    LatestReportCard = null,
                    PreviousReportCard = null
                };
            }

            var latestReportCard = MapLatestReportCard(establishmentData.ReportCardFullInspection);
            var previousReportCard = MapPreviousReportCard(establishmentData.ReportCardFullInspection);

            return new ReportCardData
            {
                Urn = urn,
                LatestReportCard = latestReportCard,
                PreviousReportCard = previousReportCard
            };

        }

        public async Task<List<ReportCardData>> GetReportCardsAsync(List<int> urns)
        {
            var response = new List<ReportCardData>();

            var results = await establishmentsClient.GetEstablishmentsWithOfstedReportCardsByUrnsAsync(new UrnRequestModel
            {
                Urns = urns
            });

            foreach (var result in results)
            {
                if (!int.TryParse(result.Urn, out var establishmentUrn))
                {
                    logger.LogError("Unable to parse academy urn {Urn}", result.Urn);

                    continue;
                }

                if (urns.Contains(establishmentUrn))
                {
                    response.Add(new ReportCardData
                    {
                        Urn = establishmentUrn,
                        LatestReportCard = MapLatestReportCard(result.ReportCardFullInspection),
                        PreviousReportCard = MapPreviousReportCard(result.ReportCardFullInspection)
                    });
                }
            }

            return response;
        }

        private static EstablishmentReportCard? MapLatestReportCard(ReportCardFullInspectionDto? reportCardDto)
        {
            if(reportCardDto?.LatestInspectionDate is null)
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
                reportCardDto.LatestSafeguarding,
                reportCardDto.LatestPost16Provision,
                reportCardDto.LatestCategoryOfConcern
                );
        }

        private static EstablishmentReportCard? MapPreviousReportCard(ReportCardFullInspectionDto? reportCardDto)
        {
            if (reportCardDto?.PreviousInspectionDate is null)
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
                reportCardDto.PreviousSafeguarding,
                reportCardDto.PreviousPost16Provision,
                reportCardDto.PreviousCategoryOfConcern);
        }

        private static DateOnly ParseDate(string dateString)
        {
            return DateOnly.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

    }
}
