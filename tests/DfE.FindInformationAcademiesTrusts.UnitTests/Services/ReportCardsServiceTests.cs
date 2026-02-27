using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;
using GovUK.Dfe.CoreLibs.Caching.Helpers;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services
{

    public class ReportCardsServiceTests
    {
        private readonly ReportCardsService _sut;

        private readonly IReportCardsRepository _mockReportCardsRepository = Substitute.For<IReportCardsRepository>();

        private readonly ICacheService<IMemoryCacheType> _mockCacheService = Substitute.For<ICacheService<IMemoryCacheType>>();

        private readonly ILogger<IReportCardsService> _mockLogger = Substitute.For<ILogger<IReportCardsService>>();

        private readonly int Urn = 123;

        public ReportCardsServiceTests()
        {
            _sut = new ReportCardsService(_mockReportCardsRepository, _mockCacheService, _mockLogger);

            var mockReportCard = new ReportCardData
            {
                Urn = Urn,
                LatestReportCard = new EstablishmentReportCard(new DateOnly(2025, 01, 20), "https://ofsted.gov.uk/1",
                    "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, "Met", "Good", "None"),
                PreviousReportCard = new EstablishmentReportCard(new DateOnly(2023, 09, 18), "https://ofsted.gov.uk/2",
                    "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, "Not Met", "Good", "None")
            };

            _mockReportCardsRepository.GetReportCardAsync(Urn).Returns(mockReportCard);

            _mockCacheService.GetOrAddAsync(Arg.Any<string>(),
                    Arg.Any<Func<Task<ReportCardData>>>(), Arg.Any<string>())
                .Returns(callInfo =>
                {
                    var callback = callInfo
                        .ArgAt<Func<Task<ReportCardData>>>(1);
                    return callback();
                });

            _mockCacheService.GetOrAddAsync(Arg.Any<string>(),
                    Arg.Any<Func<Task<List<ReportCardData>>>>(), Arg.Any<string>())
                .Returns(callInfo =>
                {
                    var callback = callInfo
                        .ArgAt<Func<Task<List<ReportCardData>>>>(1);
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
            var mockReportCard = new ReportCardData
            {
                Urn = Urn,
                LatestReportCard = null,
                PreviousReportCard = new EstablishmentReportCard(new DateOnly(2023, 09, 18), "https://ofsted.gov.uk/2",
                    "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, "Not Met", "Good", "None")
            };

            _mockReportCardsRepository.GetReportCardAsync(Urn).Returns(mockReportCard);

            var result = await _sut.GetReportCardsAsync(Urn);
            result.LatestReportCard.Should().BeNull();
            result.PreviousReportCard.Should().NotBeNull();
        }

        [Fact]
        public async Task GetReportCardsAsync_IfPreviousReportCardIsNull_ShouldReturnNull()
        {
            var mockReportCard = new ReportCardData
            {
                Urn = Urn,
                LatestReportCard = new EstablishmentReportCard(new DateOnly(2025, 01, 20), "https://ofsted.gov.uk/1", "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, "Met", "Good", "None"),
                PreviousReportCard = null
            };


            _mockReportCardsRepository.GetReportCardAsync(Urn).Returns(mockReportCard);

            var result = await _sut.GetReportCardsAsync(Urn);
            result.LatestReportCard.Should().NotBeNull();
            result.PreviousReportCard.Should().BeNull();
        }


        [Fact]
        public async Task GetReportCardsAsync_ShouldMapAllProperties()
        {
            var reportCards = await _sut.GetReportCardsAsync(Urn);
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

            await _mockCacheService.Received(1).GetOrAddAsync(expectedCacheKey, Arg.Any<Func<Task<ReportCardData>>>(), "GetReportCardsAsync");
        }

        [Theory]
        [InlineData (null, "Not inspected")]
        [InlineData("", "No concerns")]
        [InlineData("SM", "Special measures")]
        [InlineData("SWK", "Serious weaknesses")]
        [InlineData("NTI", "Notice to improve")]
        [InlineData("All good", "All good")]
        public async Task GetReportCardsAsync_ShouldMapCategoryOfConcern(string? categoryOfConcern, string expected)
        {
            var mockReportCard = new ReportCardData
            {
                Urn = Urn,
                LatestReportCard = new EstablishmentReportCard(new DateOnly(2025, 01, 20), "https://ofsted.gov.uk/1", "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, "Met", "Good", categoryOfConcern),
                PreviousReportCard = null
            };

            _mockReportCardsRepository.GetReportCardAsync(Urn).Returns(mockReportCard);

            var reportCards = await _sut.GetReportCardsAsync(Urn);
            reportCards.LatestReportCard!.CategoryOfConcern.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, "Not inspected")]
        [InlineData("NULL", "Not inspected")]
        [InlineData("", "Not inspected")]
        [InlineData("Yes", "Yes")]
        [InlineData("No", "No")]
        [InlineData("9", "Not recorded")]
        [InlineData("All good", "All good")]
        public async Task GetReportCardsAsync_ShouldMapSafeGuarding(string? safeguarding, string expected)
        {
            var mockReportCard = new ReportCardData
            {
                Urn = Urn,
                LatestReportCard = new EstablishmentReportCard(new DateOnly(2025, 01, 20), "https://ofsted.gov.uk/1", "Good", "Good",
                    "Excellent", "Good", "Good", "Outstanding", null, safeguarding, "Good", "None"),
                PreviousReportCard = null
            };

            _mockReportCardsRepository.GetReportCardAsync(Urn).Returns(mockReportCard);

            var reportCards = await _sut.GetReportCardsAsync(Urn);
            reportCards.LatestReportCard!.Safeguarding.Should().Be(expected);
        }

        [Fact]
        public async Task GetReportCardsAsync_WithListOfUrns_ShouldReturnReportCardsForValidUrns()
        {
            var urns = new List<string> { "12345", "67890" };
            var parsedUrns = new List<int> { 12345, 67890 };
            var reportCardData = new List<ReportCardData>
            {
                new() { Urn = 12345, LatestReportCard = new EstablishmentReportCard(new DateOnly(2023, 1, 1), null, null, null, null, null, null, null, null, null, null, null) },
                new() { Urn = 67890, LatestReportCard = new EstablishmentReportCard(new DateOnly(2023, 1, 2), null, null, null, null, null, null, null, null, null, null, null) }
            };

            var receivedValue = new List<int>();
            _mockReportCardsRepository.GetReportCardsAsync(Arg.Do<List<int>>(x => receivedValue = x)).Returns(reportCardData);

            var result = await _sut.GetReportCardsAsync(urns);

            result.Should().HaveCount(2);
            result.Should().Contain(rc => rc.Urn == 12345);
            result.Should().Contain(rc => rc.Urn == 67890);
            receivedValue.Should().BeEquivalentTo(parsedUrns);
        }

        [Fact]
        public async Task GetReportCardsAsync_WithMixOfValidAndInvalidUrns_ShouldLogErrorsAndReturnReportCardsForValidUrns()
        {
            var urns = new List<string> { "12345", "invalid", "67890" };
            var parsedUrns = new List<int> { 12345, 67890 };
            var reportCardData = new List<ReportCardData>
            {
                new() { Urn = 12345, LatestReportCard = new EstablishmentReportCard(new DateOnly(2023, 1, 1), null, null, null, null, null, null, null, null, null, null, null) },
                new() { Urn = 67890, LatestReportCard = new EstablishmentReportCard(new DateOnly(2023, 1, 2), null, null, null, null, null, null, null, null, null, null, null) }
            };

            var receivedValue = new List<int>();
            _mockReportCardsRepository.GetReportCardsAsync(Arg.Do<List<int>>(x => receivedValue = x)).Returns(reportCardData);

            var result = await _sut.GetReportCardsAsync(urns);

            _mockLogger.VerifyLogError($"Unable to parse academy urn {urns[1]}");
            result.Should().HaveCount(2);
            receivedValue.Should().BeEquivalentTo(parsedUrns);
        }

        [Fact]
        public async Task GetReportCardsAsync_WithEmptyListOfUrns_ShouldReturnEmptyList()
        {
            var result = await _sut.GetReportCardsAsync([]);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetReportCardsAsync_WithOnlyInvalidUrns_ShouldLogErrorsAndReturnEmptyList()
        {
            var urns = new List<string> { "invalid1", "invalid2" };

            _mockReportCardsRepository.GetReportCardsAsync([]).Returns([]);

            var result = await _sut.GetReportCardsAsync(urns);

            _mockLogger.VerifyLogError($"Unable to parse academy urn {urns[0]}");
            _mockLogger.VerifyLogError($"Unable to parse academy urn {urns[1]}");

            result.Should().BeEmpty();
            await _mockReportCardsRepository.DidNotReceiveWithAnyArgs().GetReportCardsAsync([]);
        }

        [Fact]
        public async Task GetReportCardsAsync_WithListOfUrns_ShouldCallCacheWithCorrectKey()
        {
            var urns = new List<string> { "12345", "67890" };
            var parsedUrns = new List<int> { 12345, 67890 };
            var expectedCacheKey = $"ReportCards_{CacheKeyHelper.GenerateHashedCacheKey(string.Join(",", parsedUrns))}_list";

            _mockReportCardsRepository.GetReportCardsAsync(parsedUrns).Returns([]);

            await _sut.GetReportCardsAsync(urns);

            await _mockCacheService.Received(1).GetOrAddAsync(expectedCacheKey, Arg.Any<Func<Task<List<ReportCardData>>>>(), "GetReportCardsAsync");
        }

        [Fact]
        public async Task GetReportCardsAsync_WithNoUrns_ShouldReturnEmptyList()
        {
            var urns = new List<string>();

            var result = await _sut.GetReportCardsAsync(urns);
            result.Should().BeEmpty();

            await _mockReportCardsRepository.DidNotReceiveWithAnyArgs().GetReportCardsAsync(Arg.Any<List<int>>());
            await _mockCacheService.DidNotReceiveWithAnyArgs().GetOrAddAsync(Arg.Any<string>(), Arg.Any<Func<Task<List<ReportCardData>>>>(), Arg.Any<string>());
        }
    }
}
