using ClosedXML.Excel;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
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
    private readonly SchoolOfstedServiceModel _schoolOfstedRatings;
    private readonly SchoolOfstedServiceModel _academyOfstedRatings;

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

        _schoolOfstedRatings = new SchoolOfstedServiceModel(
            SchoolUrn.ToString(),
            "Test School",
            null,
            new OfstedShortInspection(DateTime.Parse("2025-07-01"), "School remains Good"),
            new OfstedRating(
                OfstedRatingScore.RequiresImprovement,
                OfstedRatingScore.RequiresImprovement,
                OfstedRatingScore.RequiresImprovement,
                OfstedRatingScore.RequiresImprovement,
                OfstedRatingScore.RequiresImprovement,
                OfstedRatingScore.RequiresImprovement,
                OfstedRatingScore.RequiresImprovement,
                CategoriesOfConcern.NoticeToImprove,
                SafeguardingScore.No,
                DateTime.Parse("2010-07-01")
            ),
            new OfstedRating(
                OfstedRatingScore.Good,
                OfstedRatingScore.Good,
                OfstedRatingScore.Good,
                OfstedRatingScore.Good,
                OfstedRatingScore.Good,
                OfstedRatingScore.Good,
                OfstedRatingScore.Good,
                CategoriesOfConcern.NoConcerns,
                SafeguardingScore.Yes,
                DateTime.Parse("2020-07-01")
            ), false
        );

        _academyOfstedRatings = _schoolOfstedRatings with
        {
            Urn = AcademyUrn.ToString(),
            EstablishmentName = "Test Academy",
            DateAcademyJoinedTrust = DateTime.Parse("2014-09-01")
        };

        _mockSchoolService.GetSchoolSummaryAsync(SchoolUrn).Returns(_schoolSummary);
        _mockSchoolService.GetSchoolSummaryAsync(AcademyUrn).Returns(_academySummary);

        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(SchoolUrn, SchoolCategory.LaMaintainedSchool)!
            .Returns(_schoolOverview);
        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(AcademyUrn, SchoolCategory.Academy)!.Returns(
            _academyOverview);

        _mockSchoolService.GetSchoolOfstedRatingsAsync(SchoolUrn)!.Returns(_schoolOfstedRatings);
        _mockSchoolService.GetSchoolOfstedRatingsAsync(AcademyUrn)!.Returns(_academyOfstedRatings);

        _sut = new OfstedSchoolDataExportService(_mockSchoolService, _mockSchoolOverviewDetailsService);
    }

    [Fact]
    public async Task BuildAsync_should_generate_correct_headers()
    {
        var result = await GetWorksheetAsync(123456);

        result.AssertSpreadsheetMatches(
            3,
            [
                "Inspection type",
                "Date of inspection",
                "Grade",
                "Quality of education",
                "Behaviour and attitudes",
                "Personal development",
                "Leadership and management",
                "Early years provision",
                "Sixth form provision",
                "Effective safeguarding",
                "Category of concern",
                "Before or after joining the trust"
            ]);
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
    public async Task BuildAsync_includes_expected_data_for_school()
    {
        var result = await GetWorksheetAsync(SchoolUrn);

        AssertResultContainsExpectedRow(result, 4,
            new ExpectedRow(_schoolOfstedRatings.ShortInspection, BeforeOrAfterJoining.NotApplicable));

        AssertResultContainsExpectedRow(result, 5,
            new ExpectedRow("Current inspection", _schoolOfstedRatings.CurrentOfstedRating,
                BeforeOrAfterJoining.NotApplicable));
        AssertResultContainsExpectedRow(result, 6,
            new ExpectedRow("Previous inspection", _schoolOfstedRatings.PreviousOfstedRating,
                BeforeOrAfterJoining.NotApplicable));
    }

    [Fact]
    public async Task BuildAsync_includes_expected_data_for_academy()
    {
        var result = await GetWorksheetAsync(AcademyUrn);

        AssertResultContainsExpectedRow(result, 4,
            new ExpectedRow(_academyOfstedRatings.ShortInspection, BeforeOrAfterJoining.After));

        AssertResultContainsExpectedRow(result, 5,
            new ExpectedRow("Current inspection", _academyOfstedRatings.CurrentOfstedRating,
                BeforeOrAfterJoining.After));
        AssertResultContainsExpectedRow(result, 6,
            new ExpectedRow("Previous inspection", _academyOfstedRatings.PreviousOfstedRating,
                BeforeOrAfterJoining.Before));
    }

    [Fact]
    public async Task BuildAsync_when_no_recent_short_inspection_then_includes_expected_data_for_school()
    {
        _mockSchoolService.GetSchoolOfstedRatingsAsync(SchoolUrn)!.Returns(_schoolOfstedRatings with
        {
            ShortInspection = OfstedShortInspection.Unknown
        });

        var result = await GetWorksheetAsync(SchoolUrn);

        AssertResultContainsExpectedRow(result, 4,
            new ExpectedRow("Current inspection", _schoolOfstedRatings.CurrentOfstedRating,
                BeforeOrAfterJoining.NotApplicable));
        AssertResultContainsExpectedRow(result, 5,
            new ExpectedRow("Previous inspection", _schoolOfstedRatings.PreviousOfstedRating,
                BeforeOrAfterJoining.NotApplicable));
    }

    [Fact]
    public async Task BuildAsync_when_no_recent_short_inspection_then_includes_expected_data_for_academy()
    {
        _mockSchoolService.GetSchoolOfstedRatingsAsync(AcademyUrn)!.Returns(_academyOfstedRatings with
        {
            ShortInspection = OfstedShortInspection.Unknown
        });

        var result = await GetWorksheetAsync(AcademyUrn);

        AssertResultContainsExpectedRow(result, 4,
            new ExpectedRow("Current inspection", _academyOfstedRatings.CurrentOfstedRating,
                BeforeOrAfterJoining.After));
        AssertResultContainsExpectedRow(result, 5,
            new ExpectedRow("Previous inspection", _academyOfstedRatings.PreviousOfstedRating,
                BeforeOrAfterJoining.Before));
    }

    [Fact]
    public async Task BuildAsync_when_inspection_dates_are_missing_then_includes_expected_data_for_school()
    {
        var currentOfstedRating = _schoolOfstedRatings.CurrentOfstedRating with { InspectionDate = null };
        var previousOfstedRating = _schoolOfstedRatings.PreviousOfstedRating with { InspectionDate = null };

        _mockSchoolService.GetSchoolOfstedRatingsAsync(SchoolUrn)!.Returns(
            _schoolOfstedRatings with
            {
                CurrentOfstedRating = currentOfstedRating,
                PreviousOfstedRating = previousOfstedRating
            }
        );

        var result = await GetWorksheetAsync(SchoolUrn);

        AssertResultContainsExpectedRow(result, 4,
            new ExpectedRow("Current inspection", currentOfstedRating, BeforeOrAfterJoining.NotApplicable));
        AssertResultContainsExpectedRow(result, 5,
            new ExpectedRow("Previous inspection", previousOfstedRating, BeforeOrAfterJoining.NotApplicable));
    }

    [Fact]
    public async Task BuildAsync_when_inspection_dates_are_missing_then_includes_expected_data_for_academy()
    {
        var currentOfstedRating = _academyOfstedRatings.CurrentOfstedRating with { InspectionDate = null };
        var previousOfstedRating = _academyOfstedRatings.PreviousOfstedRating with { InspectionDate = null };

        _mockSchoolService.GetSchoolOfstedRatingsAsync(AcademyUrn)!.Returns(
            _academyOfstedRatings with
            {
                CurrentOfstedRating = currentOfstedRating,
                PreviousOfstedRating = previousOfstedRating
            }
        );

        var result = await GetWorksheetAsync(AcademyUrn);

        AssertResultContainsExpectedRow(result, 4,
            new ExpectedRow("Current inspection", currentOfstedRating, BeforeOrAfterJoining.NotApplicable));
        AssertResultContainsExpectedRow(result, 5,
            new ExpectedRow("Previous inspection", previousOfstedRating, BeforeOrAfterJoining.NotApplicable));
    }

    [Fact]
    public async Task BuildAsync_when_short_inspection_grade_is_missing_then_includes_expected_data_for_school()
    {
        var shortInspection = _schoolOfstedRatings.ShortInspection with { InspectionOutcome = null };

        _mockSchoolService.GetSchoolOfstedRatingsAsync(SchoolUrn)!.Returns(
            _schoolOfstedRatings with
            {
                ShortInspection = shortInspection
            }
        );

        var result = await GetWorksheetAsync(SchoolUrn);

        AssertResultContainsExpectedRow(result, 4,
            new ExpectedRow(shortInspection, BeforeOrAfterJoining.NotApplicable));

        AssertResultContainsExpectedRow(result, 5,
            new ExpectedRow("Current inspection", _schoolOfstedRatings.CurrentOfstedRating,
                BeforeOrAfterJoining.NotApplicable));
        AssertResultContainsExpectedRow(result, 6,
            new ExpectedRow("Previous inspection", _schoolOfstedRatings.PreviousOfstedRating,
                BeforeOrAfterJoining.NotApplicable));
    }

    [Fact]
    public async Task BuildAsync_when_short_inspection_grade_is_missing_then_includes_expected_data_for_academy()
    {
        var shortInspection = _academyOfstedRatings.ShortInspection with { InspectionOutcome = null };

        _mockSchoolService.GetSchoolOfstedRatingsAsync(AcademyUrn)!.Returns(
            _academyOfstedRatings with
            {
                ShortInspection = shortInspection
            }
        );

        var result = await GetWorksheetAsync(AcademyUrn);

        AssertResultContainsExpectedRow(result, 4, new ExpectedRow(shortInspection, BeforeOrAfterJoining.After));

        AssertResultContainsExpectedRow(result, 5,
            new ExpectedRow("Current inspection", _schoolOfstedRatings.CurrentOfstedRating,
                BeforeOrAfterJoining.After));
        AssertResultContainsExpectedRow(result, 6,
            new ExpectedRow("Previous inspection", _schoolOfstedRatings.PreviousOfstedRating,
                BeforeOrAfterJoining.Before));
    }

    private async Task<IXLWorksheet> GetWorksheetAsync(int urn)
    {
        var bytes = await _sut.BuildAsync(urn);
        var workbook = new XLWorkbook(new MemoryStream(bytes));
        var worksheet = workbook.Worksheet("Ofsted");

        worksheet.Should().NotBeNull();

        return worksheet;
    }

    private record ExpectedRow(
        string InspectionType,
        DateTime? InspectionDate,
        string? Grade,
        OfstedRatingScore QualityOfEducation,
        OfstedRatingScore BehaviourAndAttitudes,
        OfstedRatingScore PersonalDevelopment,
        OfstedRatingScore LeadershipAndManagement,
        OfstedRatingScore EarlyYearsProvision,
        OfstedRatingScore SixthFormProvision,
        SafeguardingScore EffectiveSafeguarding,
        CategoriesOfConcern CategoryOfConcern,
        BeforeOrAfterJoining BeforeOrAfterJoiningTrust
    )
    {
        public ExpectedRow(string inspectionType, OfstedRating ofstedRating,
            BeforeOrAfterJoining beforeOrAfterJoiningTrust)
            : this(
                inspectionType,
                ofstedRating.InspectionDate,
                ofstedRating.OverallEffectiveness.ToDisplayString(false),
                ofstedRating.QualityOfEducation,
                ofstedRating.BehaviourAndAttitudes,
                ofstedRating.PersonalDevelopment,
                ofstedRating.EffectivenessOfLeadershipAndManagement,
                ofstedRating.EarlyYearsProvision,
                ofstedRating.SixthFormProvision,
                ofstedRating.SafeguardingIsEffective,
                ofstedRating.CategoryOfConcern,
                beforeOrAfterJoiningTrust)
        {
        }

        public ExpectedRow(OfstedShortInspection ofstedShortInspection, BeforeOrAfterJoining beforeOrAfterJoiningTrust)
            : this(
                "Recent short inspection",
                ofstedShortInspection.InspectionDate,
                ofstedShortInspection.InspectionOutcome,
                OfstedRatingScore.SingleHeadlineGradeNotAvailable,
                OfstedRatingScore.SingleHeadlineGradeNotAvailable,
                OfstedRatingScore.SingleHeadlineGradeNotAvailable,
                OfstedRatingScore.SingleHeadlineGradeNotAvailable,
                OfstedRatingScore.SingleHeadlineGradeNotAvailable,
                OfstedRatingScore.SingleHeadlineGradeNotAvailable,
                SafeguardingScore.Unknown,
                CategoriesOfConcern.Unknown,
                beforeOrAfterJoiningTrust)
        {
        }
    }

    private static void AssertResultContainsExpectedRow(IXLWorksheet result, int row, ExpectedRow expected)
    {
        result.CellValue(row, ExportColumns.OfstedSchoolColumns.InspectionType).Should().Be(expected.InspectionType);

        if (expected.InspectionDate is null)
        {
            result.CellValue(row, ExportColumns.OfstedSchoolColumns.DateOfInspection).Should().Be(string.Empty);
        }
        else
        {
            result.Cell(row, ExportColumns.OfstedSchoolColumns.DateOfInspection).DataType.Should()
                .Be(XLDataType.DateTime);
            result.Cell(row, ExportColumns.OfstedSchoolColumns.DateOfInspection).GetValue<DateTime>().Should()
                .Be(expected.InspectionDate);
        }

        result.CellValue(row, ExportColumns.OfstedSchoolColumns.Grade).Should().Be(expected.Grade ?? string.Empty);

        AssertCellContainsExpectedOfstedRatingScore(result, row, ExportColumns.OfstedSchoolColumns.QualityOfEducation,
            expected.QualityOfEducation);
        AssertCellContainsExpectedOfstedRatingScore(result, row,
            ExportColumns.OfstedSchoolColumns.BehaviourAndAttitudes, expected.BehaviourAndAttitudes);
        AssertCellContainsExpectedOfstedRatingScore(result, row, ExportColumns.OfstedSchoolColumns.PersonalDevelopment,
            expected.PersonalDevelopment);
        AssertCellContainsExpectedOfstedRatingScore(result, row,
            ExportColumns.OfstedSchoolColumns.LeadershipAndManagement, expected.LeadershipAndManagement);
        AssertCellContainsExpectedOfstedRatingScore(result, row, ExportColumns.OfstedSchoolColumns.EarlyYearsProvision,
            expected.EarlyYearsProvision);
        AssertCellContainsExpectedOfstedRatingScore(result, row, ExportColumns.OfstedSchoolColumns.SixthFormProvision,
            expected.SixthFormProvision);

        AssertCellContainsExpectedSafeguardingScore(result, row,
            ExportColumns.OfstedSchoolColumns.EffectiveSafeguarding, expected.EffectiveSafeguarding);

        AssertCellContainsExpectedCategoryOfConcern(result, row, ExportColumns.OfstedSchoolColumns.CategoryOfConcern,
            expected.CategoryOfConcern);

        result.CellValue(row, ExportColumns.OfstedSchoolColumns.BeforeOrAfterJoiningTrust).Should()
            .Be(expected.BeforeOrAfterJoiningTrust.ToDisplayString());
    }

    private static void AssertCellContainsExpectedOfstedRatingScore(IXLWorksheet result, int row,
        ExportColumns.OfstedSchoolColumns column, OfstedRatingScore expected)
    {
        var expectedString = expected.ToDisplayString(false);
        result.CellValue(row, column).Should().Be(expectedString);
    }

    private static void AssertCellContainsExpectedSafeguardingScore(IXLWorksheet result, int row,
        ExportColumns.OfstedSchoolColumns column, SafeguardingScore expected)
    {
        var expectedString = expected.ToDisplayString();
        if (expectedString == "Unknown") expectedString = "Not available";
        result.CellValue(row, column).Should().Be(expectedString);
    }

    private static void AssertCellContainsExpectedCategoryOfConcern(IXLWorksheet result, int row,
        ExportColumns.OfstedSchoolColumns column, CategoriesOfConcern expected)
    {
        var expectedString = expected.ToDisplayString();
        if (expectedString == "Unknown") expectedString = "Not available";
        result.CellValue(row, column).Should().Be(expectedString);
    }
}
