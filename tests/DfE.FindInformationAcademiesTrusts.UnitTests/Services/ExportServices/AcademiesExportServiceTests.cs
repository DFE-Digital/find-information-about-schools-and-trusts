using ClosedXML.Excel;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using static DfE.FindInformationAcademiesTrusts.Services.Export.ExportColumns;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services.ExportServices
{
    public class AcademiesExportServiceTests
    {
        private readonly ITrustService _mockTrustService;
        private readonly IAcademyService _mockAcademyService;
        private readonly AcademiesExportService _sut;

        private readonly string trustUid = "1";

        public AcademiesExportServiceTests()
        {
            _mockTrustService = Substitute.For<ITrustService>();
            _mockAcademyService = Substitute.For<IAcademyService>();

            var trustSummary = new TrustSummaryServiceModel("1", "Sample Trust", "Multi-academy trust", 1);

            _mockTrustService.GetTrustSummaryAsync(trustUid).Returns(trustSummary);

            _sut = new AcademiesExportService(_mockTrustService, _mockAcademyService);
        }

        [Fact]
        public async Task ExportAcademiesToSpreadsheet_ShouldGenerateCorrectHeadersAsync()
        {
            var result = await _sut.BuildAsync(trustUid);
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Academies");

            worksheet.AssertSpreadsheetMatches(3,
            [
                "School Name", "URN", "Local authority", "Type", "Rural or Urban", "Date joined",
                "Current Ofsted Rating",
                "Before/After Joining", "Date of Current Ofsted", "Previous Ofsted Rating", "Before/After Joining",
                "Date of Previous Ofsted", "Phase of Education",
                "Age range", "Pupil Numbers",
                "Capacity", "% Full", "Pupils eligible for Free School Meals",
                "LA average pupils eligible for Free School Meals",
                "National average pupils eligible for Free School Meals"
            ]);
        }

        [Fact]
        public async Task IfTrustDetailsDontExist_ShouldThrow()
        {
            string unknownTrustId = "Unknown";

            var result = async () => { await _sut.BuildAsync(unknownTrustId); };

            await result.Should().ThrowAsync<DataIntegrityException>()
                .WithMessage($"Trust summary not found for UID {unknownTrustId}");
        }

        [Fact]
        public async Task ExportAcademiesToSpreadsheet_ShouldCorrectlyExtractAcademyDataAsync()
        {
            var now = DateTime.Now;
            _mockAcademyService.GetAcademiesInTrustDetailsAsync(trustUid).Returns([
                new("123456", "Academy 1", "Local Authority 1", "Type A", "Urban", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)))
            ]);
            _mockAcademyService.GetAcademiesInTrustOfstedAsync(trustUid).Returns([
                new("123456", "Academy 1", now, new OfstedShortInspection(now, "School remains Good"), new OfstedRating(1, now.AddDays(-1)), new OfstedRating(1, now))
            ]);
            _mockAcademyService.GetAcademiesInTrustPupilNumbersAsync(trustUid).Returns([
                new("123456", "Academy 1", "Primary", new AgeRange(5, 11), 500, 600)
            ]);
            _mockAcademyService.GetAcademiesInTrustFreeSchoolMealsAsync(trustUid)
                .Returns([new("123456", "Academy 1", 20, 25, 15)]);

            var result = await _sut.BuildAsync(trustUid);
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Academies");

            worksheet.CellValue(4, AcademyColumns.SchoolName).Should().Be("Academy 1");
            worksheet.CellValue(4, AcademyColumns.Urn).Should().Be("123456");
            worksheet.CellValue(4, AcademyColumns.LocalAuthority).Should().Be("Local Authority 1");
            worksheet.CellValue(4, AcademyColumns.Type).Should().Be("Type A");
            worksheet.CellValue(4, AcademyColumns.RuralOrUrban).Should().Be("Urban");

            // Check date joined is set as a date
            worksheet.Cell(4, AcademyColumns.DateJoined).DataType.Should().Be(XLDataType.DateTime);
            worksheet.Cell(4, AcademyColumns.DateJoined).GetValue<DateTime>().Should()
                .BeCloseTo(now, TimeSpan.FromSeconds(1));

            worksheet.CellValue(4, AcademyColumns.CurrentOfstedRating).Should().Be("Outstanding");
            worksheet.CellValue(4, AcademyColumns.CurrentBeforeAfterJoining).Should().Be("After joining");
            // Current Ofsted date also as a date
            worksheet.Cell(4, AcademyColumns.DateOfCurrentInspection).DataType.Should().Be(XLDataType.DateTime);
            worksheet.Cell(4, AcademyColumns.DateOfCurrentInspection).GetValue<DateTime>().Should()
                .BeCloseTo(now, TimeSpan.FromSeconds(1));


            worksheet.CellValue(4, AcademyColumns.PreviousOfstedRating).Should().Be("Outstanding");
            worksheet.CellValue(4, AcademyColumns.PreviousBeforeAfterJoining).Should().Be("Before joining");
            worksheet.Cell(4, AcademyColumns.DateOfPreviousInspection).DataType.Should().Be(XLDataType.DateTime);
            worksheet.Cell(4, AcademyColumns.DateOfPreviousInspection).GetValue<DateTime>().Should()
                .BeCloseTo(now.AddDays(-1), TimeSpan.FromSeconds(1));

            worksheet.CellValue(4, AcademyColumns.PhaseOfEducation).Should().Be("Primary");
            worksheet.CellValue(4, AcademyColumns.AgeRange).Should().Be("5-11");
            worksheet.CellValue(4, AcademyColumns.PupilNumbers).Should().Be("500");
            worksheet.CellValue(4, AcademyColumns.Capacity).Should().Be("600");
            worksheet.CellValue(4, AcademyColumns.PercentFull).Should().Be("83%");
            worksheet.CellValue(4, AcademyColumns.PupilsEligibleFreeSchoolMeals).Should().Be("20%");
            worksheet.CellValue(4, AcademyColumns.LaPupilsEligibleFreeSchoolMeals).Should().Be("25%");
            worksheet.CellValue(4, AcademyColumns.NationalPupilsEligibleFreeSchoolMeals).Should().Be("15%");


            var expectedCurrentRow = _sut.TrustRows + _sut.HeaderRows + 1;

            _sut.CurrentRow.Should().Be(expectedCurrentRow);
        }

        [Fact]
        public async Task ExportAcademiesToSpreadsheet_ShouldHandleEmptyAcademiesAsync()
        {
            var result = await _sut.BuildAsync(trustUid);
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Academies");

            var lastUsedRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
            lastUsedRow.Should()
                .Be(3); // If no data rows are present, we expect the last used row to be the headers row (3)
        }

        [Fact]
        public async Task ExportAcademiesToSpreadsheet_ShouldCorrectlyHandleNullValuesAsync()
        {
            _mockAcademyService.GetAcademiesInTrustDetailsAsync(trustUid)
                .Returns([new("123456", null, null, null, null, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)))]);

            var now = DateTime.Now;
            _mockAcademyService.GetAcademiesInTrustOfstedAsync(trustUid).Returns([
                new("123456", null, now, new OfstedShortInspection(now, "School remains Good"), new OfstedRating(-1, null), new OfstedRating(-1, null))
            ]);
            _mockAcademyService.GetAcademiesInTrustPupilNumbersAsync(trustUid)
                .Returns([new("123456", null, null, new AgeRange(5, 11), null, null)]);
            _mockAcademyService.GetAcademiesInTrustFreeSchoolMealsAsync(trustUid)
                .Returns([new("123456", null, null, 0, 0)]);

            var result = await _sut.BuildAsync(trustUid);
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Academies");

            worksheet.CellValue(4, AcademyColumns.SchoolName).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.Urn).Should().Be("123456");
            worksheet.CellValue(4, AcademyColumns.LocalAuthority).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.Type).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.RuralOrUrban).Should().Be(string.Empty);

            worksheet.Cell(4, AcademyColumns.DateJoined).GetValue<DateTime>().Should()
                .BeCloseTo(now, TimeSpan.FromSeconds(1));

            worksheet.CellValue(4, AcademyColumns.CurrentOfstedRating).Should().Be("Not yet inspected");
            worksheet.CellValue(4, AcademyColumns.CurrentBeforeAfterJoining).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.DateOfCurrentInspection).Should().Be(string.Empty);

            worksheet.CellValue(4, AcademyColumns.PreviousOfstedRating).Should().Be("Not inspected");
            worksheet.CellValue(4, AcademyColumns.PreviousBeforeAfterJoining).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.DateOfPreviousInspection).Should().Be(string.Empty);

            worksheet.CellValue(4, AcademyColumns.PhaseOfEducation).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.AgeRange).Should().Be("5-11");
            worksheet.CellValue(4, AcademyColumns.PupilNumbers).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.Capacity).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.PercentFull).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.PupilsEligibleFreeSchoolMeals).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.LaPupilsEligibleFreeSchoolMeals).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.NationalPupilsEligibleFreeSchoolMeals).Should().Be(string.Empty);
        }

        [Fact]
        public async Task ExportAcademiesToSpreadsheet_ShouldHandleNullOfstedData()
        {
            _mockAcademyService.GetAcademiesInTrustDetailsAsync(trustUid).Returns([
                new("123456", "Academy 1", "Local Authority 1", "Type A", "Urban", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)))
            ]);
            _mockAcademyService.GetAcademiesInTrustOfstedAsync(trustUid).Returns([]);

            _mockAcademyService.GetAcademiesInTrustPupilNumbersAsync(trustUid).Returns([
                new("123456", "Academy 1", "Primary", new AgeRange(5, 11), 500, 600)
            ]);
            _mockAcademyService.GetAcademiesInTrustFreeSchoolMealsAsync(trustUid)
                .Returns([new("123456", "Academy 1", 20, 25, 15)]);

            var result = await _sut.BuildAsync(trustUid);
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Academies");

            // Ofsted null data
            worksheet.CellValue(4, AcademyColumns.DateJoined).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.CurrentOfstedRating).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.CurrentBeforeAfterJoining).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.DateOfCurrentInspection).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.PreviousOfstedRating).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.PreviousBeforeAfterJoining).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.DateOfPreviousInspection).Should().Be(string.Empty);
        }

        [Fact]
        public async Task ExportAcademiesToSpreadsheet_ShouldHandleNullPupilData()
        {
            var now = DateTime.Now;
            _mockAcademyService.GetAcademiesInTrustDetailsAsync(trustUid).Returns([
                new("123456", "Academy 1", "Local Authority 1", "Type A", "Urban", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)))
            ]);
            _mockAcademyService.GetAcademiesInTrustOfstedAsync(trustUid).Returns([
                new("123456", "Academy 1", now, new OfstedShortInspection(now, "School remains Good"), new OfstedRating(-1, null), new OfstedRating(1, now))
            ]);

            _mockAcademyService.GetAcademiesInTrustPupilNumbersAsync(trustUid).Returns([]);

            _mockAcademyService.GetAcademiesInTrustFreeSchoolMealsAsync(trustUid)
                .Returns([new("123456", "Academy 1", 20, 25, 15)]);

            var result = await _sut.BuildAsync(trustUid);
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Academies");

            worksheet.CellValue(4, AcademyColumns.SchoolName).Should().Be("Academy 1");
            worksheet.CellValue(4, AcademyColumns.Urn).Should().Be("123456");
            worksheet.CellValue(4, AcademyColumns.LocalAuthority).Should().Be("Local Authority 1");
            worksheet.CellValue(4, AcademyColumns.Type).Should().Be("Type A");
            worksheet.CellValue(4, AcademyColumns.RuralOrUrban).Should().Be("Urban");

            // NULL pupil data
            worksheet.CellValue(4, AcademyColumns.PhaseOfEducation).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.AgeRange).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.PupilNumbers).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.Capacity).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.PercentFull).Should().Be(string.Empty);
        }

        [Fact]
        public async Task ExportAcademiesToSpreadsheet_ShouldHandleNullFreeSchoolsData()
        {
            var now = DateTime.Now;
            _mockAcademyService.GetAcademiesInTrustDetailsAsync(trustUid).Returns([
                new("123456", "Academy 1", "Local Authority 1", "Type A", "Urban", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)))
            ]);

            _mockAcademyService.GetAcademiesInTrustOfstedAsync(trustUid).Returns([
                new("123456", "Academy 1", now, new OfstedShortInspection(now, "School remains Good"), new OfstedRating(-1, null), new OfstedRating(1, now))
            ]);

            _mockAcademyService.GetAcademiesInTrustPupilNumbersAsync(trustUid).Returns([
                new("123456", "Academy 1", "Primary", new AgeRange(5, 11), 500, 600)
            ]);

            _mockAcademyService.GetAcademiesInTrustFreeSchoolMealsAsync(trustUid).Returns([]);

            var result = await _sut.BuildAsync(trustUid);
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Academies");

            worksheet.CellValue(4, AcademyColumns.SchoolName).Should().Be("Academy 1");
            worksheet.CellValue(4, AcademyColumns.Urn).Should().Be("123456");
            worksheet.CellValue(4, AcademyColumns.LocalAuthority).Should().Be("Local Authority 1");
            worksheet.CellValue(4, AcademyColumns.Type).Should().Be("Type A");
            worksheet.CellValue(4, AcademyColumns.RuralOrUrban).Should().Be("Urban");

            // NULL free schools data

            worksheet.CellValue(4, AcademyColumns.PupilsEligibleFreeSchoolMeals).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.LaPupilsEligibleFreeSchoolMeals).Should().Be(string.Empty);
            worksheet.CellValue(4, AcademyColumns.NationalPupilsEligibleFreeSchoolMeals).Should().Be(string.Empty);
        }
    }
}
