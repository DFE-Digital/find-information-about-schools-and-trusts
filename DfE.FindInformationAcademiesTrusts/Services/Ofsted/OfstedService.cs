using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public interface IOfstedService
    {
        Task<OfstedOverviewInspectionServiceModel> GetOfstedOverviewInspectionAsync(int urn);

        Task<List<OfstedOverviewInspectionServiceModel>> GetOfstedOverviewInspectionForTrustAsync(
            string trustReferenceNumber);

        Task<OlderSchoolOfstedServiceModel> GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(int urn);

        Task<List<TrustOfstedReportServiceModel<ReportCardServiceModel>>>
            GetEstablishmentsInTrustReportCardsAsync(string uid);

        Task<List<TrustOfstedReportServiceModel<OlderInspectionServiceModel>>>
            GetEstablishmentsInTrustOlderOfstedRatings(string trustReferenceNumber);

        Task<List<TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>>>
            GetOfstedOverviewSafeguardingAndConcerns(string trustReferenceNumber);
    }

    public class OfstedService(
        IReportCardsService reportCardsService,
        IOfstedServiceModelBuilder ofstedServiceModelBuilder,
        IAcademyService academyService,
        IOfstedHelperService ofstedHelperService,
        ILogger<OfstedService> logger) : IOfstedService
    {
        public async Task<OfstedOverviewInspectionServiceModel> GetOfstedOverviewInspectionAsync(int urn)
        {
            var schoolOfstedRatings = await ofstedHelperService.GetSchoolOfstedAsync(urn);
            var reportCards = await reportCardsService.GetReportCardsAsync(urn);

            return ofstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRatings, reportCards);
        }

        public async Task<List<OfstedOverviewInspectionServiceModel>> GetOfstedOverviewInspectionForTrustAsync(
            string trustReferenceNumber)
        {
            var schoolOfstedRatings = await ofstedHelperService.GetAcademiesOfstedAsync(trustReferenceNumber);

            var reportCards =
                await reportCardsService.GetReportCardsAsync(schoolOfstedRatings.Select(x => x.Urn).ToList());

            var result = new List<OfstedOverviewInspectionServiceModel>();

            foreach (var schoolOfstedRating in schoolOfstedRatings)
            {
                int schoolUrn = int.Parse(schoolOfstedRating.Urn);
                var reportCard = reportCards.FirstOrDefault(x => x.Urn == schoolUrn) ?? new ReportCardServiceModel
                {
                    Urn = schoolUrn
                };

                var ofstedOverview =
                    ofstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRating, reportCard);

                ofstedOverview.Urn = schoolUrn;
                ofstedOverview.SchoolName = schoolOfstedRating.EstablishmentName ?? "";
                ofstedOverview.DateJoinedTrust = schoolOfstedRating.DateEstablishmentJoinedGroup!.Value;

                result.Add(ofstedOverview);
            }

            return result;
        }

        public async Task<OlderSchoolOfstedServiceModel>
            GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(int urn)
        {
            var schoolOfstedRatings = await ofstedHelperService.GetSchoolOfstedAsync(urn);

            return ofstedServiceModelBuilder
                .BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfstedRatings);
        }

        public async Task<List<TrustOfstedReportServiceModel<ReportCardServiceModel>>>
            GetEstablishmentsInTrustReportCardsAsync(string uid)
        {
            var academiesInTrust = await academyService.GetAcademiesInTrustDetailsAsync(uid);

            var trustReportCards = new List<TrustOfstedReportServiceModel<ReportCardServiceModel>>();

            var reportCards =
                await reportCardsService.GetReportCardsAsync(academiesInTrust.Select(x => x.Urn).ToList());

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

        public async Task<List<TrustOfstedReportServiceModel<OlderInspectionServiceModel>>>
            GetEstablishmentsInTrustOlderOfstedRatings(string trustReferenceNumber)
        {
            var academies = await ofstedHelperService.GetAcademiesOfstedAsync(trustReferenceNumber);

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
                    logger.LogError("Unable to parse academy urn {Urn} for trust {Trn}", schoolOfsted.Urn,
                        trustReferenceNumber);
                }
            }

            return trustOfstedReports;
        }

        public async Task<List<TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>>>
            GetOfstedOverviewSafeguardingAndConcerns(string trustReferenceNumber)
        {
            var schoolOfstedRatings = await ofstedHelperService.GetAcademiesOfstedAsync(trustReferenceNumber);

            var reportCards =
                await reportCardsService.GetReportCardsAsync(schoolOfstedRatings.Select(x => x.Urn).ToList());

            var result = new List<TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>>();

            foreach (var schoolOfstedRating in schoolOfstedRatings)
            {
                if (int.TryParse(schoolOfstedRating.Urn, out var urn))
                {
                    var reportCard = reportCards.FirstOrDefault(x => x.Urn == urn) ?? new ReportCardServiceModel
                    {
                        Urn = urn
                    };

                    var latestInspection = ofstedHelperService.GetLatestSafeGuardingInspection(reportCard, schoolOfstedRating);

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
                    logger.LogError("Unable to parse academy urn {Urn} for trust {Trn}", schoolOfstedRating.Urn,
                        trustReferenceNumber);
                }
            }

            return result;
        }
    }
}
