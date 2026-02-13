using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;
using GovUK.Dfe.CoreLibs.Caching.Helpers;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class ReportCardsService(
        IReportCardsRepository reportCardsRepository,
        ICacheService<IMemoryCacheType> cacheService) : IReportCardsService
    {
        public async Task<ReportCardServiceModel> GetReportCardsAsync(int urn)
        {
            var cacheKey = $"ReportCards_{CacheKeyHelper.GenerateHashedCacheKey(urn.ToString())}";
            var methodName = nameof(GetReportCardsAsync);

            var reportCards = await cacheService.GetOrAddAsync(cacheKey,
                async () => await reportCardsRepository.GetReportCardAsync(urn), methodName);

            return new ReportCardServiceModel
            {
                LatestReportCard = MapToReportCardDetails(reportCards.LatestReportCard),
                PreviousReportCard = MapToReportCardDetails(reportCards.PreviousReportCard)
            };
        }

        private static ReportCardDetails? MapToReportCardDetails(EstablishmentReportCard? reportCard)
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
                MapSafeGuarding(reportCard.Safeguarding),
                reportCard.Post16Provision,
                MapCategoryOfConcern(reportCard.CategoryOfConcern));
        }

        private static string MapCategoryOfConcern(string? raw)
        {
            return raw switch
            {
                null => "Not inspected",
                "" => "No concerns",
                "SM" => "Special measures",
                "SWK" => "Serious weaknesses",
                "NTI" => "Notice to improve",
                _ => raw
            };
        }

        private static string MapSafeGuarding(string? raw)
        {
            return raw switch
            {
                null or "NULL" or "" => "Not inspected",
                "Yes" => "Yes",
                "No" => "No",
                "9" => "Not recorded",
                _ => raw
            };
        }
    }
}

