using ClosedXML.Excel;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using static DfE.FindInformationAcademiesTrusts.Services.Export.ExportColumns;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services.ExportServices
{

    public class PipelineAcademiesExportServiceTests
    {
        private readonly ITrustService _mockTrustService;
        private readonly IAcademyService _mockAcademyService;
        private readonly PipelineAcademiesExportService _sut;

        private readonly string trustUid = "1";
        private readonly string trustReferenceNumber = "TR1234";

        public PipelineAcademiesExportServiceTests()
        {
            _mockTrustService = Substitute.For<ITrustService>();
            _mockAcademyService = Substitute.For<IAcademyService>();

            var trustSummary = new TrustSummaryServiceModel("1", "TR1234","Sample Trust", "Multi-academy trust", 1);

            _mockTrustService.GetTrustSummaryAsync(trustReferenceNumber).Returns(trustSummary);

            _sut = new PipelineAcademiesExportService(_mockTrustService, _mockAcademyService);
        }

        [Fact]
        public async Task IfTrustDetailsDontExist_ShouldThrow()
        {
            string unknownTrustId = "Unknown";
            string unknownTrustReferenceNumber = "TRUnknown";
            

            var result = async () => { await _sut.BuildAsync(unknownTrustId,unknownTrustReferenceNumber); };

            await result.Should().ThrowAsync<DataIntegrityException>()
                .WithMessage($"Trust summary not found for trust reference number {unknownTrustReferenceNumber}");
        }

        [Fact]
        public async Task ExportPipelineAcademiesToSpreadsheet_ShouldCorrectlyExtractPipelineAcademyDataAsync()
        {
            _mockTrustService.GetTrustReferenceNumberAsync(trustUid).Returns(trustReferenceNumber);

            _mockAcademyService.GetAcademiesPipelinePreAdvisoryAsync(trustReferenceNumber)
                .Returns([
                    new AcademyPipelineServiceModel("1", "Academy 1", new AgeRange(4, 11), "Local Authority 1",
                        "Pre-advisory", new DateTime(2025, 2, 19))
                ]);

            _mockAcademyService.GetAcademiesPipelinePostAdvisoryAsync(trustReferenceNumber)
                .Returns([
                    new AcademyPipelineServiceModel("2", "Academy 2", new AgeRange(2, 11), "Local Authority 2",
                        "Post-advisory", new DateTime(2026, 2, 20))
                ]);

            _mockAcademyService.GetAcademiesPipelineFreeSchoolsAsync(trustReferenceNumber)
                .Returns([
                    new AcademyPipelineServiceModel("3", "Academy 3", new AgeRange(11, 18), "Local Authority 3",
                        "Free school", new DateTime(2025, 6, 21))
                ]);

            var result = await _sut.BuildAsync(trustUid,trustReferenceNumber);

            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Pipeline Academies");

            worksheet.AssertSpreadsheetMatches(1,
                ["Sample Trust"],
                ["Multi-academy trust"],
                [],
                ["Pre-advisory academies"],
                [
                    "School Name", "URN", "Age range", "Local authority", "Project type",
                    "Proposed conversion or transfer date"
                ],
                ["Academy 1", "1", "4-11", "Local Authority 1", "Pre-advisory", new DateTime(2025, 2, 19)],
                [],
                ["Post-advisory academies"],
                [
                    "School Name", "URN", "Age range", "Local authority", "Project type",
                    "Proposed conversion or transfer date"
                ],
                ["Academy 2", "2", "2-11", "Local Authority 2", "Post-advisory", new DateTime(2026, 2, 20)],
                [],
                ["Free schools"],
                ["School Name", "URN", "Age range", "Local authority", "Project type", "Provisional opening date"],
                ["Academy 3", "3", "11-18", "Local Authority 3", "Free school", new DateTime(2025, 6, 21)]
            );
        }

        [Fact]
        public async Task ExportPipelineAcademiesToSpreadsheetAsync_ShouldArrangeAcademiesByNameAlphabetically()
        {
            _mockTrustService.GetTrustReferenceNumberAsync(trustUid).Returns(trustReferenceNumber);

            _mockAcademyService.GetAcademiesPipelinePreAdvisoryAsync(trustReferenceNumber)
                .Returns(
                [
                    new AcademyPipelineServiceModel("1", "z Academy", new AgeRange(4, 10), "Local Authority 1",
                        "Pre-advisory", new DateTime(2023, 2, 1)),
                    new AcademyPipelineServiceModel("2", "B Academy", new AgeRange(5, 11), "Local Authority 2",
                        "Pre-advisory", new DateTime(2024, 3, 2)),
                    new AcademyPipelineServiceModel("3", "S Academy 1", new AgeRange(6, 12), "Local Authority 3",
                        "Pre-advisory", new DateTime(2025, 4, 3)),
                    new AcademyPipelineServiceModel("4", "S Academy 2", new AgeRange(7, 13), "Local Authority 4",
                        "Pre-advisory", new DateTime(2026, 5, 4))
                ]);

            _mockAcademyService.GetAcademiesPipelinePostAdvisoryAsync(trustReferenceNumber)
                .Returns(
                [
                    new AcademyPipelineServiceModel("5", "zz Academy", new AgeRange(4, 10), "Local Authority 1",
                        "Post-advisory", new DateTime(2023, 2, 1)),
                    new AcademyPipelineServiceModel("6", "Bb Academy 2", new AgeRange(5, 11), "Local Authority 2",
                        "Post-advisory", new DateTime(2024, 3, 2)),
                    new AcademyPipelineServiceModel("7", "Bb Academy 1", new AgeRange(6, 12), "Local Authority 3",
                        "Post-advisory", new DateTime(2025, 4, 3)),
                    new AcademyPipelineServiceModel("8", "Ss Academy", new AgeRange(7, 13), "Local Authority 4",
                        "Post-advisory", new DateTime(2026, 5, 4))
                ]);

            _mockAcademyService.GetAcademiesPipelineFreeSchoolsAsync(trustReferenceNumber)
                .Returns(
                [
                    new AcademyPipelineServiceModel("9", "Aaa Academy", new AgeRange(4, 10), "Local Authority 1",
                        "Free school", new DateTime(2023, 2, 1)),
                    new AcademyPipelineServiceModel("10", "Zzz Academy", new AgeRange(5, 11), "Local Authority 2",
                        "Free school", new DateTime(2024, 3, 2)),
                    new AcademyPipelineServiceModel("11", "Xxx Academy", new AgeRange(6, 12), "Local Authority 3",
                        "Free school", new DateTime(2025, 4, 3)),
                    new AcademyPipelineServiceModel("12", "Fff Academy", new AgeRange(7, 13), "Local Authority 4",
                        "Free school", new DateTime(2026, 5, 4))
                ]);

            var result = await _sut.BuildAsync(trustUid,trustReferenceNumber);

            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Pipeline Academies");

            worksheet.AssertSpreadsheetMatches(1,
                ["Sample Trust"],
                ["Multi-academy trust"],
                [],
                ["Pre-advisory academies"],
                [
                    "School Name", "URN", "Age range", "Local authority", "Project type",
                    "Proposed conversion or transfer date"
                ],
                ["B Academy", "2", "5-11", "Local Authority 2", "Pre-advisory", new DateTime(2024, 3, 2)],
                ["S Academy 1", "3", "6-12", "Local Authority 3", "Pre-advisory", new DateTime(2025, 4, 3)],
                ["S Academy 2", "4", "7-13", "Local Authority 4", "Pre-advisory", new DateTime(2026, 5, 4)],
                ["z Academy", "1", "4-10", "Local Authority 1", "Pre-advisory", new DateTime(2023, 2, 1)],
                [],
                ["Post-advisory academies"],
                [
                    "School Name", "URN", "Age range", "Local authority", "Project type",
                    "Proposed conversion or transfer date"
                ],
                ["Bb Academy 1", "7", "6-12", "Local Authority 3", "Post-advisory", new DateTime(2025, 4, 3)],
                ["Bb Academy 2", "6", "5-11", "Local Authority 2", "Post-advisory", new DateTime(2024, 3, 2)],
                ["Ss Academy", "8", "7-13", "Local Authority 4", "Post-advisory", new DateTime(2026, 5, 4)],
                ["zz Academy", "5", "4-10", "Local Authority 1", "Post-advisory", new DateTime(2023, 2, 1)],
                [],
                ["Free schools"],
                ["School Name", "URN", "Age range", "Local authority", "Project type", "Provisional opening date"],
                ["Aaa Academy", "9", "4-10", "Local Authority 1", "Free school", new DateTime(2023, 2, 1)],
                ["Fff Academy", "12", "7-13", "Local Authority 4", "Free school", new DateTime(2026, 5, 4)],
                ["Xxx Academy", "11", "6-12", "Local Authority 3", "Free school", new DateTime(2025, 4, 3)],
                ["Zzz Academy", "10", "5-11", "Local Authority 2", "Free school", new DateTime(2024, 3, 2)]
            );
        }

        [Fact]
        public async Task Export_ShouldCorrectlyHandleNullValuesAsync()
        {
            _mockTrustService.GetTrustReferenceNumberAsync(trustUid).Returns(trustReferenceNumber);

            _mockAcademyService.GetAcademiesPipelinePreAdvisoryAsync(trustReferenceNumber)
                .Returns(
                [
                    new AcademyPipelineServiceModel(null, null, null, null,
                        null, null)
                ]);


            var result = await _sut.BuildAsync(trustUid,trustReferenceNumber);

            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Pipeline Academies");

            worksheet.CellValue(6, PipelineAcademiesColumns.SchoolName).Should().Be(string.Empty);
            worksheet.CellValue(6, PipelineAcademiesColumns.Urn).Should().Be(string.Empty);
            worksheet.CellValue(6, PipelineAcademiesColumns.AgeRange).Should().Be("Unconfirmed");
            worksheet.CellValue(6, PipelineAcademiesColumns.LocalAuthority).Should().Be(string.Empty);
            worksheet.CellValue(6, PipelineAcademiesColumns.ProjectType).Should().Be(string.Empty);
            worksheet.CellValue(6, PipelineAcademiesColumns.ChangeDate).Should().Be("Unconfirmed");
        }
    }
}
