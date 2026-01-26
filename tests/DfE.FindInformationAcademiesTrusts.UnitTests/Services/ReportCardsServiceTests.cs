using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using GovUK.Dfe.CoreLibs.Caching.Helpers;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services
{

    public class ReportCardsServiceTests
    {
        private readonly ReportCardsService _sut;

        private readonly IReportCardsRepository _mockReportCardsRepository = Substitute.For<IReportCardsRepository>();
        private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();

        private readonly ICacheService<IMemoryCacheType> _mockCacheService =
            Substitute.For<ICacheService<IMemoryCacheType>>();

        private readonly int Urn = 123;

        public ReportCardsServiceTests()
        {
            _sut = new ReportCardsService(_mockReportCardsRepository, _mockSchoolRepository, _mockCacheService);

            _mockReportCardsRepository.GetReportCardAsync(Urn)
                .Returns((
                    new EstablishmentReportCard(new DateOnly(2025, 01, 20), "https://ofsted.gov.uk/1", "Good", "Good",
                        "Excellent", "Good", "Good", "Outstanding", null, "Met", "Good", "None"),
                    new EstablishmentReportCard(new DateOnly(2023, 09, 18), "https://ofsted.gov.uk/2", "Good", "Good",
                        "Excellent", "Good", "Good", "Outstanding", null, "Not Met", "Good", "None")));

            _mockSchoolRepository.GetDateJoinedTrustAsync(Urn)
                .Returns(new DateOnly(2020, 01, 01));

            _mockCacheService.GetOrAddAsync(Arg.Any<string>(),
                    Arg.Any<Func<Task<(EstablishmentReportCard? LatestReportCard, EstablishmentReportCard?
                        PreviousReportCard)>>>(), Arg.Any<string>())
                .Returns(callInfo =>
                {
                    var callback = callInfo
                        .ArgAt<Func<Task<(EstablishmentReportCard? LatestReportCard, EstablishmentReportCard?
                            PreviousReportCard)>>>(1);
                    return callback();
                });
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldCallRepository()
        {
            await _sut.GetReportCardsAsync(Urn);
            await _mockReportCardsRepository.Received(1).GetReportCardAsync(Urn);
        }

        [Fact]
        public async Task GetReportCardsAsync_IfLatestReportCardIsNull_ShouldReturnNull()
        {
            _mockReportCardsRepository.GetReportCardAsync(Urn).Returns((null,
                new EstablishmentReportCard(new DateOnly(2025, 01, 20), "https://ofsted.gov.uk/1", "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, "Met", "Good", "None")));

            var result = await _sut.GetReportCardsAsync(Urn);
            result.LatestReportCard.Should().BeNull();
            result.PreviousReportCard.Should().NotBeNull();
        }

        [Fact]
        public async Task GetReportCardsAsync_IfPreviousReportCardIsNull_ShouldReturnNull()
        {
            _mockReportCardsRepository.GetReportCardAsync(Urn).Returns((
                new EstablishmentReportCard(new DateOnly(2025, 01, 20), "https://ofsted.gov.uk/1", "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, "Met", "Good", "None"), null));

            var result = await _sut.GetReportCardsAsync(Urn);
            result.LatestReportCard.Should().NotBeNull();
            result.PreviousReportCard.Should().BeNull();
        }

        [Fact]
        public async Task GetReportCardsAsync_IfSchoolIsNotPartOfATrust_ShouldSetDateJoinedAsNull()
        {
            _mockSchoolRepository.GetDateJoinedTrustAsync(Urn)
                .ReturnsNull();

            _mockReportCardsRepository.GetReportCardAsync(Urn).Returns((
                new EstablishmentReportCard(new DateOnly(2025, 01, 20), "https://ofsted.gov.uk/1", "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, "Met", "Good", "None"), null));

            var result = await _sut.GetReportCardsAsync(Urn);
            result.LatestReportCard.Should().NotBeNull();
            result.PreviousReportCard.Should().BeNull();
            result.DateJoinedTrust.Should().BeNull();
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldMapAllProperties()
        {
            var reportCards = await _sut.GetReportCardsAsync(Urn);
            reportCards.DateJoinedTrust.Should().NotBeNull();
            reportCards.DateJoinedTrust.Should().Be(new DateOnly(2020, 01, 01));
            reportCards.LatestReportCard.Should().NotBeNull();
            reportCards.PreviousReportCard.Should().NotBeNull();

            reportCards.LatestReportCard!.InspectionDate.Should().Be(new DateOnly(2025, 01, 20));
            reportCards.LatestReportCard.WebLink.Should().Be("https://ofsted.gov.uk/1");
            reportCards.LatestReportCard.CurriculumAndTeaching.Should().Be("Good");
            reportCards.LatestReportCard.AttendanceAndBehaviour.Should().Be("Good");
            reportCards.LatestReportCard.PersonalDevelopmentAndWellBeing.Should().Be("Excellent");
            reportCards.LatestReportCard.LeadershipAndGovernance.Should().Be("Good");
            reportCards.LatestReportCard.Inclusion.Should().Be("Good");
            reportCards.LatestReportCard.Achievement.Should().Be("Outstanding");
            reportCards.LatestReportCard.EarlyYearsProvision.Should().BeNull();
            reportCards.LatestReportCard.Safeguarding.Should().Be("Met");
            reportCards.LatestReportCard.Post16Provision.Should().Be("Good");
            reportCards.LatestReportCard.CategoryOfConcern.Should().Be("None");

            reportCards.PreviousReportCard!.InspectionDate.Should().Be(new DateOnly(2023, 09, 18));
            reportCards.PreviousReportCard.WebLink.Should().Be("https://ofsted.gov.uk/2");
            reportCards.PreviousReportCard.CurriculumAndTeaching.Should().Be("Good");
            reportCards.PreviousReportCard.AttendanceAndBehaviour.Should().Be("Good");
            reportCards.PreviousReportCard.PersonalDevelopmentAndWellBeing.Should().Be("Excellent");
            reportCards.PreviousReportCard.LeadershipAndGovernance.Should().Be("Good");
            reportCards.PreviousReportCard.Inclusion.Should().Be("Good");
            reportCards.PreviousReportCard.Achievement.Should().Be("Outstanding");
            reportCards.PreviousReportCard.EarlyYearsProvision.Should().BeNull();
            reportCards.PreviousReportCard.Safeguarding.Should().Be("Not Met");
            reportCards.PreviousReportCard.Post16Provision.Should().Be("Good");
            reportCards.PreviousReportCard.CategoryOfConcern.Should().Be("None");
        }

        [Fact]
        public async Task GetReportCardsAsync_ShouldCallGetOrAddToCache()
        {
            var expectedCacheKey = $"ReportCards_{CacheKeyHelper.GenerateHashedCacheKey(Urn.ToString())}";

            _ = await _sut.GetReportCardsAsync(Urn);

            await _mockCacheService.Received(1).GetOrAddAsync(expectedCacheKey, Arg.Any<Func<Task<(EstablishmentReportCard? LatestReportCard, EstablishmentReportCard?
                PreviousReportCard)>>>(), "GetReportCardsAsync");
        }
    }
}
