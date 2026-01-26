using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using GovUK.Dfe.CoreLibs.Caching.Helpers;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class ReportCardsService(IReportCardsRepository reportCardsRepository, ISchoolRepository schoolRepository , ICacheService<IMemoryCacheType> cacheService) : IReportCardsService
    {
        public async Task<ReportCardServiceModel> GetReportCardsAsync(int urn)
        {
            var cacheKey = $"ReportCards_{CacheKeyHelper.GenerateHashedCacheKey(urn.ToString())}";
            var methodName = nameof(GetReportCardsAsync);

            var reportCards = await cacheService.GetOrAddAsync(cacheKey,
                async () => await reportCardsRepository.GetReportCardAsync(urn), methodName);

            var dateJoinedTrust = await schoolRepository.GetDateJoinedTrustAsync(urn);

            return new ReportCardServiceModel
            {
                DateJoinedTrust = dateJoinedTrust,
                LatestReportCard = MapToReportCardDetails(reportCards.LatestReportCard),
                PreviousReportCard = MapToReportCardDetails(reportCards.PreviousReportCard)
            };
        }

        private ReportCardDetails? MapToReportCardDetails(EstablishmentReportCard? reportCard)
        {
            if (reportCard == null)
            {
                return null;
            }

            return new ReportCardDetails(
                reportCard.InspectionDate,
                reportCard.WebLink,
                reportCard.CurriculumAndTeaching,
                reportCard.AttendanceAndBehaviour,
                reportCard.PersonalDevelopmentAndWellBeing,
                reportCard.LeadershipAndGovernance,
                reportCard.Inclusion,
                reportCard.Achievement,
                reportCard.EarlyYearsProvision,
                reportCard.Safeguarding,
                reportCard.Post16Provision,
                reportCard.CategoryOfConcern
            );
        }
    }
}
