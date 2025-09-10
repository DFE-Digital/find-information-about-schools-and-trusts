using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class PupilCensusRepositoryTests
{
    private const int Urn = 123456;
    
    private readonly PupilCensusRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();

    private readonly List<EdperfFiat> _dummyEdperfFiats =
    [
        new()
        {
            Urn = Urn,
            DownloadYear = "2020-2021",
            CensusNor = "100",
            CensusTsenelse = "10",
            CensusPsenelse = "10.0",
            CensusTsenelk = "11",
            CensusPsenelk = "11.0",
            CensusNumeal = "12",
            CensusPnumeal = "12.0",
            CensusNumfsm = "13",
            AbsencePerctot = "14.0",
            AbsencePpersabs10 = "15.0",
            MetaCensusIngestionDatetime = DateTime.Parse("2020-08-31"),
            MetaAbsenceIngestionDatetime = DateTime.Parse("2020-08-31")
        },
        new()
        {
            Urn = Urn,
            DownloadYear = "2021-2022",
            CensusNor = "200",
            CensusTsenelse = "20",
            CensusPsenelse = "20.0",
            CensusTsenelk = "21",
            CensusPsenelk = "21.0",
            CensusNumeal = "22",
            CensusPnumeal = "22.0",
            CensusNumfsm = "23",
            AbsencePerctot = "24.0",
            AbsencePpersabs10 = "25.0",
            MetaCensusIngestionDatetime = DateTime.Parse("2021-08-31"),
            MetaAbsenceIngestionDatetime = DateTime.Parse("2021-08-31")
        },
        new()
        {
            Urn = Urn,
            DownloadYear = "2022-2023",
            CensusNor = "300",
            CensusTsenelse = "30",
            CensusPsenelse = "30.0",
            CensusTsenelk = "31",
            CensusPsenelk = "31.0",
            CensusNumeal = "32",
            CensusPnumeal = "32.0",
            CensusNumfsm = "33",
            AbsencePerctot = "34.0",
            AbsencePpersabs10 = "35.0",
            MetaCensusIngestionDatetime = DateTime.Parse("2022-08-31"),
            MetaAbsenceIngestionDatetime = DateTime.Parse("2022-08-31")
        },
        new()
        {
            Urn = Urn,
            DownloadYear = "2023-2024",
            CensusNor = "400",
            CensusTsenelse = "40",
            CensusPsenelse = "40.0",
            CensusTsenelk = "41",
            CensusPsenelk = "41.0",
            CensusNumeal = "42",
            CensusPnumeal = "42.0",
            CensusNumfsm = "43",
            AbsencePerctot = "44.0",
            AbsencePpersabs10 = "45.0",
            MetaCensusIngestionDatetime = DateTime.Parse("2023-08-31"),
            MetaAbsenceIngestionDatetime = DateTime.Parse("2023-08-31")
        },
        new()
        {
            Urn = Urn,
            DownloadYear = "2024-2025",
            CensusNor = "500",
            CensusTsenelse = "50",
            CensusPsenelse = "50.0",
            CensusTsenelk = "51",
            CensusPsenelk = "51.0",
            CensusNumeal = "52",
            CensusPnumeal = "52.0",
            CensusNumfsm = "53",
            AbsencePerctot = "54.0",
            AbsencePpersabs10 = "55.0",
            MetaCensusIngestionDatetime = DateTime.Parse("2024-08-31"),
            MetaAbsenceIngestionDatetime = DateTime.Parse("2024-08-31")
        }
    ];

    private readonly Dictionary<CensusYear, SchoolPopulation> _dummySchoolPopulations =
        new()
        {
            {
                new CensusYear(2021),
                new SchoolPopulation(
                    new Statistic<int>.WithValue(100),
                    new Statistic<int>.WithValue(10),
                    new Statistic<decimal>.WithValue(10.0m),
                    new Statistic<int>.WithValue(11),
                    new Statistic<decimal>.WithValue(11.0m),
                    new Statistic<int>.WithValue(12),
                    new Statistic<decimal>.WithValue(12.0m),
                    new Statistic<int>.WithValue(13),
                    new Statistic<decimal>.WithValue(13.0m)
                )
            },
            {
                new CensusYear(2022),
                new SchoolPopulation(
                    new Statistic<int>.WithValue(200),
                    new Statistic<int>.WithValue(20),
                    new Statistic<decimal>.WithValue(20.0m),
                    new Statistic<int>.WithValue(21),
                    new Statistic<decimal>.WithValue(21.0m),
                    new Statistic<int>.WithValue(22),
                    new Statistic<decimal>.WithValue(22.0m),
                    new Statistic<int>.WithValue(23),
                    new Statistic<decimal>.WithValue(11.5m)
                )
            },
            {
                new CensusYear(2023),
                new SchoolPopulation(
                    new Statistic<int>.WithValue(300),
                    new Statistic<int>.WithValue(30),
                    new Statistic<decimal>.WithValue(30.0m),
                    new Statistic<int>.WithValue(31),
                    new Statistic<decimal>.WithValue(31.0m),
                    new Statistic<int>.WithValue(32),
                    new Statistic<decimal>.WithValue(32.0m),
                    new Statistic<int>.WithValue(33),
                    new Statistic<decimal>.WithValue(11.0m)
                )
            },
            {
                new CensusYear(2024),
                new SchoolPopulation(
                    new Statistic<int>.WithValue(400),
                    new Statistic<int>.WithValue(40),
                    new Statistic<decimal>.WithValue(40.0m),
                    new Statistic<int>.WithValue(41),
                    new Statistic<decimal>.WithValue(41.0m),
                    new Statistic<int>.WithValue(42),
                    new Statistic<decimal>.WithValue(42.0m),
                    new Statistic<int>.WithValue(43),
                    new Statistic<decimal>.WithValue(10.8m)
                )
            },
            {
                new CensusYear(2025),
                new SchoolPopulation(
                    new Statistic<int>.WithValue(500),
                    new Statistic<int>.WithValue(50),
                    new Statistic<decimal>.WithValue(50.0m),
                    new Statistic<int>.WithValue(51),
                    new Statistic<decimal>.WithValue(51.0m),
                    new Statistic<int>.WithValue(52),
                    new Statistic<decimal>.WithValue(52.0m),
                    new Statistic<int>.WithValue(53),
                    new Statistic<decimal>.WithValue(10.6m)
                )
            }
        };

    public PupilCensusRepositoryTests()
    {
        _mockAcademiesDbContext.EdperfFiats.AddRange(_dummyEdperfFiats);

        _sut = new PupilCensusRepository(_mockAcademiesDbContext.Object);
    }

    [Fact]
    public async Task GetSchoolPopulationStatisticsAsync_should_return_empty_when_school_is_not_found()
    {
        var result = await _sut.GetSchoolPopulationStatisticsAsync(999999);
        
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetSchoolPopulationStatisticsAsync_should_return_school_population_statistics_when_school_is_found()
    {
        var result = await _sut.GetSchoolPopulationStatisticsAsync(Urn);
        
        result.Should().BeEquivalentTo(_dummySchoolPopulations);
    }
    
    [Theory]
    [InlineData("SUPP", StatisticKind.Suppressed)]
    [InlineData("NP", StatisticKind.NotPublished)]
    [InlineData("NA", StatisticKind.NotApplicable)]
    [InlineData("Other text", StatisticKind.NotAvailable)]
    [InlineData("Different text", StatisticKind.NotAvailable)]
    [InlineData("", StatisticKind.NotAvailable)]
    public async Task GetSchoolPopulationStatisticsAsync_parses_statistics_without_values_correctly(string statisticValue, StatisticKind expectedKind)
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = Urn,
                DownloadYear = "2019-2020",
                CensusNor = statisticValue,
                CensusTsenelse = statisticValue,
                CensusPsenelse = statisticValue,
                CensusTsenelk = statisticValue,
                CensusPsenelk = statisticValue,
                CensusNumeal = statisticValue,
                CensusPnumeal = statisticValue,
                CensusNumfsm = statisticValue,
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);
        
        var result = await sut.GetSchoolPopulationStatisticsAsync(Urn);
        
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[2020].PupilsOnRole.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsWithEhcPlan.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsWithEhcPlanPercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[2020].PupilsWithSenSupport.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsWithSenSupportPercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[2020].PupilsWithEnglishAsAdditionalLanguage.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsWithEnglishAsAdditionalLanguagePercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[2020].PupilsEligibleForFreeSchoolMeals.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsEligibleForFreeSchoolMealsPercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
    }

    [Fact]
    public async Task GetSchoolPopulationStatisticsAsync_parses_decimal_statistics_with_percent_signs_correctly()
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = Urn,
                DownloadYear = "2019-2020",
                CensusNor = "100",
                CensusTsenelse = "10",
                CensusPsenelse = "10.0%",
                CensusTsenelk = "11",
                CensusPsenelk = "11.0%",
                CensusNumeal = "12",
                CensusPnumeal = "12.0%",
                CensusNumfsm = "13",
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);
        
        var result = await sut.GetSchoolPopulationStatisticsAsync(Urn);
        
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[2020].PupilsWithEhcPlanPercentage.Should().Be(new Statistic<decimal>.WithValue(10.0m));
        result[2020].PupilsWithSenSupportPercentage.Should().Be(new Statistic<decimal>.WithValue(11.0m));
        result[2020].PupilsWithEnglishAsAdditionalLanguagePercentage.Should().Be(new Statistic<decimal>.WithValue(12.0m));
        result[2020].PupilsEligibleForFreeSchoolMealsPercentage.Should().Be(new Statistic<decimal>.WithValue(13.0m));
    }
}
