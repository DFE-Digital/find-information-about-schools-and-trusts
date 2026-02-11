using DfE.FindInformationAcademiesTrusts.Services.Academy;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services
{
    using DfE.FindInformationAcademiesTrusts.Data;
    using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
    using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
    using DfE.FindInformationAcademiesTrusts.Services.School;
    using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;
    using NSubstitute;

    public class OfstedServiceTests
    {
        private readonly OfstedService _sut;
        private readonly IOfstedRepository _mockOfstedRepository = Substitute.For<IOfstedRepository>();
        private readonly IReportCardsService _mockReportCardsService = Substitute.For<IReportCardsService>();
        private readonly IOfstedServiceModelBuilder _mockOfstedServiceModelBuilder = Substitute.For<IOfstedServiceModelBuilder>();
        private readonly IAcademyService _mockAcademyService = Substitute.For<IAcademyService>();
        private readonly ILogger<IOfstedService> _mockLogger = Substitute.For<ILogger<IOfstedService>>();

        public OfstedServiceTests()
        {
            _sut = new OfstedService(_mockReportCardsService, _mockOfstedRepository, _mockOfstedServiceModelBuilder, _mockAcademyService, _mockLogger);

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

        [Fact]
        public async Task GetEstablishmentsInTrustReportCardsAsync_should_return_empty_list_when_no_academies_found()
        {
            const string uid = "TEST123";
            _mockAcademyService.GetAcademiesInTrustDetailsAsync(uid).Returns([]);

            var result = await _sut.GetEstablishmentsInTrustReportCardsAsync(uid);

            result.Should().BeEmpty();
            await _mockReportCardsService.DidNotReceive().GetReportCardsAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task GetEstablishmentsInTrustReportCardsAsync_should_return_trust_report_cards_for_single_academy()
        {
            const string uid = "TEST123";
            const string urn = "12345";
            const string establishmentName = "Test Academy";
            var joinDate = new DateOnly(2020, 9, 1);

            var academies = new List<AcademyDetailsServiceModel>
            {
                new(urn, establishmentName, "Test LA", "Academy", "Urban", joinDate)
            };

            var reportCard = new ReportCardServiceModel
            {
                LatestReportCard = new ReportCardDetails(
                    new DateOnly(2023, 1, 15),
                    "https://reports.ofsted.gov.uk/provider/files/12345/urn/12345.pdf",
                    "Good", "Good", "Good", "Good", "Good", "Good", null, "Met", "Good", "None")
            };

            _mockAcademyService.GetAcademiesInTrustDetailsAsync(uid).Returns(academies.ToArray());
            _mockReportCardsService.GetReportCardsAsync(12345).Returns(reportCard);

            var result = await _sut.GetEstablishmentsInTrustReportCardsAsync(uid);

            result.Should().HaveCount(1);
            var trustReportCard = result[0];
            trustReportCard.Urn.Should().Be(12345);
            trustReportCard.SchoolName.Should().Be(establishmentName);
            trustReportCard.ReportDetails.Should().Be(reportCard);

            await _mockAcademyService.Received(1).GetAcademiesInTrustDetailsAsync(uid);
            await _mockReportCardsService.Received(1).GetReportCardsAsync(12345);
        }

        [Fact]
        public async Task GetEstablishmentsInTrustReportCardsAsync_should_return_trust_report_cards_for_multiple_academies()
        {
            const string uid = "TEST123";
            var joinDate = new DateOnly(2020, 9, 1);

            var academies = new List<AcademyDetailsServiceModel>
            {
                new("11111", "First Academy", "Test LA", "Academy", "Urban", joinDate),
                new("22222", "Second Academy", "Test LA", "Academy", "Rural", joinDate),
                new("33333", "Third Academy", "Test LA", "Academy", "Urban", joinDate)
            };

            var reportCard1 = new ReportCardServiceModel { LatestReportCard = new ReportCardDetails(new DateOnly(2023, 1, 15), "https://example.com/1", "Good", "Good", "Good", "Good", "Good", "Good", null, "Met", "Good", "None") };
            var reportCard2 = new ReportCardServiceModel { LatestReportCard = new ReportCardDetails(new DateOnly(2023, 2, 15), "https://example.com/2", "Outstanding", "Outstanding", "Good", "Outstanding", "Good", "Good", null, "Met", "Good", "None") };
            var reportCard3 = new ReportCardServiceModel { LatestReportCard = new ReportCardDetails(new DateOnly(2023, 3, 15), "https://example.com/3", "Requires Improvement", "Good", "Good", "Requires Improvement", "Good", "Good", null, "Met", "Good", "None") };

            _mockAcademyService.GetAcademiesInTrustDetailsAsync(uid).Returns(academies.ToArray());
            _mockReportCardsService.GetReportCardsAsync(11111).Returns(reportCard1);
            _mockReportCardsService.GetReportCardsAsync(22222).Returns(reportCard2);
            _mockReportCardsService.GetReportCardsAsync(33333).Returns(reportCard3);

            var result = await _sut.GetEstablishmentsInTrustReportCardsAsync(uid);

            result.Should().HaveCount(3);

            result[0].Urn.Should().Be(11111);
            result[0].SchoolName.Should().Be("First Academy");
            result[0].ReportDetails.Should().Be(reportCard1);

            result[1].Urn.Should().Be(22222);
            result[1].SchoolName.Should().Be("Second Academy");
            result[1].ReportDetails.Should().Be(reportCard2);

            result[2].Urn.Should().Be(33333);
            result[2].SchoolName.Should().Be("Third Academy");
            result[2].ReportDetails.Should().Be(reportCard3);

            await _mockAcademyService.Received(1).GetAcademiesInTrustDetailsAsync(uid);
            await _mockReportCardsService.Received(1).GetReportCardsAsync(11111);
            await _mockReportCardsService.Received(1).GetReportCardsAsync(22222);
            await _mockReportCardsService.Received(1).GetReportCardsAsync(33333);
        }

        [Fact]
        public async Task GetEstablishmentsInTrustReportCardsAsync_should_handle_null_establishment_name()
        {
            const string uid = "TEST123";
            const string urn = "12345";
            var joinDate = new DateOnly(2020, 9, 1);

            var academies = new List<AcademyDetailsServiceModel>
            {
                new(urn, null, "Test LA", "Academy", "Urban", joinDate)
            };

            var reportCard = new ReportCardServiceModel();

            _mockAcademyService.GetAcademiesInTrustDetailsAsync(uid).Returns(academies.ToArray());
            _mockReportCardsService.GetReportCardsAsync(12345).Returns(reportCard);

            var result = await _sut.GetEstablishmentsInTrustReportCardsAsync(uid);

            result.Should().HaveCount(1);
            var trustReportCard = result[0];
            trustReportCard.Urn.Should().Be(12345);
            trustReportCard.SchoolName.Should().Be("");
            trustReportCard.ReportDetails.Should().Be(reportCard);
        }

        [Fact]
        public async Task GetEstablishmentsInTrustReportCardsAsync_should_handle_empty_establishment_name()
        {
            const string uid = "TEST123";
            const string urn = "12345";
            var joinDate = new DateOnly(2020, 9, 1);

            var academies = new List<AcademyDetailsServiceModel>
            {
                new(urn, "", "Test LA", "Academy", "Urban", joinDate)
            };

            var reportCard = new ReportCardServiceModel();

            _mockAcademyService.GetAcademiesInTrustDetailsAsync(uid).Returns(academies.ToArray());
            _mockReportCardsService.GetReportCardsAsync(12345).Returns(reportCard);

            var result = await _sut.GetEstablishmentsInTrustReportCardsAsync(uid);

            result.Should().HaveCount(1);
            var trustReportCard = result[0];
            trustReportCard.Urn.Should().Be(12345);
            trustReportCard.SchoolName.Should().Be("");
            trustReportCard.ReportDetails.Should().Be(reportCard);
        }


        [Fact]
        public async Task GetEstablishmentsInTrustReportCardsAsync_should_call_dependencies_with_correct_parameters()
        {
            const string uid = "TEST123";
            const string urn = "12345";
            const string establishmentName = "Test Academy";
            var joinDate = new DateOnly(2020, 9, 1);

            var academies = new List<AcademyDetailsServiceModel>
            {
                new(urn, establishmentName, "Test LA", "Academy", "Urban", joinDate)
            };

            var reportCard = new ReportCardServiceModel();

            _mockAcademyService.GetAcademiesInTrustDetailsAsync(uid).Returns(academies.ToArray());
            _mockReportCardsService.GetReportCardsAsync(Arg.Any<int>()).Returns(reportCard);

            await _sut.GetEstablishmentsInTrustReportCardsAsync(uid);

            await _mockAcademyService.Received(1).GetAcademiesInTrustDetailsAsync(uid);
            await _mockReportCardsService.Received(1).GetReportCardsAsync(12345);
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("")]
        public async Task GetEstablishmentsInTrustReportCardsAsync_if_none_integer_urn_ShouldLog(string urn)
        {
            const string uid = "TEST123";
            const string establishmentName = "Test Academy";
            var joinDate = new DateOnly(2020, 9, 1);

            var academies = new List<AcademyDetailsServiceModel>
            {
                new(urn, establishmentName, "Test LA", "Academy", "Urban", joinDate)
            };

            var reportCard = new ReportCardServiceModel();

            _mockAcademyService.GetAcademiesInTrustDetailsAsync(uid).Returns(academies.ToArray());
            _mockReportCardsService.GetReportCardsAsync(Arg.Any<int>()).Returns(reportCard);

            await _sut.GetEstablishmentsInTrustReportCardsAsync(uid);

            _mockLogger.VerifyLogErrors($"Unable to parse academy urn {urn} for trust {uid}");

        }

        [Fact]
        public async Task GetEstablishmentsInTrustOlderOfstedRatings_ShouldReturnEmptyList_WhenNoAcademiesInTrust()
        {
            const string trustUid = "1234";
            var emptyAcademies = Array.Empty<SchoolOfsted>();

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns(emptyAcademies);

            var result = await _sut.GetEstablishmentsInTrustOlderOfstedRatings(trustUid);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetEstablishmentsInTrustOlderOfstedRatings_ShouldReturnCorrectData_WhenAcademiesHaveValidUrns()
        {
            // Arrange
            const string trustUid = "1234";
            const int urn1 = 123456;
            const int urn2 = 789012;

            var currentRating1 = new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local));
            var previousRating1 = new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local));

            var currentRating2 = new OfstedRating((int)OfstedRatingScore.Outstanding, new DateTime(2024, 8, 15, 0, 0, 0, DateTimeKind.Local));
            var previousRating2 = new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2023, 7, 15, 0, 0, 0, DateTimeKind.Local));

            var academy1 = new SchoolOfsted(
                urn1.ToString(),
                "First Academy",
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                previousRating1,
                currentRating1,
                false);

            var academy2 = new SchoolOfsted(
                urn2.ToString(),
                "Second Academy",
                new DateTime(2023, 9, 15, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2023, 8, 15, 0, 0, 0, DateTimeKind.Local), "School remains Outstanding"),
                previousRating2,
                currentRating2,
                false);

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns([academy1, academy2]);

            var result = await _sut.GetEstablishmentsInTrustOlderOfstedRatings(trustUid);

            result.Should().HaveCount(2);

            result[0].Urn.Should().Be(urn1);
            result[0].SchoolName.Should().Be("First Academy");
            result[0].ReportDetails.Should().NotBeNull();
            result[0].ReportDetails.Ratings.Should().HaveCount(2);
            result[0].ReportDetails.Ratings[0].Should().Be(currentRating1);
            result[0].ReportDetails.Ratings[1].Should().Be(previousRating1);

            result[1].Urn.Should().Be(urn2);
            result[1].SchoolName.Should().Be("Second Academy");
            result[1].ReportDetails.Should().NotBeNull();
            result[1].ReportDetails.Ratings.Should().HaveCount(2);
            result[1].ReportDetails.Ratings[0].Should().Be(currentRating2);
            result[1].ReportDetails.Ratings[1].Should().Be(previousRating2);
        }

        [Fact]
        public async Task GetEstablishmentsInTrustOlderOfstedRatings_ShouldHandleNullEstablishmentName()
        {
            const string trustUid = "1234";
            const int urn = 123456;

            var currentRating = new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local));
            var previousRating = new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local));

            var academy = new SchoolOfsted(
                urn.ToString(),
                null, // null establishment name
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                currentRating,
                previousRating,
                false);

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns([academy]);

            var result = await _sut.GetEstablishmentsInTrustOlderOfstedRatings(trustUid);

            result.Should().HaveCount(1);
            result[0].SchoolName.Should().Be(string.Empty);
            result[0].Urn.Should().Be(urn);
            result[0].ReportDetails.Ratings.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetEstablishmentsInTrustOlderOfstedRatings_ShouldLogError_WhenUrnCannotBeParsed()
        {
            const string trustUid = "1234";
            const string invalidUrn = "invalid-urn";

            var currentRating = new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local));
            var previousRating = new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local));

            var academy = new SchoolOfsted(
                invalidUrn,
                "Test Academy",
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                currentRating,
                previousRating,
                false);

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns([academy]);

            var result = await _sut.GetEstablishmentsInTrustOlderOfstedRatings(trustUid);

            result.Should().BeEmpty();
            _mockLogger.VerifyLogError($"Unable to parse academy urn {invalidUrn} for trust {trustUid}");
        }

        [Fact]
        public async Task GetEstablishmentsInTrustOlderOfstedRatings_ShouldSkipInvalidUrnsAndIncludeValidOnes()
        {
            const string trustUid = "1234";
            const int validUrn = 123456;
            const string invalidUrn = "invalid-urn";

            var currentRating = new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local));
            var previousRating = new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local));

            var validAcademy = new SchoolOfsted(
                validUrn.ToString(),
                "Valid Academy",
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                currentRating,
                previousRating,
                false);

            var invalidAcademy = new SchoolOfsted(
                invalidUrn,
                "Invalid Academy",
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                currentRating,
                previousRating,
                false);

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns([validAcademy, invalidAcademy]);

            var result = await _sut.GetEstablishmentsInTrustOlderOfstedRatings(trustUid);

            result.Should().HaveCount(1);
            result[0].Urn.Should().Be(validUrn);
            result[0].SchoolName.Should().Be("Valid Academy");

            _mockLogger.VerifyLogError($"Unable to parse academy urn {invalidUrn} for trust {trustUid}");
        }

        [Fact]
        public async Task GetEstablishmentsInTrustOlderOfstedRatings_ShouldCallRepositoryWithCorrectTrustUid()
        {
            const string trustUid = "1234";
            var emptyAcademies = Array.Empty<SchoolOfsted>();

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns(emptyAcademies);

            await _sut.GetEstablishmentsInTrustOlderOfstedRatings(trustUid);

            await _mockOfstedRepository.Received(1).GetAcademiesInTrustOfstedAsync(trustUid);
        }

        [Fact]
        public async Task GetEstablishmentsInTrustOlderOfstedRatings_ShouldCreateCorrectOlderInspectionServiceModel()
        {
            const string trustUid = "1234";
            const int urn = 123456;

            var currentRating = new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Local));
            var previousRating = new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Local));

            var academy = new SchoolOfsted(
                urn.ToString(),
                "Test Academy",
                new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Local),
                new OfstedShortInspection(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Local), "School remains Good"),
                currentRating,
                previousRating,
                false);

            _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(trustUid)
                .Returns([academy]);

            var result = await _sut.GetEstablishmentsInTrustOlderOfstedRatings(trustUid);

            result.Should().HaveCount(1);
            var trustReport = result[0];

            trustReport.Should().BeOfType<TrustOfstedReportServiceModel<OlderInspectionServiceModel>>();
            trustReport.ReportDetails.Should().BeOfType<OlderInspectionServiceModel>();
            trustReport.ReportDetails.Ratings.Should().Contain(currentRating);
            trustReport.ReportDetails.Ratings.Should().Contain(previousRating);
            trustReport.ReportDetails.Ratings.Should().HaveCount(2);
        }
    }
}
