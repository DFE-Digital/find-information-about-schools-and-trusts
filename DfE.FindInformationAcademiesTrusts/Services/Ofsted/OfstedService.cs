using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
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

            var result = new List<OfstedOverviewInspectionServiceModel>();

            foreach (var schoolOfstedRating in schoolOfstedRatings)
            {
                int schoolUrn = int.Parse(schoolOfstedRating.Urn);

                var reportCards = await reportCardsService.GetReportCardsAsync(schoolUrn);

                var ofstedOverview = ofstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRating, reportCards);

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

           foreach (var academy in academiesInTrust)
           {
               if (int.TryParse(academy.Urn, out var urn))
               {
                   var reportCard = await reportCardsService.GetReportCardsAsync(urn);

                   var trustReportCard = new TrustOfstedReportServiceModel<ReportCardServiceModel>
                   {
                       ReportDetails = reportCard,
                       Urn = urn,
                       SchoolName = academy.EstablishmentName ?? ""
                   };

                   trustReportCards.Add(trustReportCard);
               }
               else
               {
                   logger.LogError("Unable to parse academy urn {Urn} for trust {Uid}", academy.Urn, uid);
               }

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
                    var trustReportCard = new TrustOfstedReportServiceModel<OlderInspectionServiceModel>
                    {
                        ReportDetails = new OlderInspectionServiceModel
                        {
                            Ratings = [schoolOfsted.CurrentOfstedRating, schoolOfsted.PreviousOfstedRating]
                        },
                        Urn = urn,
                        SchoolName = schoolOfsted.EstablishmentName ?? ""
                    };

                    trustOfstedReports.Add(trustReportCard);
                }
                else
                {
                    logger.LogError("Unable to parse academy urn {Urn} for trust {Uid}", schoolOfsted.Urn, uid);
                }
            }

            return trustOfstedReports;
        }
    }
}
