using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public interface IOfstedServiceModelBuilder
    {
        OfstedOverviewInspectionServiceModel BuildOfstedOverviewInspection(SchoolOfsted schoolOfstedRatings, ReportCardServiceModel reportCards);

        OlderSchoolOfstedServiceModel BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(SchoolOfsted schoolOfstedRatings);
    }

    public class OfstedServiceModelBuilder : IOfstedServiceModelBuilder
    {
        public OfstedOverviewInspectionServiceModel BuildOfstedOverviewInspection(SchoolOfsted schoolOfstedRatings, ReportCardServiceModel reportCards)
        {
            var overviewModels = new List<OverviewServiceModel>();

            void AddOfstedRating(OfstedRating? rating)
            {
                if (rating?.InspectionDate is { } inspectionDate)
                {
                    overviewModels.Add(new OverviewServiceModel
                    {
                        IsReportCard = false,
                        InspectionDate = DateOnly.FromDateTime(inspectionDate),
                        BeforeOrAfterJoining = schoolOfstedRatings.DateAcademyJoinedTrust
                            .GetBeforeOrAfterJoiningTrust(inspectionDate)
                    });
                }
            }

            void AddReportCard(ReportCardDetails? reportCard)
            {
                if (reportCard != null)
                {
                    overviewModels.Add(new OverviewServiceModel
                    {
                        IsReportCard = true,
                        InspectionDate = reportCard.InspectionDate,
                        BeforeOrAfterJoining = schoolOfstedRatings.DateAcademyJoinedTrust
                            .GetBeforeOrAfterJoiningTrust(reportCard.InspectionDate)
                    });
                }
            }

            AddReportCard(reportCards.LatestReportCard);
            AddReportCard(reportCards.PreviousReportCard);
            AddOfstedRating(schoolOfstedRatings.CurrentOfstedRating);
            AddOfstedRating(schoolOfstedRatings.PreviousOfstedRating);

            var orderedOverviewModels = overviewModels
                .OrderByDescending(x => x.InspectionDate)
                .ToList();

            var shortInspectionModel = GetShortInspectionModel(
                schoolOfstedRatings.ShortInspection,
                schoolOfstedRatings.DateAcademyJoinedTrust
            );

            return new OfstedOverviewInspectionServiceModel(
                orderedOverviewModels.FirstOrDefault(),
                orderedOverviewModels.Skip(1).FirstOrDefault(),
                shortInspectionModel
            );
        }

        private static ShortInspectionOverviewServiceModel? GetShortInspectionModel(OfstedShortInspection ofstedShortInspection, DateTime? dateAcademyJoinedTrust)
        {
            if (ofstedShortInspection.InspectionDate is null)
            {
                return null;
            }

            return new ShortInspectionOverviewServiceModel
            {
                IsReportCard = false,
                InspectionDate = DateOnly.FromDateTime(ofstedShortInspection.InspectionDate.Value),
                BeforeOrAfterJoining =
                    dateAcademyJoinedTrust.GetBeforeOrAfterJoiningTrust(ofstedShortInspection.InspectionDate),
                InspectionOutcome = ofstedShortInspection.ToOutcomeDisplayString()
            };
        }

        public OlderSchoolOfstedServiceModel BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(SchoolOfsted schoolOfstedRatings)
        {
            var (withSingleHeadlineGrade, withoutSingleHeadlineGrade) = GetOlderBeforeAndAfter(schoolOfstedRatings);

            return new OlderSchoolOfstedServiceModel(
                schoolOfstedRatings.Urn,
                schoolOfstedRatings.EstablishmentName,
                schoolOfstedRatings.DateAcademyJoinedTrust,
                schoolOfstedRatings.ShortInspection,
                withSingleHeadlineGrade,
                withoutSingleHeadlineGrade,
                schoolOfstedRatings.IsFurtherEducationalEstablishment
            );
        }

        private static (List<OfstedRating> withSingleHeadlineGrade, List<OfstedRating> withoutSingleHeadlineGrade) GetOlderBeforeAndAfter(SchoolOfsted schoolOfstedRatings)
        {
            DateTime SingleHeadlineGradesPolicyChangeDate = new(2024, 09, 02, 0, 0, 0, DateTimeKind.Utc);

            List<OfstedRating> ofstedRatings =
                [schoolOfstedRatings.CurrentOfstedRating, schoolOfstedRatings.PreviousOfstedRating];

            ofstedRatings = ofstedRatings.OrderByDescending(x => x.InspectionDate).ToList();

            var withoutSingleHeadlineGrade = ofstedRatings.Where(x => x.InspectionDate > SingleHeadlineGradesPolicyChangeDate).ToList();
            var withSingleHeadlineGrade = ofstedRatings.Where(x => x.InspectionDate <= SingleHeadlineGradesPolicyChangeDate).ToList();

            return (withSingleHeadlineGrade, withoutSingleHeadlineGrade);
        }
    }
}
