using System.Numerics;
using ClosedXML.Excel;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services.ExportServices;

public class SchoolPupilsExportServiceTests
{
    private readonly SchoolPupilsExportService _sut;
    
    private readonly ISchoolService _mockSchoolService = Substitute.For<ISchoolService>();
    private readonly ISchoolPupilService _mockSchoolPupilService = Substitute.For<ISchoolPupilService>();
    private readonly IDateTimeProvider _mockDateTimeProvider = Substitute.For<IDateTimeProvider>();

    private const int SchoolUrn = 123456;
    private const int AcademyUrn = 987654;

    private readonly Dictionary<CensusYear, SchoolPopulation> _schoolPopulations =
        new()
        {
            {
                2021,
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
                2022,
                new SchoolPopulation(
                    new Statistic<int>.WithValue(200),
                    new Statistic<int>.WithValue(20),
                    new Statistic<decimal>.WithValue(10.0m),
                    new Statistic<int>.WithValue(22),
                    new Statistic<decimal>.WithValue(11.0m),
                    new Statistic<int>.WithValue(24),
                    new Statistic<decimal>.WithValue(12.0m),
                    new Statistic<int>.WithValue(26),
                    new Statistic<decimal>.WithValue(13.0m)
                )
            },
            {
                2023,
                new SchoolPopulation(
                    new Statistic<int>.WithValue(300),
                    new Statistic<int>.WithValue(30),
                    new Statistic<decimal>.WithValue(10.0m),
                    new Statistic<int>.WithValue(33),
                    new Statistic<decimal>.WithValue(11.0m),
                    new Statistic<int>.WithValue(36),
                    new Statistic<decimal>.WithValue(12.0m),
                    new Statistic<int>.WithValue(39),
                    new Statistic<decimal>.WithValue(13.0m)
                )
            },
            {
                2024,
                new SchoolPopulation(
                    new Statistic<int>.WithValue(400),
                    new Statistic<int>.WithValue(40),
                    new Statistic<decimal>.WithValue(10.0m),
                    new Statistic<int>.WithValue(44),
                    new Statistic<decimal>.WithValue(11.0m),
                    new Statistic<int>.WithValue(48),
                    new Statistic<decimal>.WithValue(12.0m),
                    new Statistic<int>.WithValue(52),
                    new Statistic<decimal>.WithValue(13.0m)
                )
            },
            {
                2025,
                SchoolPopulation.NotYetSubmitted
            }
        };

    private readonly Dictionary<CensusYear, Attendance> _attendances =
        new()
        {
            {
                2021,
                new Attendance(
                    new Statistic<decimal>.WithValue(14.0m),
                    new Statistic<decimal>.WithValue(15.0m)
                )
            },
            {
                2022,
                new Attendance(
                    new Statistic<decimal>.WithValue(16.0m),
                    new Statistic<decimal>.WithValue(17.0m)
                )
            },
            {
                2023,
                new Attendance(
                    new Statistic<decimal>.WithValue(18.0m),
                    new Statistic<decimal>.WithValue(19.0m)
                )
            },
            {
                2024,
                new Attendance(
                    new Statistic<decimal>.WithValue(20.0m),
                    new Statistic<decimal>.WithValue(21.0m)
                )
            },
            {
                2025,
                Attendance.NotYetSubmitted
            }
        };

