using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public interface IOfstedService
    {
        Task<OfstedOverviewInspectionServiceModel> GetOfstedOverviewInspectionAsync(int urn);
        Task<List<OfstedOverviewInspectionServiceModel>> GetOfstedOverviewInspectionForTrustAsync(string uid);
        Task<OlderSchoolOfstedServiceModel> GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(int urn);
    }

    public class OfstedService(IReportCardsService reportCardsService, IOfstedRepository ofstedRepository, IOfstedServiceModelBuilder ofstedServiceModelBuilder) : IOfstedService
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
    }
}
