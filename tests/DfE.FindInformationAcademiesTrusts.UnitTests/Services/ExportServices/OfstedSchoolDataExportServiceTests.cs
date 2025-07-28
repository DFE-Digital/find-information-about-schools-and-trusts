using ClosedXML.Excel;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services.ExportServices;

public class OfstedSchoolDataExportServiceTests
{
    private const int SchoolUrn = 123456;
    private const int AcademyUrn = 234567;

    private readonly ISchoolService _mockSchoolService = Substitute.For<ISchoolService>();

    private readonly ISchoolOverviewDetailsService _mockSchoolOverviewDetailsService =
        Substitute.For<ISchoolOverviewDetailsService>();

    private readonly OfstedSchoolDataExportService _sut;

    private readonly SchoolSummaryServiceModel _schoolSummary;
    private readonly SchoolSummaryServiceModel _academySummary;
    private readonly SchoolOverviewServiceModel _schoolOverview;
    private readonly SchoolOverviewServiceModel _academyOverview;
    private readonly OfstedHeadlineGradesServiceModel _headlineGrades;

    public OfstedSchoolDataExportServiceTests()
    {
        _schoolSummary =
            new SchoolSummaryServiceModel(SchoolUrn, "Test School", "Some School Type",
                SchoolCategory.LaMaintainedSchool);
        _academySummary =
            new SchoolSummaryServiceModel(AcademyUrn, "Test Academy", "Some Academy Type", SchoolCategory.Academy);

        _schoolOverview = new SchoolOverviewServiceModel("Test School", "1 Some Street", "East of England", "Essex",
            "Primary", new AgeRange(3, 11), NurseryProvision.HasClasses);
        _academyOverview = new SchoolOverviewServiceModel("Test Academy", "2 Another Place", "South West",
                "Devon and Cornwall", "Secondary", new AgeRange(11, 18), NurseryProvision.NoClasses)
            { DateJoinedTrust = DateOnly.Parse("2014-09-01") };

        _headlineGrades = new OfstedHeadlineGradesServiceModel(
            new OfstedShortInspection(DateTime.Parse("2025-07-01"), "School remains Good"),
            new OfstedFullInspectionSummary(DateTime.Parse("2020-07-01"), OfstedRatingScore.Good),
            new OfstedFullInspectionSummary(DateTime.Parse("2010-07-01"), OfstedRatingScore.RequiresImprovement));

        _mockSchoolService.GetSchoolSummaryAsync(SchoolUrn).Returns(_schoolSummary);
        _mockSchoolService.GetSchoolSummaryAsync(AcademyUrn).Returns(_academySummary);

        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(SchoolUrn, SchoolCategory.LaMaintainedSchool)!
            .Returns(_schoolOverview);
        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(AcademyUrn, SchoolCategory.Academy)!.Returns(
            _academyOverview);

        _mockSchoolService.GetOfstedHeadlineGrades(SchoolUrn)!.Returns(_headlineGrades);
        _mockSchoolService.GetOfstedHeadlineGrades(AcademyUrn)!.Returns(_headlineGrades);

        _sut = new OfstedSchoolDataExportService(_mockSchoolService, _mockSchoolOverviewDetailsService);
    }

    [Fact]
    public async Task BuildAsync_should_generate_correct_headers()
    {
        var result = await GetWorksheetAsync(123456);

        result.AssertSpreadsheetMatches(3, ["Inspection type", "Date of inspection", "Grade"]);
    }

    [Fact]
    public async Task BuildAsync_when_school_is_not_found_then_throw_DataIntegrityException()
    {
        var unknownUrn = 999111;

        var result = async () => { await _sut.BuildAsync(unknownUrn); };

        await result.Should().ThrowAsync<DataIntegrityException>()
            .WithMessage("School summary not found for urn: 999111");
    }

    [Fact]
    public async Task BuildAsync_when_school_is_la_maintained_then_school_preamble_is_correct()
    {
        var result = await GetWorksheetAsync(SchoolUrn);

        result.CellValue(1, 1).Should().Be(_schoolOverview.Name);
        result.CellValue(2, 1).Should().Be("Not part of a trust");
    }

    [Fact]
    public async Task BuildAsync_when_school_is_academy_then_school_preamble_is_correct()
    {
        var result = await GetWorksheetAsync(AcademyUrn);

        result.CellValue(1, 1).Should().Be(_academyOverview.Name);
        result.CellValue(2, 1).Should().Be("Joined trust on 01/09/2014");
    }