    public SchoolPupilsExportServiceTests()
    {
        _mockSchoolService.GetSchoolSummaryAsync(SchoolUrn)
            .Returns(new SchoolSummaryServiceModel(
                SchoolUrn,
                "Test school",
                "Some kind of school",
                SchoolCategory.LaMaintainedSchool
            ));
        _mockSchoolService.GetSchoolSummaryAsync(AcademyUrn)
            .Returns(new SchoolSummaryServiceModel(
                AcademyUrn,
                "Test academy",
                "Some kind of academy",
                SchoolCategory.Academy
            ));
        
        _mockSchoolPupilService
            .GetSchoolPopulationStatisticsAsync(Arg.Any<int>(), Arg.Any<CensusYear>(), Arg.Any<CensusYear>())
            .Returns(call =>
            {
                var urn = call.ArgAt<int>(0);

                if (urn != SchoolUrn && urn != AcademyUrn)
                {
                    return Task.FromResult(new AnnualStatistics<SchoolPopulation>());
                }

                var from = call.ArgAt<CensusYear>(1);
                var to = call.ArgAt<CensusYear>(2);
                
                var result = new AnnualStatistics<SchoolPopulation>();
                foreach (var year in Enumerable.Range(from.Value, to.Value - from.Value + 1))
                {
                    var schoolPopulation = _schoolPopulations.TryGetValue(year, out var value)
                        ? value
                        : SchoolPopulation.Unknown;
                    result.Add(year, schoolPopulation);
                }
                
                return Task.FromResult(result);
            });

        _mockSchoolPupilService
            .GetAttendanceStatisticsAsync(Arg.Any<int>(), Arg.Any<CensusYear>(), Arg.Any<CensusYear>())
            .Returns(call =>
            {
                var urn = call.ArgAt<int>(0);

                if (urn != SchoolUrn && urn != AcademyUrn)
                {
                    return Task.FromResult(new AnnualStatistics<Attendance>());
                }

                var from = call.ArgAt<CensusYear>(1);
                var to = call.ArgAt<CensusYear>(2);
                
                var result = new AnnualStatistics<Attendance>();
                foreach (var year in Enumerable.Range(from.Value, to.Value - from.Value + 1))
                {
                    var attendance = _attendances.TryGetValue(year, out var value)
                        ? value
                        : Attendance.Unknown;
                    result.Add(year, attendance);
                }
                
                return Task.FromResult(result);
            });

        _mockDateTimeProvider.Today.Returns(new DateTime(2025, 1, 1));

        _sut = new SchoolPupilsExportService(_mockSchoolService, _mockSchoolPupilService, _mockDateTimeProvider);
    }

    [Fact]
    public async Task BuildAsync_should_generate_correct_headers()
    {
        var worksheet = await GetWorksheetForUrn(SchoolUrn);

        worksheet.AssertSpreadsheetMatches(
            4,
            [
                "Year",
                "Number of pupils on roll (NOR)",
                "Number of eligible pupils with EHC plan",
                "Percentage of eligible pupils with EHC plan",
                "Number of eligible pupils with SEN support",
                "Percentage of eligible pupils with SEN support",
                "Number of pupils with English as an additional language",
                "Percentage of pupils with English as an additional language",
                "Number of pupils eligible for free school meals",
                "Percentage of pupils eligible for free school meals",
                "Percentage of overall absence",
                "Percentage of enrolments who are persistent absentees"
            ]
        );
    }

    [Fact]
    public async Task BuildAsync_throws_DataIntegrityException_when_school_is_not_found()
    {
        var result = () => _sut.BuildAsync(999999);
        await result.Should().ThrowAsync<DataIntegrityException>()
            .WithMessage("School with URN '999999' was not found.");
    }

    [Fact]
    public async Task BuildAsync_includes_correct_preamble_for_school()
    {
        var worksheet = await GetWorksheetForUrn(SchoolUrn);

        worksheet.CellValue(1, 1).Should().Be("School name");
        worksheet.CellValue(1, 2).Should().Be("Test school");
        worksheet.CellValue(2, 1).Should().Be("URN");
        worksheet.CellValue(2, 2).Should().Be(SchoolUrn.ToString());
    }

    [Fact]
    public async Task BuildAsync_includes_correct_preamble_for_academy()
    {
        var worksheet = await GetWorksheetForUrn(AcademyUrn);
        
        worksheet.CellValue(1, 1).Should().Be("Academy name");
        worksheet.CellValue(1, 2).Should().Be("Test academy");
        worksheet.CellValue(2, 1).Should().Be("URN");
        worksheet.CellValue(2, 2).Should().Be(AcademyUrn.ToString());
    }

    [Fact]
    public async Task BuildAsync_includes_expected_data()
    {
        var worksheet = await GetWorksheetForUrn(SchoolUrn);

        var expectedRow2021 = new ExpectedRow(
            9,
            2021,
            100,
            10,
            0.10,
            11,
            0.11,
            12,
            0.12,
            13,
            0.13,
            0.14,
            0.15
        );
        var expectedRow2022 = new ExpectedRow(
            8,
            2022,
            200,
            20,
            0.10,
            22,
            0.11,
            24,
            0.12,
            26,
            0.13,
            0.16,
            0.17
        );
        var expectedRow2023 = new ExpectedRow(
            7,
            2023,
            300,
            30,
            0.10,
            33,
            0.11,
            36,
            0.12,
            39,
            0.13,
            0.18,
            0.19
        );
        var expectedRow2024 = new ExpectedRow(
            6,
            2024,
            400,
            40,
            0.10,
            44,
            0.11,
            48,
            0.12,
            52,
            0.13,
            0.20,
            0.21
        );
        var expectedRow2025 = new ExpectedRow(5, 2025, StatisticKind.NotYetSubmitted);

        AssertWorksheetContainsExpectedRows(
            worksheet,
            expectedRow2021,
            expectedRow2022,
            expectedRow2023,
            expectedRow2024,
            expectedRow2025
        );
    }

