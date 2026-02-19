using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;
using GovUK.Dfe.CoreLibs.Caching.Helpers;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class ReportCardsService(IReportCardsRepository reportCardsRepository, ICacheService<IMemoryCacheType> cacheService, ILogger<IReportCardsService> logger) : IReportCardsService
    {
        public async Task<ReportCardServiceModel> GetReportCardsAsync(int urn)
        {
            var cacheKey = $"ReportCards_{CacheKeyHelper.GenerateHashedCacheKey(urn.ToString())}";
            var methodName = nameof(GetReportCardsAsync);

            var reportCard = await cacheService.GetOrAddAsync(cacheKey,
                async () => await reportCardsRepository.GetReportCardAsync(urn), methodName);

            return new ReportCardServiceModel
            {
                Urn = urn,
                LatestReportCard = MapToReportCardDetails(reportCard.LatestReportCard),
                PreviousReportCard = MapToReportCardDetails(reportCard.PreviousReportCard)
            };
        }

        public async Task<List<ReportCardServiceModel>> GetReportCardsAsync(List<string> urns)
        {

            List<int> parsedUrns = [];

            foreach (var urnString in urns)
            {
                if (int.TryParse(urnString, out var urn))
                {
                    parsedUrns.Add(urn);
                }
                else
                {
                    logger.LogError("Unable to parse academy urn {Urn}", urnString);
                }
            }

            if (!parsedUrns.Any())
            {
                return [];
            }

            var cacheKey = $"ReportCards_{CacheKeyHelper.GenerateHashedCacheKey(string.Join(",", parsedUrns))}_list";
            var methodName = nameof(GetReportCardsAsync);

            var reportCards = await cacheService.GetOrAddAsync(cacheKey,
                async () => await reportCardsRepository.GetReportCardsAsync(parsedUrns), methodName);

            if (reportCards is null)
            {
                return [];
            }

            return reportCards.Select(reportCard => new ReportCardServiceModel
            {
                Urn = reportCard.Urn,
                LatestReportCard = MapToReportCardDetails(reportCard.LatestReportCard),
                PreviousReportCard = MapToReportCardDetails(reportCard.PreviousReportCard)
            }).ToList();
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

