namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services
{
    using DfE.FindInformationAcademiesTrusts.Data;
    using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
    using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
    using DfE.FindInformationAcademiesTrusts.Services.School;

    public class OfstedServiceTests
    {
        private readonly OfstedService _sut;
        private readonly IOfstedRepository _mockOfstedRepository = Substitute.For<IOfstedRepository>();
        private readonly IReportCardsService _mockReportCardsService = Substitute.For<IReportCardsService>();
        private readonly IOfstedServiceModelBuilder _mockOfstedServiceModelBuilder = Substitute.For<IOfstedServiceModelBuilder>();

        public OfstedServiceTests()
        {
            _sut = new OfstedService(_mockReportCardsService, _mockOfstedRepository, _mockOfstedServiceModelBuilder);

            _mockReportCardsService.GetReportCardsAsync(Arg.Any<int>()).Returns(new ReportCardServiceModel());
        }


        [Fact]
        public async Task GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync_Should_CallBuilderWithCorrectParameters()
        {
            const int urn = 123456;

            var expectedSchoolOfsted = new SchoolOfsted(urn.ToString(), "Academy 1", new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local)),
                new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local)), false);

            _mockOfstedRepository.GetSchoolOfstedRatingsAsync(urn)
                .Returns(expectedSchoolOfsted);


            await _sut.GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(urn);

            _mockOfstedServiceModelBuilder.Received(1)
                .BuildSchoolOfstedRatingsAsBeforeAndAfterSeptemberGrade(expectedSchoolOfsted);
        }

        [Fact]
        public async Task GetOfstedOverviewInspectionAsync_ShouldGetDataAndCallBuilder()
        {
            const int urn = 123456;

            var expectedSchoolOfsted = new SchoolOfsted(urn.ToString(), "Academy 1", new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local)),
                new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local)), false);

            var expectedReportCards = new ReportCardServiceModel
            {
                DateJoinedTrust = new DateOnly(2024, 8, 1),
                LatestReportCard = null,
                PreviousReportCard = null
            };

            _mockOfstedRepository.GetSchoolOfstedRatingsAsync(urn)
                .Returns(expectedSchoolOfsted);
            _mockReportCardsService.GetReportCardsAsync(urn)
                .Returns(expectedReportCards);

            await _sut.GetOfstedOverviewInspectionAsync(urn);

            _mockOfstedServiceModelBuilder.Received(1)
                .BuildOfstedOverviewInspection(expectedSchoolOfsted, expectedReportCards);

        }

        [Fact]
        public async Task GetOfstedOverviewInspectionForTrustAsync_ShouldReturnEmptyList_WhenNoSchoolsInTrust()
        {
            const string trustUid = "1234";
            var emptySchoolOfstedRatings = Array.Empty<SchoolOfsted>();

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns(emptySchoolOfstedRatings);

            var result = await _sut.GetOfstedOverviewInspectionForTrustAsync(trustUid);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetOfstedOverviewInspectionForTrustAsync_ShouldReturnCorrectData_WhenMultipleSchoolsInTrust()
        {
            // Arrange
            const string trustUid = "1234";
            const int schoolUrn1 = 123456;
            const int schoolUrn2 = 789012;

            var schoolOfstedRating1 = new SchoolOfsted(
                schoolUrn1.ToString(),
                "First Academy",
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local)),
                new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local)),
                false);

            var schoolOfstedRating2 = new SchoolOfsted(
                schoolUrn2.ToString(),
                "Second Academy",
                new DateTime(2023, 9, 15, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2023, 8, 15, 0, 0, 0, DateTimeKind.Local), "School remains Outstanding"),
                new OfstedRating((int)OfstedRatingScore.Outstanding, new DateTime(2024, 8, 15, 0, 0, 0, DateTimeKind.Local)),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2023, 7, 15, 0, 0, 0, DateTimeKind.Local)),
                false);

            var reportCards1 = new ReportCardServiceModel();
            var reportCards2 = new ReportCardServiceModel();

            var expectedOfstedOverview1 = new OfstedOverviewInspectionServiceModel(null, null, null);
            var expectedOfstedOverview2 = new OfstedOverviewInspectionServiceModel(null, null, null);

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns([schoolOfstedRating1, schoolOfstedRating2]);

            _mockReportCardsService.GetReportCardsAsync(schoolUrn1).Returns(reportCards1);
            _mockReportCardsService.GetReportCardsAsync(schoolUrn2).Returns(reportCards2);

            _mockOfstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRating1, reportCards1)
                .Returns(expectedOfstedOverview1);
            _mockOfstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRating2, reportCards2)
                .Returns(expectedOfstedOverview2);

            var result = await _sut.GetOfstedOverviewInspectionForTrustAsync(trustUid);

            result.Should().HaveCount(2);

            result[0].Urn.Should().Be(schoolUrn1);
            result[0].SchoolName.Should().Be("First Academy");
            result[0].DateJoinedTrust.Should().Be(new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local));

            result[1].Urn.Should().Be(schoolUrn2);
            result[1].SchoolName.Should().Be("Second Academy");
            result[1].DateJoinedTrust.Should().Be(new DateTime(2023, 9, 15, 0, 0, 0, DateTimeKind.Local));
        }

        [Fact]
        public async Task GetOfstedOverviewInspectionForTrustAsync_ShouldHandleNullEstablishmentName()
        {
            const string trustUid = "1234";
            const int schoolUrn = 123456;

            var schoolOfstedRating = new SchoolOfsted(
                schoolUrn.ToString(),
                null, // null establishment name
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local)),
                new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local)),
                false);

            var reportCards = new ReportCardServiceModel();
            var expectedOfstedOverview = new OfstedOverviewInspectionServiceModel(null, null, null);

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns([schoolOfstedRating]);

            _mockReportCardsService.GetReportCardsAsync(schoolUrn).Returns(reportCards);
            _mockOfstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRating, reportCards)
                .Returns(expectedOfstedOverview);

            var result = await _sut.GetOfstedOverviewInspectionForTrustAsync(trustUid);

            // Assert
            result.Should().HaveCount(1);
            result[0].SchoolName.Should().Be(string.Empty);
        }

        [Fact]
        public async Task GetOfstedOverviewInspectionForTrustAsync_ShouldCallDependenciesWithCorrectParameters()
        {
            const string trustUid = "1234";
            const int schoolUrn = 123456;

            var schoolOfstedRating = new SchoolOfsted(
                schoolUrn.ToString(),
                "Test Academy",
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local)),
                new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local)),
                false);

            var reportCards = new ReportCardServiceModel();
            var expectedOfstedOverview = new OfstedOverviewInspectionServiceModel(null, null, null);

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns([schoolOfstedRating]);

            _mockReportCardsService.GetReportCardsAsync(schoolUrn).Returns(reportCards);
            _mockOfstedServiceModelBuilder.BuildOfstedOverviewInspection(schoolOfstedRating, reportCards)
                .Returns(expectedOfstedOverview);

            await _sut.GetOfstedOverviewInspectionForTrustAsync(trustUid);

            await _mockOfstedRepository.Received(1).GetAcademiesInTrustOfstedAsync(trustUid);
            await _mockReportCardsService.Received(1).GetReportCardsAsync(schoolUrn);
            _mockOfstedServiceModelBuilder.Received(1).BuildOfstedOverviewInspection(schoolOfstedRating, reportCards);
        }
    }
}