    [Fact]
    public async Task BuildAsync_includes_expected_data_when_statistics_have_no_value()
    {
        _mockSchoolPupilService
            .GetSchoolPopulationStatisticsAsync(Arg.Any<int>(), Arg.Any<CensusYear>(), Arg.Any<CensusYear>())
            .Returns(new AnnualStatistics<SchoolPopulation>
            {
                {
                    2021,
                    new SchoolPopulation(
                        Statistic<int>.FromKind(StatisticKind.Suppressed),
                        Statistic<int>.FromKind(StatisticKind.Suppressed),
                        Statistic<decimal>.FromKind(StatisticKind.Suppressed),
                        Statistic<int>.FromKind(StatisticKind.Suppressed),
                        Statistic<decimal>.FromKind(StatisticKind.Suppressed),
                        Statistic<int>.FromKind(StatisticKind.Suppressed),
                        Statistic<decimal>.FromKind(StatisticKind.Suppressed),
                        Statistic<int>.FromKind(StatisticKind.Suppressed),
                        Statistic<decimal>.FromKind(StatisticKind.Suppressed)
                    )
                },
                {
                    2022,
                    new SchoolPopulation(
                        Statistic<int>.FromKind(StatisticKind.NotPublished),
                        Statistic<int>.FromKind(StatisticKind.NotPublished),
                        Statistic<decimal>.FromKind(StatisticKind.NotPublished),
                        Statistic<int>.FromKind(StatisticKind.NotPublished),
                        Statistic<decimal>.FromKind(StatisticKind.NotPublished),
                        Statistic<int>.FromKind(StatisticKind.NotPublished),
                        Statistic<decimal>.FromKind(StatisticKind.NotPublished),
                        Statistic<int>.FromKind(StatisticKind.NotPublished),
                        Statistic<decimal>.FromKind(StatisticKind.NotPublished)
                    )
                },
                {
                    2023,
                    new SchoolPopulation(
                        Statistic<int>.FromKind(StatisticKind.NotApplicable),
                        Statistic<int>.FromKind(StatisticKind.NotApplicable),
                        Statistic<decimal>.FromKind(StatisticKind.NotApplicable),
                        Statistic<int>.FromKind(StatisticKind.NotApplicable),
                        Statistic<decimal>.FromKind(StatisticKind.NotApplicable),
                        Statistic<int>.FromKind(StatisticKind.NotApplicable),
                        Statistic<decimal>.FromKind(StatisticKind.NotApplicable),
                        Statistic<int>.FromKind(StatisticKind.NotApplicable),
                        Statistic<decimal>.FromKind(StatisticKind.NotApplicable)
                    )
                },
                {
                    2024,
                    new SchoolPopulation(
                        Statistic<int>.FromKind(StatisticKind.NotAvailable),
                        Statistic<int>.FromKind(StatisticKind.NotAvailable),
                        Statistic<decimal>.FromKind(StatisticKind.NotAvailable),
                        Statistic<int>.FromKind(StatisticKind.NotAvailable),
                        Statistic<decimal>.FromKind(StatisticKind.NotAvailable),
                        Statistic<int>.FromKind(StatisticKind.NotAvailable),
                        Statistic<decimal>.FromKind(StatisticKind.NotAvailable),
                        Statistic<int>.FromKind(StatisticKind.NotAvailable),
                        Statistic<decimal>.FromKind(StatisticKind.NotAvailable)
                    )
                },
                {
                    2025,
                    SchoolPopulation.NotYetSubmitted
                }
            });
        _mockSchoolPupilService
            .GetAttendanceStatisticsAsync(Arg.Any<int>(), Arg.Any<CensusYear>(), Arg.Any<CensusYear>())
            .Returns(new AnnualStatistics<Attendance>
            {
                {
                    2021,
                    new Attendance(
                        Statistic<decimal>.FromKind(StatisticKind.Suppressed),
                        Statistic<decimal>.FromKind(StatisticKind.Suppressed)
                    )
                },
                {
                    2022,
                    new Attendance(
                        Statistic<decimal>.FromKind(StatisticKind.NotPublished),
                        Statistic<decimal>.FromKind(StatisticKind.NotPublished)
                    )
                },
                {
                    2023,
                    new Attendance(
                        Statistic<decimal>.FromKind(StatisticKind.NotApplicable),
                        Statistic<decimal>.FromKind(StatisticKind.NotApplicable)
                    )
                },
                {
                    2024,
                    new Attendance(
                        Statistic<decimal>.FromKind(StatisticKind.NotAvailable),
                        Statistic<decimal>.FromKind(StatisticKind.NotAvailable)
                    )
                },
                {
                    2025,
                    Attendance.NotYetSubmitted
                }
            });
        
        var worksheet = await GetWorksheetForUrn(SchoolUrn);

        var expectedRow2021 = new ExpectedRow(9, 2021, StatisticKind.Suppressed);
        var expectedRow2022 = new ExpectedRow(8, 2022, StatisticKind.NotPublished);
        var expectedRow2023 = new ExpectedRow(7, 2023, StatisticKind.NotApplicable);
        var expectedRow2024 = new ExpectedRow(6, 2024, StatisticKind.NotAvailable);
        var expectedRow2025 = new ExpectedRow(5, 2025, StatisticKind.NotYetSubmitted);

        AssertWorksheetContainsExpectedRows(
            worksheet,
            expectedRow2021,
            expectedRow2022,
            expectedRow2023,
            expectedRow2024,
            expectedRow2025
        );
    }

