using System.Globalization;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services
{
    using DfE.FindInformationAcademiesTrusts.Data;
    using DfE.FindInformationAcademiesTrusts.Data.Enums;
    using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
    using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

    public class OfstedServiceModelBuilderTests
    {
        private readonly OfstedServiceModelBuilder _sut;
        private const int urn = 123456;

        public OfstedServiceModelBuilderTests()
        {
            _sut = new OfstedServiceModelBuilder();
        }


        [Theory]
        [InlineData("2024-07-01", "2024-08-01", BeforeOrAfterJoining.Before)]
        [InlineData("2024-08-02", "2024-08-01", BeforeOrAfterJoining.After)]
        public void BuildOfstedOverviewInspection_ForShortInspection_ShouldSetCorrectDetails(DateTime inspectionDate, DateTime dateJoined, BeforeOrAfterJoining expectedBeforeOrAfterJoining)
        {

            var shortInspectionData = new OfstedShortInspection(inspectionDate, "School remains Good");

           var result = _sut.BuildOfstedOverviewInspection(new SchoolOfsted(urn.ToString(), "Academy 1", dateJoined,
                    shortInspectionData,
                    new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local)),
                    new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local)), false),
               new ReportCardServiceModel());


            result.ShortInspection.Should().NotBeNull();
            result.ShortInspection!.InspectionDate.Should().Be(DateOnly.FromDateTime(inspectionDate));
            result.ShortInspection!.InspectionOutcome.Should().Be("School remains Good");
            result.ShortInspection!.IsReportCard.Should().BeFalse();
            result.ShortInspection!.BeforeOrAfterJoining.Should().Be(expectedBeforeOrAfterJoining);
        }


        [Fact]
        public void BuildOfstedOverviewInspection_IfNoData_ShouldReturnEmptyModel()
        {
            var result = _sut.BuildOfstedOverviewInspection(new SchoolOfsted(urn.ToString(), null, null,
                new OfstedShortInspection(null, null),
                new OfstedRating(null, null),
                new OfstedRating(null, null), false), new ReportCardServiceModel());

            result.Should().NotBeNull();
            result.ShortInspection.Should().BeNull();
            result.Current.Should().BeNull();
            result.Previous.Should().BeNull();
        }

        [Fact]
        public void GetOfstedOverviewInspectionAsync_SetsIsReportCardCorrectly()
        {
            var shortInspectionDate = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Local);

            var dateForOlderReport = new DateTime(2022, 11, 1, 0, 0, 0, DateTimeKind.Local);
            var dateForReportCard = new DateOnly(2025, 1, 20);

            var schoolOfstedRatings = new SchoolOfsted(

                urn.ToString(),
                "Test School",
                new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(shortInspectionDate, "Good"),
                new OfstedRating(null, null),
                new OfstedRating((int)OfstedRatingScore.Outstanding, dateForOlderReport),
                false
            );

            var reportCards = new ReportCardServiceModel
            {
                LatestReportCard = new ReportCardDetails(dateForReportCard, null, null, null, null, null, null, null,
                    null, "Met", null, "None"),
                PreviousReportCard = null
            };

            var result = _sut.BuildOfstedOverviewInspection(schoolOfstedRatings, reportCards);

            result.Current.Should().NotBeNull();
            result.Previous.Should().NotBeNull();
            result.ShortInspection.Should().NotBeNull();

            result.Current!.InspectionDate.Should().Be(dateForReportCard);
            result.Current.IsReportCard.Should().BeTrue();

            result.Previous!.InspectionDate.Should().Be(DateOnly.FromDateTime(dateForOlderReport));
            result.Previous.IsReportCard.Should().BeFalse();
        }


        [Theory]
        [InlineData("2023-01-01", "2022-01-01", "2024-01-01", "2023-06-01")]
        [InlineData("2023-01-02", "2023-01-03", "2023-01-04", "2023-01-05")]
        [InlineData("2023-01-05", "2023-01-04", "2023-01-03", "2023-01-02")]
        public void BuildOfstedOverviewInspection_ReturnsExpectedOverviewModel(DateTime olderPreviousDate,
            DateTime olderCurrentDate, DateTime reportCardPreviousDate, DateTime reportCardLatestDate)
        {
            var dateAcademyJoinedTrust = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var shortInspectionDate = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Local);

            var schoolOfstedRatings = new SchoolOfsted(

                urn.ToString(),
                "Test School",
                dateAcademyJoinedTrust,
                new OfstedShortInspection(shortInspectionDate, "Good"),
                new OfstedRating((int)OfstedRatingScore.Good, olderPreviousDate),
                new OfstedRating((int)OfstedRatingScore.Outstanding, olderCurrentDate),
                false
            );

            var reportCards = new ReportCardServiceModel
            {
                LatestReportCard = new ReportCardDetails(DateOnly.FromDateTime(reportCardLatestDate), null, null, null,
                    null, null, null, null,
                    null, "Met", null, "None"),
                PreviousReportCard = new ReportCardDetails(DateOnly.FromDateTime(reportCardPreviousDate), null, null,
                    null, null, null, null,
                    null, null, "Met", null, "None")
            };

            var result = _sut.BuildOfstedOverviewInspection(schoolOfstedRatings, reportCards);

            // Assert
            result.Should().NotBeNull();
            result.Current.Should().NotBeNull();
            result.Previous.Should().NotBeNull();
            result.ShortInspection.Should().NotBeNull();

            // Check ordering by InspectionDate descending
            var expectedDates = new List<DateOnly>
            {
                reportCards.LatestReportCard.InspectionDate,
                reportCards.PreviousReportCard.InspectionDate,
                DateOnly.FromDateTime(schoolOfstedRatings.CurrentOfstedRating.InspectionDate!.Value),
                DateOnly.FromDateTime(schoolOfstedRatings.PreviousOfstedRating.InspectionDate!.Value)
            }.OrderByDescending(x => x).ToList();

            result.Current!.InspectionDate.Should().Be(expectedDates[0]);
            result.Previous!.InspectionDate.Should().Be(expectedDates[1]);

            result.ShortInspection!.InspectionDate.Should().Be(DateOnly.FromDateTime(shortInspectionDate));
        }

        [Fact]
        public void BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade_AssignsAllPropertiesCorrectly()
        {
            var shortInspection = new OfstedShortInspection(null, null);
            var currentRating = new OfstedRating(null, new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local));
            var previousRating = new OfstedRating(null, new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Local));

            var schoolOfsted = new SchoolOfsted("123456", 
                "Test School", 
                new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local),
                shortInspection,
                currentRating,
                previousRating, true
            );

            var result = _sut.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfsted);

            // Assert
            result.Urn.Should().Be("123456");
            result.EstablishmentName.Should().Be("Test School");
            result.DateAcademyJoinedTrust.Should().Be(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local));
            result.ShortInspection.Should().Be(shortInspection);
            result.IsFurtherEducationalEstablishment.Should().BeTrue();
        }

        [Theory]
        [InlineData("2024-09-01", true, false)]
        [InlineData("2024-09-02", true, false)]
        [InlineData("2024-09-03", false, true)]
        [InlineData("2023-12-31", true, false)]
        [InlineData("2025-01-01", false, true)] 
        public void BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade_SplitsRatingsByPolicyChangeDate(
            string inspectionDateString, bool expectedInWithSingle, bool expectedInWithout)
        {
            var inspectionDate = DateTime.Parse(inspectionDateString, new CultureInfo("en-GB"));
            var rating = new OfstedRating(null, inspectionDate);

            var schoolOfsted = new SchoolOfsted("123456",
                "Test School",
                new DateTime(2020, 1, 1, 0 ,0, 0, DateTimeKind.Local),
                new OfstedShortInspection(null, null),
                rating,
                new OfstedRating(null, null),
                false
            );

            var result = _sut.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfsted);

            if (expectedInWithSingle)
            {
                result.RatingsWithSingleHeadlineGrade.Should().ContainSingle();
                result.RatingsWithSingleHeadlineGrade.First().Should().Be(rating);
                result.RatingsWithoutSingleHeadlineGrade.Should().BeEmpty();
            }

            if (expectedInWithout)
            {
                result.RatingsWithoutSingleHeadlineGrade.Should().ContainSingle();
                result.RatingsWithoutSingleHeadlineGrade.First().Should().Be(rating);
                result.RatingsWithSingleHeadlineGrade.Should().BeEmpty();
            }
        }

        [Fact]
        public void BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade_OrdersRatingsByInspectionDateDescending()
        {
            // Arrange
            var olderRating = new OfstedRating(null, new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local));
            var newerRating = new OfstedRating(null, new DateTime(2024, 8, 15, 0, 0, 0, DateTimeKind.Local));

            var schoolOfsted = new SchoolOfsted("123456",
                "Test School",
                new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(null, null),
                newerRating, // newer rating as older
                olderRating,
                false);

            var result = _sut.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfsted);

            // Assert
            result.RatingsWithSingleHeadlineGrade.Should().HaveCount(2);
            result.RatingsWithSingleHeadlineGrade[0].Should().Be(newerRating); // Newer should be first
            result.RatingsWithSingleHeadlineGrade[1].Should().Be(olderRating); // Older should be second
        }

        [Fact]
        public void BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade_HandlesBothRatingsCrossingPolicyDate()
        {
            var beforePolicyRating = new OfstedRating( null, new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Local));
            var afterPolicyRating = new OfstedRating( null, new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Local));

            var schoolOfsted = new SchoolOfsted("123456",
                "Test School",
                new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(null, null),
                beforePolicyRating,
                afterPolicyRating,
                false);

            var result = _sut.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfsted);

            result.RatingsWithSingleHeadlineGrade.Should().ContainSingle();
            result.RatingsWithSingleHeadlineGrade.First().Should().Be(beforePolicyRating);

            result.RatingsWithoutSingleHeadlineGrade.Should().ContainSingle();
            result.RatingsWithoutSingleHeadlineGrade.First().Should().Be(afterPolicyRating);
        }

        [Fact]
        public void BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade_HandlesNullRatings()
        {
            var schoolOfsted = new SchoolOfsted("123456",
                "Test School",
                new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(null, null),
                new OfstedRating(null, null),
                new OfstedRating(null, null),
                false);

            var result = _sut.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfsted);

            result.RatingsWithSingleHeadlineGrade.Should().BeEmpty();
            result.RatingsWithoutSingleHeadlineGrade.Should().BeEmpty();
        }

        [Fact]
        public void BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade_HandlesOnlyCurrentRating()
        {
            var currentRating = new OfstedRating(null, new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local));

            var schoolOfsted = new SchoolOfsted("123456",
                "Test School",
                new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(null, null),
                new OfstedRating(null, null),
                currentRating,
                false);

            var result = _sut.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfsted);

            result.RatingsWithSingleHeadlineGrade.Should().ContainSingle();
            result.RatingsWithSingleHeadlineGrade.First().Should().Be(currentRating);
            result.RatingsWithoutSingleHeadlineGrade.Should().BeEmpty();
        }

        [Fact]
        public void BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade_HandlesOnlyPreviousRating()
        {
            var previous = new OfstedRating(null, new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Local));

            var schoolOfsted = new SchoolOfsted("123456",
                "Test School",
                new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(null, null),
                previous,
                new OfstedRating(null, null),
                false);

            var result = _sut.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfsted);

            result.RatingsWithSingleHeadlineGrade.Should().BeEmpty();
            result.RatingsWithoutSingleHeadlineGrade.Should().ContainSingle();
            result.RatingsWithoutSingleHeadlineGrade.First().Should().Be(previous);
        }

        [Fact]
        public void BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade_HandlesNullDateAcademyJoinedTrust()
        {
            var rating = new OfstedRating(null, new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local));

            var schoolOfsted = new SchoolOfsted("123456",
                "Test School",
                null,
                new OfstedShortInspection(null, null),
                rating,
                new OfstedRating(null, null),
                false
            );

            var result = _sut.BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(schoolOfsted);

            result.DateAcademyJoinedTrust.Should().BeNull();
        }
    }
}
