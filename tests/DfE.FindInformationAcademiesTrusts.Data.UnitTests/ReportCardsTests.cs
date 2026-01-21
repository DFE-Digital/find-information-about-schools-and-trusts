using Dfe.AcademiesApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.Data.UnitTests
{
    using DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;
    using NSubstitute;

    public class ReportCardsTests
    {
        private readonly ReportCardsRepository _sut;
        private readonly IEstablishmentsV5Client mockEstablishmentsV5Client;
        private readonly int Urn = 1234;

        public ReportCardsTests()
        {
            mockEstablishmentsV5Client = Substitute.For<IEstablishmentsV5Client>();

            _sut = new ReportCardsRepository(mockEstablishmentsV5Client);

            mockEstablishmentsV5Client.SearchEstablishmentsWithMockReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([]);
        }

        [Fact]
        public async Task WhenCalling_GetReportCardAsync_ShouldCallCorrectEndpoint()
        {
            await _sut.GetReportCardAsync(Urn);
            await mockEstablishmentsV5Client.Received(1).SearchEstablishmentsWithMockReportCardsAsync(null, null, Urn.ToString(), null, null);
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
            mockEstablishmentsV5Client.SearchEstablishmentsWithMockReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([new EstablishmentDto2()
            {
                ReportCard = new ReportCardDto
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
            mockEstablishmentsV5Client.SearchEstablishmentsWithMockReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([new EstablishmentDto2()
            {
                ReportCard = new ReportCardDto
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
                .SearchEstablishmentsWithMockReportCardsAsync(null, null, Urn.ToString(), null, null).Returns([
                    new EstablishmentDto2
                    {
                        ReportCard = CreateMockReportCardDto(),
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

        private static ReportCardDto CreateMockReportCardDto()
        {
            return new ReportCardDto
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
