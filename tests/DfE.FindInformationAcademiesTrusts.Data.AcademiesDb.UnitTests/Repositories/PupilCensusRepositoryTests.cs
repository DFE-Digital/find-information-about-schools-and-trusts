using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class PupilCensusRepositoryTests
{
    private const int SchoolUrn = 123456;

    private const int AcademyUrn1 = 234567;
    private const int AcademyUrn2 = 345678;
    
    private const string Uid = "1234";
    
    private readonly PupilCensusRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();

    private readonly List<EdperfFiat> _dummyEdperfFiats =
    [
        new()
        {
            Urn = SchoolUrn,
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
            Urn = SchoolUrn,
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
            Urn = SchoolUrn,
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
            Urn = SchoolUrn,
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
            Urn = SchoolUrn,
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
        },
        new()
        {
            Urn = AcademyUrn1,
            DownloadYear = "2020-2021",
            CensusNor = "600",
            CensusTsenelse = "60",
            CensusPsenelse = "60.0",
            CensusTsenelk = "61",
            CensusPsenelk = "61.0",
            CensusNumeal = "62",
            CensusPnumeal = "62.0",
            CensusNumfsm = "63",
            AbsencePerctot = "64.0",
        },
        new()
        {
            Urn = AcademyUrn2,
            DownloadYear = "2021-2022",
            CensusNor = "700",
            CensusTsenelse = "70",
            CensusPsenelse = "70.0",
            CensusTsenelk = "71",
            CensusPsenelk = "71.0",
            CensusNumeal = "72",
            CensusPnumeal = "72.0",
            CensusNumfsm = "73",
            AbsencePerctot = "74.0",
        },
        new()
        {
            Urn = AcademyUrn2,
            DownloadYear = "2022-2023",
            CensusNor = "800",
            CensusTsenelse = "80",
            CensusPsenelse = "80.0",
            CensusTsenelk = "81",
            CensusPsenelk = "81.0",
            CensusNumeal = "82",
            CensusPnumeal = "82.0",
            CensusNumfsm = "83",
            AbsencePerctot = "84.0",
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

    private readonly Dictionary<CensusYear, Attendance> _dummyAttendances =
        new()
        {
            {
                new CensusYear(2020),
                new Attendance(
                    new Statistic<decimal>.WithValue(14.0m),
                    new Statistic<decimal>.WithValue(15.0m)
                )
            },
            {
                new CensusYear(2021),
                new Attendance(
                    new Statistic<decimal>.WithValue(24.0m),
                    new Statistic<decimal>.WithValue(25.0m)
                )
            },
            {
                new CensusYear(2022),
                new Attendance(
                    new Statistic<decimal>.WithValue(34.0m),
                    new Statistic<decimal>.WithValue(35.0m)
                )
            },
            {
                new CensusYear(2023),
                new Attendance(
                    new Statistic<decimal>.WithValue(44.0m),
                    new Statistic<decimal>.WithValue(45.0m)
                )
            },
            {
                new CensusYear(2024),
                new Attendance(
                    new Statistic<decimal>.WithValue(54.0m),
                    new Statistic<decimal>.WithValue(55.0m)
                )
            }
        };

    private readonly Dictionary<int, SchoolPopulation> _dummySchoolPopulationsForTrust =
        new()
        {
            {
                AcademyUrn1,
                new SchoolPopulation(
                    new Statistic<int>.WithValue(600),
                    new Statistic<int>.WithValue(60),
                    new Statistic<decimal>.WithValue(60.0m),
                    new Statistic<int>.WithValue(61),
                    new Statistic<decimal>.WithValue(61.0m),
                    new Statistic<int>.WithValue(62),
                    new Statistic<decimal>.WithValue(62.0m),
                    new Statistic<int>.WithValue(63),
                    new Statistic<decimal>.WithValue(63.0m)
                )
            },
            {
                AcademyUrn2,
                new SchoolPopulation(
                    new Statistic<int>.WithValue(800),
                    new Statistic<int>.WithValue(80),
                    new Statistic<decimal>.WithValue(80.0m),
                    new Statistic<int>.WithValue(81),
                    new Statistic<decimal>.WithValue(81.0m),
                    new Statistic<int>.WithValue(82),
                    new Statistic<decimal>.WithValue(82.0m),
                    new Statistic<int>.WithValue(83),
                    new Statistic<decimal>.WithValue(83.0m)
                )
            }
        };

    public PupilCensusRepositoryTests()
    {
        _mockAcademiesDbContext.EdperfFiats.AddRange(_dummyEdperfFiats);
        _mockAcademiesDbContext.GiasGroupLinks.AddRange([
            new GiasGroupLink { GroupUid = Uid, Urn = AcademyUrn1.ToString(), GroupStatusCode = "OPEN", JoinedDate = "01/01/2020" },
            new GiasGroupLink { GroupUid = Uid, Urn = AcademyUrn2.ToString(), GroupStatusCode = "OPEN", JoinedDate = "01/01/2020" }
        ]);

        _sut = new PupilCensusRepository(_mockAcademiesDbContext.Object);
    }

    [Fact]
    public async Task GetSchoolPopulationStatisticsAsync_should_return_empty_when_school_is_not_found()
    {
        var result = await _sut.GetSchoolPopulationStatisticsAsync(999999);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSchoolPopulationStatisticsAsync_uses_spring_census()
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = SchoolUrn,
                DownloadYear = "2019-2020"
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);

        var result = await sut.GetSchoolPopulationStatisticsAsync(SchoolUrn);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.Should().ContainKey(2020);
    }

    [Fact]
    public async Task
        GetSchoolPopulationStatisticsAsync_should_return_school_population_statistics_when_school_is_found()
    {
        var result = await _sut.GetSchoolPopulationStatisticsAsync(SchoolUrn);
        result.Should().BeEquivalentTo(_dummySchoolPopulations);
    }

    [Theory]
    [InlineData("SUPP", StatisticKind.Suppressed)]
    [InlineData("NP", StatisticKind.NotPublished)]
    [InlineData("NA", StatisticKind.NotApplicable)]
    [InlineData("Other text", StatisticKind.NotAvailable)]
    [InlineData("Different text", StatisticKind.NotAvailable)]
    [InlineData("", StatisticKind.NotAvailable)]
    public async Task GetSchoolPopulationStatisticsAsync_parses_statistics_without_values_correctly(
        string statisticValue, StatisticKind expectedKind)
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = SchoolUrn,
                DownloadYear = "2019-2020",
                CensusNor = statisticValue,
                CensusTsenelse = statisticValue,
                CensusPsenelse = statisticValue,
                CensusTsenelk = statisticValue,
                CensusPsenelk = statisticValue,
                CensusNumeal = statisticValue,
                CensusPnumeal = statisticValue,
                CensusNumfsm = statisticValue
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);

        var result = await sut.GetSchoolPopulationStatisticsAsync(SchoolUrn);
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[2020].PupilsOnRole.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsWithEhcPlan.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsWithEhcPlanPercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[2020].PupilsWithSenSupport.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsWithSenSupportPercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[2020].PupilsWithEnglishAsAdditionalLanguage.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[2020].PupilsWithEnglishAsAdditionalLanguagePercentage.Should()
            .Be(Statistic<decimal>.FromKind(expectedKind));
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
                Urn = SchoolUrn,
                DownloadYear = "2019-2020",
                CensusNor = "100",
                CensusTsenelse = "10",
                CensusPsenelse = "10.0%",
                CensusTsenelk = "11",
                CensusPsenelk = "11.0%",
                CensusNumeal = "12",
                CensusPnumeal = "12.0%",
                CensusNumfsm = "13"
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);

        var result = await sut.GetSchoolPopulationStatisticsAsync(SchoolUrn);
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[2020].PupilsWithEhcPlanPercentage.Should().Be(new Statistic<decimal>.WithValue(10.0m));
        result[2020].PupilsWithSenSupportPercentage.Should().Be(new Statistic<decimal>.WithValue(11.0m));
        result[2020].PupilsWithEnglishAsAdditionalLanguagePercentage.Should()
            .Be(new Statistic<decimal>.WithValue(12.0m));
        result[2020].PupilsEligibleForFreeSchoolMealsPercentage.Should().Be(new Statistic<decimal>.WithValue(13.0m));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(27)]
    [InlineData(44)]
    [InlineData(86)]
    public async Task GetSchoolPopulationStatisticsAsync_handles_zero_pupils_on_role_correctly(int freeSchoolMeals)
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = SchoolUrn,
                DownloadYear = "2019-2020",
                CensusNor = "0",
                CensusNumfsm = freeSchoolMeals.ToString()
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);

        var result = await sut.GetSchoolPopulationStatisticsAsync(SchoolUrn);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[2020].PupilsOnRole.Should().Be(new Statistic<int>.WithValue(0));
        result[2020].PupilsEligibleForFreeSchoolMealsPercentage.Should().Be(new Statistic<decimal>.WithValue(0.0m));
    }

    [Fact]
    public async Task GetAttendanceStatisticsAsync_should_return_empty_when_school_is_not_found()
    {
        var result = await _sut.GetAttendanceStatisticsAsync(999999);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAttendanceStatisticsAsync_uses_autumn_census()
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = SchoolUrn,
                DownloadYear = "2019-2020"
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);

        var result = await sut.GetAttendanceStatisticsAsync(SchoolUrn);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.Should().ContainKey(2019);
    }

    [Fact]
    public async Task GetAttendanceStatisticsAsync_should_return_attendance_statistics_when_school_is_found()
    {
        var result = await _sut.GetAttendanceStatisticsAsync(SchoolUrn);

        result.Should().BeEquivalentTo(_dummyAttendances);
    }

    [Theory]
    [InlineData("SUPP", StatisticKind.Suppressed)]
    [InlineData("NP", StatisticKind.NotPublished)]
    [InlineData("NA", StatisticKind.NotApplicable)]
    [InlineData("Other text", StatisticKind.NotAvailable)]
    [InlineData("Different text", StatisticKind.NotAvailable)]
    [InlineData("", StatisticKind.NotAvailable)]
    public async Task GetAttendanceStatisticsAsync_parses_statistics_without_values_correctly(string statisticValue,
        StatisticKind expectedKind)
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = SchoolUrn,
                DownloadYear = "2019-2020",
                AbsencePerctot = statisticValue,
                AbsencePpersabs10 = statisticValue
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);

        var result = await sut.GetAttendanceStatisticsAsync(SchoolUrn);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[2019].OverallAbsencePercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[2019].EnrolmentsWhoArePersistentAbsenteesPercentage.Should()
            .Be(Statistic<decimal>.FromKind(expectedKind));
    }

    [Fact]
    public async Task GetAttendanceStatisticsAsync_parses_decimal_statistics_with_percent_signs_correctly()
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = SchoolUrn,
                DownloadYear = "2019-2020",
                AbsencePerctot = "10.0%",
                AbsencePpersabs10 = "11.0%"
            }
        ]);

        var sut = new PupilCensusRepository(mockDbContext.Object);

        var result = await sut.GetAttendanceStatisticsAsync(SchoolUrn);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[2019].OverallAbsencePercentage.Should().Be(new Statistic<decimal>.WithValue(10.0m));
        result[2019].EnrolmentsWhoArePersistentAbsenteesPercentage.Should().Be(new Statistic<decimal>.WithValue(11.0m));
    }

    [Fact]
    public async Task GetMostRecentPopulationStatisticsForTrustAsync_should_return_empty_when_trust_is_not_found()
    {
        var result = await _sut.GetMostRecentPopulationStatisticsForTrustAsync("9999");
        
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetMostRecentPopulationStatisticsForTrustAsync_should_return_trust_population_statistics_when_trust_is_found()
    {
        var result = await _sut.GetMostRecentPopulationStatisticsForTrustAsync(Uid);
        
        result.Should().BeEquivalentTo(_dummySchoolPopulationsForTrust);
    }
    
    [Theory]
    [InlineData("SUPP", StatisticKind.Suppressed)]
    [InlineData("NP", StatisticKind.NotPublished)]
    [InlineData("NA", StatisticKind.NotApplicable)]
    [InlineData("Other text", StatisticKind.NotAvailable)]
    [InlineData("Different text", StatisticKind.NotAvailable)]
    [InlineData("", StatisticKind.NotAvailable)]
    public async Task GetMostRecentPopulationStatisticsForTrustAsync_parses_statistics_without_values_correctly(string statisticValue, StatisticKind expectedKind)
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = AcademyUrn1,
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
        mockDbContext.GiasGroupLinks.Add(new GiasGroupLink { GroupUid = Uid, Urn = AcademyUrn1.ToString(), GroupStatusCode = "OPEN", JoinedDate = "01/01/2020" });

        var sut = new PupilCensusRepository(mockDbContext.Object);
        
        var result = await sut.GetMostRecentPopulationStatisticsForTrustAsync(Uid);
        
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[AcademyUrn1].PupilsOnRole.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[AcademyUrn1].PupilsWithEhcPlan.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[AcademyUrn1].PupilsWithEhcPlanPercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[AcademyUrn1].PupilsWithSenSupport.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[AcademyUrn1].PupilsWithSenSupportPercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[AcademyUrn1].PupilsWithEnglishAsAdditionalLanguage.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[AcademyUrn1].PupilsWithEnglishAsAdditionalLanguagePercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
        result[AcademyUrn1].PupilsEligibleForFreeSchoolMeals.Should().Be(Statistic<int>.FromKind(expectedKind));
        result[AcademyUrn1].PupilsEligibleForFreeSchoolMealsPercentage.Should().Be(Statistic<decimal>.FromKind(expectedKind));
    }

    [Fact]
    public async Task GetMostRecentPopulationStatisticsForTrustAsync_parses_decimal_statistics_with_percent_signs_correctly()
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = AcademyUrn2,
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
        mockDbContext.GiasGroupLinks.Add(new GiasGroupLink { GroupUid = Uid, Urn = AcademyUrn2.ToString(), GroupStatusCode = "OPEN", JoinedDate = "01/01/2020" });

        var sut = new PupilCensusRepository(mockDbContext.Object);
        
        var result = await sut.GetMostRecentPopulationStatisticsForTrustAsync(Uid);
        
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[AcademyUrn2].PupilsWithEhcPlanPercentage.Should().Be(new Statistic<decimal>.WithValue(10.0m));
        result[AcademyUrn2].PupilsWithSenSupportPercentage.Should().Be(new Statistic<decimal>.WithValue(11.0m));
        result[AcademyUrn2].PupilsWithEnglishAsAdditionalLanguagePercentage.Should().Be(new Statistic<decimal>.WithValue(12.0m));
        result[AcademyUrn2].PupilsEligibleForFreeSchoolMealsPercentage.Should().Be(new Statistic<decimal>.WithValue(13.0m));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(27)]
    [InlineData(44)]
    [InlineData(86)]
    public async Task GetMostRecentPopulationStatisticsForTrustAsync_handles_zero_pupils_on_role_correctly(int freeSchoolMeals)
    {
        var mockDbContext = new MockAcademiesDbContext();
        mockDbContext.EdperfFiats.AddRange([
            new EdperfFiat
            {
                Urn = AcademyUrn2,
                DownloadYear = "2019-2020",
                CensusNor = "0",
                CensusNumfsm = freeSchoolMeals.ToString(),
            }
        ]);

        mockDbContext.GiasGroupLinks.Add(new GiasGroupLink { GroupUid = Uid, Urn = AcademyUrn2.ToString(), GroupStatusCode = "OPEN", JoinedDate = "01/01/2020" });

        var sut = new PupilCensusRepository(mockDbContext.Object);
        
        var result = await sut.GetMostRecentPopulationStatisticsForTrustAsync(Uid);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[AcademyUrn2].PupilsOnRole.Should().Be(new Statistic<int>.WithValue(0));
        result[AcademyUrn2].PupilsEligibleForFreeSchoolMealsPercentage.Should().Be(new Statistic<decimal>.WithValue(0.0m));
    }
}