    [Fact]
    public async Task BuildAsync_includes_expected_data()
    {
        var result = await GetWorksheetAsync(SchoolUrn);

        AssertResultContainsExpectedRow(result, 4, "Recent short inspection",
            _headlineGrades.ShortInspection.InspectionDate, _headlineGrades.ShortInspection.InspectionOutcome);

        AssertResultContainsExpectedRow(result, 5, "Current inspection",
            _headlineGrades.CurrentInspection.InspectionDate, "Good");
        AssertResultContainsExpectedRow(result, 6, "Previous inspection",
            _headlineGrades.PreviousInspection.InspectionDate, "Requires improvement");
    }

    [Fact]
    public async Task BuildAsync_when_no_recent_short_inspection_then_includes_expected_data()
    {
        _mockSchoolService.GetOfstedHeadlineGrades(SchoolUrn)!.Returns(_headlineGrades with
        {
            ShortInspection = OfstedShortInspection.Unknown
        });

        var result = await GetWorksheetAsync(SchoolUrn);

        AssertResultContainsExpectedRow(result, 4, "Current inspection",
            _headlineGrades.CurrentInspection.InspectionDate, "Good");
        AssertResultContainsExpectedRow(result, 5, "Previous inspection",
            _headlineGrades.PreviousInspection.InspectionDate, "Requires improvement");
    }

    [Fact]
    public async Task BuildAsync_when_inspection_dates_are_missing_then_includes_expected_data()
    {
        _mockSchoolService.GetOfstedHeadlineGrades(SchoolUrn)!.Returns(
            _headlineGrades with
            {
                CurrentInspection = _headlineGrades.CurrentInspection with { InspectionDate = null },
                PreviousInspection = _headlineGrades.PreviousInspection with { InspectionDate = null }
            }
        );

        var result = await GetWorksheetAsync(SchoolUrn);

        AssertResultContainsExpectedRow(result, 4, "Current inspection", null, "Good");
        AssertResultContainsExpectedRow(result, 5, "Previous inspection", null, "Requires improvement");
    }

    [Fact]
    public async Task BuildAsync_when_short_inspection_grade_is_missing_then_includes_expected_data()
    {
        _mockSchoolService.GetOfstedHeadlineGrades(SchoolUrn)!.Returns(
            _headlineGrades with
            {
                ShortInspection = _headlineGrades.ShortInspection with { InspectionOutcome = null }
            }
        );

        var result = await GetWorksheetAsync(SchoolUrn);

        AssertResultContainsExpectedRow(result, 4, "Recent short inspection",
            _headlineGrades.ShortInspection.InspectionDate, null);

        AssertResultContainsExpectedRow(result, 5, "Current inspection",
            _headlineGrades.CurrentInspection.InspectionDate, "Good");
        AssertResultContainsExpectedRow(result, 6, "Previous inspection",
            _headlineGrades.PreviousInspection.InspectionDate, "Requires improvement");
    }

    private async Task<IXLWorksheet> GetWorksheetAsync(int urn)
    {
        var bytes = await _sut.BuildAsync(urn);
        var workbook = new XLWorkbook(new MemoryStream(bytes));
        var worksheet = workbook.Worksheet("Ofsted");

        worksheet.Should().NotBeNull();

        return worksheet;
    }

    private static void AssertResultContainsExpectedRow(IXLWorksheet result, int row, string expectedInspectionType,
        DateTime? expectedInspectionDate, string? expectedGrade)
    {
        result.CellValue(row, ExportColumns.OfstedSchoolColumns.InspectionType).Should().Be(expectedInspectionType);

        if (expectedInspectionDate is null)
        {
            result.CellValue(row, ExportColumns.OfstedSchoolColumns.DateOfInspection).Should().Be(string.Empty);
        }
        else
        {
            result.Cell(row, ExportColumns.OfstedSchoolColumns.DateOfInspection).DataType.Should()
                .Be(XLDataType.DateTime);
            result.Cell(row, ExportColumns.OfstedSchoolColumns.DateOfInspection).GetValue<DateTime>().Should()
                .Be(expectedInspectionDate);
        }

        result.CellValue(row, ExportColumns.OfstedSchoolColumns.Grade).Should().Be(expectedGrade ?? string.Empty);
    }
}
