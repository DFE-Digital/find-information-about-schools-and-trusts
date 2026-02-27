using Dfe.AcademiesApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.Data.UnitTests
{
    using DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;
    using Microsoft.Extensions.Logging;
    using NSubstitute;

    public class ReportCardsTests
    {
        private readonly ReportCardsRepository _sut;
        private readonly IEstablishmentsV5Client mockEstablishmentsV5Client;
        private readonly ILogger<IReportCardsRepository> _mockLogger = Substitute.For<ILogger<IReportCardsRepository>>();
        private readonly int Urn = 1234;

        public ReportCardsTests()
        {
            mockEstablishmentsV5Client = Substitute.For<IEstablishmentsV5Client>();


            _sut = new ReportCardsRepository(mockEstablishmentsV5Client, _mockLogger);

            mockEstablishmentsV5Client.SearchEstablishmentsWithOfstedReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([]);
        }

        [Fact]
        public async Task WhenCalling_GetReportCardAsync_ShouldCallCorrectEndpoint()
        {
            await _sut.GetReportCardAsync(Urn);
            await mockEstablishmentsV5Client.Received(1).SearchEstablishmentsWithOfstedReportCardsAsync(null, null, Urn.ToString(), null, null);
        }

        [Fact]
        public async Task WhenCalling_GetReportCardAsync_IfNoResultsShouldReturnNull()
        {
            var result = await _sut.GetReportCardAsync(Urn);
            result.LatestReportCard.Should().BeNull();
            result.PreviousReportCard.Should().BeNull();
        }

        [Fact]
        public async Task WhenCalling_GetReportCardAsync_IfLatestInspectionDateIsNull_ShouldReturnNull()
        {
            mockEstablishmentsV5Client.SearchEstablishmentsWithOfstedReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([new EstablishmentDto2()
            {
                ReportCardFullInspection = new ReportCardFullInspectionDto
                {
                    LatestInspectionDate = null,
                    PreviousInspectionDate = "25/12/2020",
                },
                Urn = Urn.ToString()
            }]);

            var result = await _sut.GetReportCardAsync(Urn);
            result.LatestReportCard.Should().BeNull();
            result.PreviousReportCard.Should().NotBeNull();
        }

        [Fact]
        public async Task WhenCalling_GetReportCardAsync_IfPreviousInspectionDateIsNull_ShouldReturnNull()
        {
            mockEstablishmentsV5Client.SearchEstablishmentsWithOfstedReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([new EstablishmentDto2()
            {
                ReportCardFullInspection = new ReportCardFullInspectionDto
                {
                    LatestInspectionDate = "25/12/2020",
                    PreviousInspectionDate = null,
                },
                Urn = Urn.ToString()
            }]);

            var result = await _sut.GetReportCardAsync(Urn);
            result.LatestReportCard.Should().NotBeNull();
            result.PreviousReportCard.Should().BeNull();
        }

        [Fact]
        public async Task WhenCalling_GetReportCardAsync_ShouldMapAllProperties()
        {

            mockEstablishmentsV5Client
                .SearchEstablishmentsWithOfstedReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([
                    new EstablishmentDto2
                    {
                        ReportCardFullInspection = CreateMockReportCardDto(),
                        Urn = Urn.ToString()
                    }
                ]);

            var result = await _sut.GetReportCardAsync(Urn);
            var latest = result.LatestReportCard;
            var previous = result.PreviousReportCard;

            latest.Should().NotBeNull();
            previous.Should().NotBeNull();

            // Latest report card assertions
            latest!.InspectionDate.Should().Be(new DateOnly(2023, 1, 1));
            latest.CurriculumAndTeaching.Should().Be("Outstanding");
            latest.AttendanceAndBehaviour.Should().Be("Good");
            latest.PersonalDevelopmentAndWellBeing.Should().Be("Good");
            latest.LeadershipAndGovernance.Should().Be("Outstanding");
            latest.Inclusion.Should().Be("Good");
            latest.Achievement.Should().Be("Outstanding");
            latest.EarlyYearsProvision.Should().Be("Good");
            latest.Safeguarding.Should().Be("Effective");
            latest.WebLink.Should().Be("https://ofsted.gov.uk/report/1234");

            // Previous report card assertions
            previous!.InspectionDate.Should().Be(new DateOnly(2021, 1, 1));
            previous.CurriculumAndTeaching.Should().Be("Good");
            previous.AttendanceAndBehaviour.Should().Be("Requires Improvement");
            previous.PersonalDevelopmentAndWellBeing.Should().Be("Good");
            previous.LeadershipAndGovernance.Should().Be("Good");
            previous.Inclusion.Should().Be("Requires Improvement");
            previous.Achievement.Should().Be("Good");
            previous.EarlyYearsProvision.Should().Be("Requires Improvement");
            previous.Safeguarding.Should().Be("Effective");
            previous.WebLink.Should().Be("https://ofsted.gov.uk/report/1234");
        }

        [Fact]
        public async Task WhenCalling_GetReportCardAsync_IfDataIsNotReturn_ShouldReturnEmptyObject()
        {
            mockEstablishmentsV5Client.SearchEstablishmentsWithOfstedReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([new EstablishmentDto2()
            {
                ReportCardFullInspection = new ReportCardFullInspectionDto
                {
                    LatestInspectionDate = "25/12/2020",
                    PreviousInspectionDate = null,
                },
                Urn = "A different urn"
            }]);

            var result = await _sut.GetReportCardAsync(Urn);
            result.Should().NotBeNull();
            result.Urn.Should().Be(Urn);
            result.LatestReportCard.Should().BeNull();
            result.PreviousReportCard.Should().BeNull();
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldReturnEmptyList_WhenClientReturnsNoResults()
        {
            var urns = new List<int> { 100001 };
            mockEstablishmentsV5Client.GetEstablishmentsWithOfstedReportCardsByUrnsAsync(Arg.Any<UrnRequestModel>())
                .Returns([]);

            var result = await _sut.GetReportCardsAsync(urns);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldSendAllUrns_ToService()
        {
            var urns = new List<int> { 100001, 123344 };

            mockEstablishmentsV5Client.GetEstablishmentsWithOfstedReportCardsByUrnsAsync(Arg.Any<UrnRequestModel>())
                .Returns([]);

            await _sut.GetReportCardsAsync(urns);

           await mockEstablishmentsV5Client.Received(1).GetEstablishmentsWithOfstedReportCardsByUrnsAsync(Arg.Is<UrnRequestModel>(x => x.Urns == urns));
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldMapEstablishmentDataToReportCardData()
        {
            var urns = new List<int> { Urn };
            var establishmentDto = new EstablishmentDto2
            {
                Urn = Urn.ToString(),
                ReportCardFullInspection = CreateMockReportCardDto()
            };
            mockEstablishmentsV5Client.GetEstablishmentsWithOfstedReportCardsByUrnsAsync(Arg.Any<UrnRequestModel>())
                .Returns([establishmentDto]);

            var result = await _sut.GetReportCardsAsync(urns);

            result.Should().HaveCount(1);
            var reportCardData = result.First();
            reportCardData.Urn.Should().Be(Urn);

            var latest = reportCardData.LatestReportCard;
            latest.Should().NotBeNull();
            latest!.InspectionDate.Should().Be(new DateOnly(2023, 1, 1));
            latest.WebLink.Should().Be("https://ofsted.gov.uk/report/1234");

            var previous = reportCardData.PreviousReportCard;
            previous.Should().NotBeNull();
            previous!.InspectionDate.Should().Be(new DateOnly(2021, 1, 1));
            previous.WebLink.Should().Be("https://ofsted.gov.uk/report/1234");
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldLogAndSkipInvalidUrn()
        {
            var urns = new List<int> { Urn };
            var establishmentDto = new EstablishmentDto2
            {
                Urn = "invalid-urn",
                ReportCardFullInspection = CreateMockReportCardDto()
            };
            mockEstablishmentsV5Client.GetEstablishmentsWithOfstedReportCardsByUrnsAsync(Arg.Any<UrnRequestModel>())
                .Returns([establishmentDto]);

            var result = await _sut.GetReportCardsAsync(urns);

            result.Should().BeEmpty();
            _mockLogger.VerifyLogError($"Unable to parse academy urn invalid-urn");
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldContinueProcessing_WhenUrnCannotBeParsed()
        {
            var urns = new List<int> { Urn, 9999 };
            var establishmentDto1 = new EstablishmentDto2
            {
                Urn = Urn.ToString(),
                ReportCardFullInspection = CreateMockReportCardDto()
            };
            var establishmentDto2 = new EstablishmentDto2
            {
                Urn = "not-a-urn",
                ReportCardFullInspection = CreateMockReportCardDto()
            };
            var establishmentDto3 = new EstablishmentDto2
            {
                Urn = "9999",
                ReportCardFullInspection = CreateMockReportCardDto()
            };
            mockEstablishmentsV5Client.GetEstablishmentsWithOfstedReportCardsByUrnsAsync(Arg.Any<UrnRequestModel>())
                .Returns([establishmentDto1, establishmentDto2, establishmentDto3]);

            var result = await _sut.GetReportCardsAsync(urns);

            result.Should().HaveCount(2);
            result.Should().Contain(r => r.Urn == Urn);
            result.Should().Contain(r => r.Urn == 9999);
            _mockLogger.VerifyLogError("Unable to parse academy urn not-a-urn");
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldSkipResult_WhenUrnIsNotInTheRequest()
        {
            var urns = new List<int> { 100001 };
            var establishmentDto = new EstablishmentDto2
            {
                Urn = "100002", // This URN is not in the requested list
                ReportCardFullInspection = CreateMockReportCardDto()
            };
            mockEstablishmentsV5Client.GetEstablishmentsWithOfstedReportCardsByUrnsAsync(Arg.Any<UrnRequestModel>())
                .Returns([establishmentDto]);

            var result = await _sut.GetReportCardsAsync(urns);

            result.Should().BeEmpty();
        }

        private static ReportCardFullInspectionDto CreateMockReportCardDto()
        {
            return new ReportCardFullInspectionDto
            {
                WebLink = "https://ofsted.gov.uk/report/1234",
                LatestInspectionDate = "01/01/2023",
                LatestCurriculumAndTeaching = "Outstanding",
                LatestAttendanceAndBehaviour = "Good",
                LatestPersonalDevelopmentAndWellbeing = "Good",
                LatestLeadershipAndGovernance = "Outstanding",
                LatestInclusion = "Good",
                LatestAchievement = "Outstanding",
                LatestEarlyYearsProvision = "Good",
                LatestSafeguarding = "Effective",
                PreviousInspectionDate = "01/01/2021",
                PreviousCurriculumAndTeaching = "Good",
                PreviousAttendanceAndBehaviour = "Requires Improvement",
                PreviousPersonalDevelopmentAndWellbeing = "Good",
                PreviousLeadershipAndGovernance = "Good",
                PreviousInclusion = "Requires Improvement",
                PreviousAchievement = "Good",
                PreviousEarlyYearsProvision = "Requires Improvement",
                PreviousSafeguarding = "Effective"
            };
        }
    }
}
