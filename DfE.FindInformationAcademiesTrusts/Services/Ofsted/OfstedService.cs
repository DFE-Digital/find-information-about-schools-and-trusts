using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public interface IOfstedService
    {
        Task<OfstedOverviewInspectionServiceModel> GetOfstedOverviewInspectionAsync(int urn);
        Task<List<OfstedOverviewInspectionServiceModel>> GetOfstedOverviewInspectionForTrustAsync(string uid);
        Task<OlderSchoolOfstedServiceModel> GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(int urn);
        Task<List<TrustOfstedReportServiceModel<ReportCardServiceModel>>> GetEstablishmentsInTrustReportCardsAsync(string uid);
        Task<List<TrustOfstedReportServiceModel<OlderInspectionServiceModel>>> GetEstablishmentsInTrustOlderOfstedRatings(string uid);
        Task<List<TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>>> GetOfstedOverviewSafeguardingAndConcerns(string uid);
    }

    public class OfstedService(IReportCardsService reportCardsService, IOfstedRepository ofstedRepository, IOfstedServiceModelBuilder ofstedServiceModelBuilder, IAcademyService academyService, ILogger<IOfstedService> logger) : IOfstedService
    {
        public async Task<OfstedOverviewInspectionServiceModel> GetOfstedOverviewInspectionAsync(int urn)
        {
            var schoolOfstedRatings = await ofstedRepository.GetSchoolOfstedRatingsAsync(urn);
            var reportCards = await reportCardsService.GetReportCardsAsync(urn);

            return ofstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRatings, reportCards);
        }
        
        public async Task<List<OfstedOverviewInspectionServiceModel>> GetOfstedOverviewInspectionForTrustAsync(string uid)
        {
            var schoolOfstedRatings = await ofstedRepository.GetAcademiesInTrustOfstedAsync(uid);
            
            var reportCards = await reportCardsService.GetReportCardsAsync(schoolOfstedRatings.Select(x => x.Urn).ToList());

            var result = new List<OfstedOverviewInspectionServiceModel>();

            foreach (var schoolOfstedRating in schoolOfstedRatings)
            {
                int schoolUrn = int.Parse(schoolOfstedRating.Urn);
                var reportCard = reportCards.FirstOrDefault(x => x.Urn == schoolUrn) ?? new ReportCardServiceModel
                {
                    Urn = schoolUrn
                };

                var ofstedOverview = ofstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRating, reportCard);

                ofstedOverview.Urn = schoolUrn;
                ofstedOverview.SchoolName = schoolOfstedRating.EstablishmentName ?? "";
                ofstedOverview.DateJoinedTrust = schoolOfstedRating.DateAcademyJoinedTrust!.Value;

                result.Add(ofstedOverview);
            }

            return result;
        }

        public async Task<OlderSchoolOfstedServiceModel> GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(int urn)
        {
            var schoolOfstedRatings = await ofstedRepository.GetSchoolOfstedRatingsAsync(urn);

            return ofstedServiceModelBuilder.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfstedRatings);
        }

        public async Task<List<TrustOfstedReportServiceModel<ReportCardServiceModel>>> GetEstablishmentsInTrustReportCardsAsync(string uid)
        {
            var academiesInTrust = await academyService.GetAcademiesInTrustDetailsAsync(uid);

            var trustReportCards = new List<TrustOfstedReportServiceModel<ReportCardServiceModel>>();

            var reportCards = await reportCardsService.GetReportCardsAsync(academiesInTrust.Select(x => x.Urn).ToList());

            foreach (var reportCard in reportCards)
            {
                var academy = academiesInTrust.First(x => x.Urn == reportCard.Urn.ToString());

                var trustReportCard = new TrustOfstedReportServiceModel<ReportCardServiceModel>
                {
                    ReportDetails = reportCard,
                    Urn = reportCard.Urn,
                    SchoolName = academy.EstablishmentName ?? ""
                };

                trustReportCards.Add(trustReportCard);
            }

            return trustReportCards;
        }

        public async Task<List<TrustOfstedReportServiceModel<OlderInspectionServiceModel>>> GetEstablishmentsInTrustOlderOfstedRatings(string uid)
        {
            var academies = await ofstedRepository.GetAcademiesInTrustOfstedAsync(uid);
            
            var trustOfstedReports = new List<TrustOfstedReportServiceModel<OlderInspectionServiceModel>>();

            foreach (var schoolOfsted in academies)
            {
                if (int.TryParse(schoolOfsted.Urn, out var urn))
                {
                    var trustReportRating = new TrustOfstedReportServiceModel<OlderInspectionServiceModel>
                    {
                        ReportDetails = new OlderInspectionServiceModel
                        {
                            Ratings = [schoolOfsted.CurrentOfstedRating, schoolOfsted.PreviousOfstedRating]
                        },
                        Urn = urn,
                        SchoolName = schoolOfsted.EstablishmentName ?? ""
                    };

                    trustOfstedReports.Add(trustReportRating);
                }
                else
                {
                    logger.LogError("Unable to parse academy urn {Urn} for trust {Uid}", schoolOfsted.Urn, uid);
                }
            }

            return trustOfstedReports;
        }

        public async Task<List<TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>>> GetOfstedOverviewSafeguardingAndConcerns(string uid)
        {
            var schoolOfstedRatings = await ofstedRepository.GetAcademiesInTrustOfstedAsync(uid);

            var reportCards = await reportCardsService.GetReportCardsAsync(schoolOfstedRatings.Select(x => x.Urn).ToList());
            
            var result = new List<TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>>();

            foreach (var schoolOfstedRating in schoolOfstedRatings)
            {
                if (int.TryParse(schoolOfstedRating.Urn, out var urn))
                {
                    var reportCard = reportCards.FirstOrDefault(x => x.Urn == urn) ?? new ReportCardServiceModel
                    {
                        Urn = urn
                    };

                    var latestInspection = GetLatestSafeGuardingInspection(reportCard, schoolOfstedRating);

                    var safeGuarding = new TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>
                    {
                        Urn = urn,
                        SchoolName = schoolOfstedRating.EstablishmentName ?? "",
                        ReportDetails = latestInspection
                    };

                    result.Add(safeGuarding);
                }
                else
                {
                    logger.LogError("Unable to parse academy urn {Urn} for trust {Uid}", schoolOfstedRating.Urn, uid);
                }
            }

            return result;
        }

        private static SafeGuardingAndConcernsServiceModel GetLatestSafeGuardingInspection(ReportCardServiceModel reportCards, SchoolOfsted schoolOfstedRating)
        {
            var safeGuardingAndConcerns = new SafeGuardingAndConcernsServiceModel(schoolOfstedRating.DateAcademyJoinedTrust!.Value);

            if (TryGetSafeGuarding(safeGuardingAndConcerns, reportCards))
            {
                return safeGuardingAndConcerns;
            }

            return GetSafeGuardingFromOlderInspections(safeGuardingAndConcerns, schoolOfstedRating);
        }

        private static bool TryGetSafeGuarding(SafeGuardingAndConcernsServiceModel safeGuardingAndConcerns, ReportCardServiceModel reportCards)
        {

            var recentReportCard = reportCards.LatestReportCard;
            var previousReportCard = reportCards.PreviousReportCard;

            var latestInspection = recentReportCard;

            if (previousReportCard?.InspectionDate is { } prevDate &&
                recentReportCard?.InspectionDate is { } recentDate &&
                prevDate > recentDate)
            {
                latestInspection = previousReportCard;
            }

            if (latestInspection == null)
            {
                return false;
            }

            safeGuardingAndConcerns.Concerns = latestInspection.CategoryOfConcern;
            safeGuardingAndConcerns.SafeGuarding = latestInspection.Safeguarding;
            safeGuardingAndConcerns.InspectionDate = latestInspection.InspectionDate;

            return true;
        }

        private static SafeGuardingAndConcernsServiceModel GetSafeGuardingFromOlderInspections(SafeGuardingAndConcernsServiceModel safeGuardingAndConcerns, SchoolOfsted schoolOfstedRating)
        {
            var recent = schoolOfstedRating.CurrentOfstedRating;
            var previous = schoolOfstedRating.PreviousOfstedRating;

            var latestInspection = recent;

            if (previous.InspectionDate is { } prevDate &&
                recent.InspectionDate is { } recentDate &&
                prevDate > recentDate)
            {
                latestInspection = previous;
            }

            if (latestInspection.InspectionDate == null)
            {
                return safeGuardingAndConcerns;
            }

            safeGuardingAndConcerns.Concerns = latestInspection.CategoryOfConcern.ToDisplayString();
            safeGuardingAndConcerns.SafeGuarding = latestInspection.SafeguardingIsEffective.ToDisplayString();
            safeGuardingAndConcerns.InspectionDate = DateOnly.FromDateTime(latestInspection.InspectionDate.Value);

            return safeGuardingAndConcerns;
        }
    }
}