    private async Task<IXLWorksheet> GetWorksheetForUrn(int urn)
    {
        var result = await _sut.BuildAsync(urn);
        var workbook = new XLWorkbook(new MemoryStream(result));
        return workbook.Worksheet("Pupil population");
    }

    private static void AssertWorksheetContainsExpectedRows(IXLWorksheet worksheet, params ExpectedRow[] expectedRows)
    {
        foreach (var expectedRow in expectedRows)
        {
            AssertWorksheetContainsExpectedRow(worksheet, expectedRow);
        }
    }

    private static void AssertWorksheetContainsExpectedRow(IXLWorksheet worksheet, ExpectedRow expectedRow)
    {
        var row = expectedRow.RowNumber;
        AssertCellContainsNumber(worksheet, row, ExportColumns.SchoolPopulationColumns.Year, expectedRow.Year);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.NumberOfPupilsOnRole, expectedRow.NumberOfPupilsOnRole);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.NumberOfEligiblePupilsWithEhcPlan, expectedRow.NumberOfEligiblePupilsWithEhcPlan);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.PercentageOfEligiblePupilsWithEhcPlan, expectedRow.PercentageOfEligiblePupilsWithEhcPlan);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.NumberOfEligiblePupilsWithSenSupport, expectedRow.NumberOfEligiblePupilsWithSenSupport);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.PercentageOfEligiblePupilsWithSenSupport, expectedRow.PercentageOfEligiblePupilsWithSenSupport);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.NumberOfPupilsWithEnglishAsAnAdditionalLanguage, expectedRow.NumberOfPupilsWithEnglishAsAnAdditionalLanguage);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.PercentageOfPupilsWithEnglishAsAnAdditionalLanguage, expectedRow.PercentageOfPupilsWithEnglishAsAnAdditionalLanguage);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.NumberOfPupilsEligibleForFreeSchoolMeals, expectedRow.NumberOfPupilsEligibleForFreeSchoolMeals);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.PercentageOfPupilsEligibleForFreeSchoolMeals, expectedRow.PercentageOfPupilsEligibleForFreeSchoolMeals);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.PercentageOfOverallAbsence, expectedRow.PercentageOfOverallAbsence);
        AssertCellContainsStatistic(worksheet, row, ExportColumns.SchoolPopulationColumns.PercentageOfEnrolmentsWhoArePersistentAbsentees, expectedRow.PercentageOfEnrolmentsWhoArePersistentAbsentees);
    }

    private static void AssertCellContainsStatistic<T>(
        IXLWorksheet worksheet,
        int row,
        ExportColumns.SchoolPopulationColumns column,
        Statistic<T> expectedValue
    ) where T : INumber<T>
    {
        if (expectedValue is Statistic<T>.WithValue value)
        {
            AssertCellContainsNumber(worksheet, row, column, value.Value);
        }
        else
        {
            AssertCellContainsText(worksheet, row, column, expectedValue.DisplayValue());
        }
    }

    private static void AssertCellContainsText(IXLWorksheet worksheet, int row, ExportColumns.SchoolPopulationColumns column, string expectedValue)
    {
        worksheet.CellValue(row, column).Should().Be(expectedValue);
    }

    private static void AssertCellContainsNumber<T>(IXLWorksheet worksheet, int row, ExportColumns.SchoolPopulationColumns column, T expectedValue)
        where T : INumber<T>
    {
        worksheet.Cell(row, column).DataType.Should().Be(XLDataType.Number);
        worksheet.Cell(row, column).GetValue<T>().Should().Be(expectedValue);
    }

    private record ExpectedRow(
        int RowNumber,
        int Year,
        Statistic<int> NumberOfPupilsOnRole,
        Statistic<int> NumberOfEligiblePupilsWithEhcPlan,
        Statistic<double> PercentageOfEligiblePupilsWithEhcPlan,
        Statistic<int> NumberOfEligiblePupilsWithSenSupport,
        Statistic<double> PercentageOfEligiblePupilsWithSenSupport,
        Statistic<int> NumberOfPupilsWithEnglishAsAnAdditionalLanguage,
        Statistic<double> PercentageOfPupilsWithEnglishAsAnAdditionalLanguage,
        Statistic<int> NumberOfPupilsEligibleForFreeSchoolMeals,
        Statistic<double> PercentageOfPupilsEligibleForFreeSchoolMeals,
        Statistic<double> PercentageOfOverallAbsence,
        Statistic<double> PercentageOfEnrolmentsWhoArePersistentAbsentees
    )
    {
        public ExpectedRow(
            int rowNumber,
            int year,
            int numberOfPupilsOnRole,
            int numberOfEligiblePupilsWithEhcPlan,
            double percentageOfEligiblePupilsWithEhcPlan,
            int numberOfEligiblePupilsWithSenSupport,
            double percentageOfEligiblePupilsWithSenSupport,
            int numberOfPupilsWithEnglishAsAnAdditionalLanguage,
            double percentageOfPupilsWithEnglishAsAnAdditionalLanguage,
            int numberOfPupilsEligibleForFreeSchoolMeals,
            double percentageOfPupilsEligibleForFreeSchoolMeals,
            double percentageOfOverallAbsence,
            double percentageOfEnrolmentsWhoArePersistentAbsentees
        ) : this(
            rowNumber,
            year,
            new Statistic<int>.WithValue(numberOfPupilsOnRole),
            new Statistic<int>.WithValue(numberOfEligiblePupilsWithEhcPlan),
            new Statistic<double>.WithValue(percentageOfEligiblePupilsWithEhcPlan),
            new Statistic<int>.WithValue(numberOfEligiblePupilsWithSenSupport),
            new Statistic<double>.WithValue(percentageOfEligiblePupilsWithSenSupport),
            new Statistic<int>.WithValue(numberOfPupilsWithEnglishAsAnAdditionalLanguage),
            new Statistic<double>.WithValue(percentageOfPupilsWithEnglishAsAnAdditionalLanguage),
            new Statistic<int>.WithValue(numberOfPupilsEligibleForFreeSchoolMeals),
            new Statistic<double>.WithValue(percentageOfPupilsEligibleForFreeSchoolMeals),
            new Statistic<double>.WithValue(percentageOfOverallAbsence),
            new Statistic<double>.WithValue(percentageOfEnrolmentsWhoArePersistentAbsentees)
        )
        {
        }

        public ExpectedRow(int rowNumber, int year, StatisticKind statisticKind) : this(
            rowNumber,
            year,
            Statistic<int>.FromKind(statisticKind),
            Statistic<int>.FromKind(statisticKind),
            Statistic<double>.FromKind(statisticKind),
            Statistic<int>.FromKind(statisticKind),
            Statistic<double>.FromKind(statisticKind),
            Statistic<int>.FromKind(statisticKind),
            Statistic<double>.FromKind(statisticKind),
            Statistic<int>.FromKind(statisticKind),
            Statistic<double>.FromKind(statisticKind),
            Statistic<double>.FromKind(statisticKind),
            Statistic<double>.FromKind(statisticKind)
        )
        {
        }
    }
}
