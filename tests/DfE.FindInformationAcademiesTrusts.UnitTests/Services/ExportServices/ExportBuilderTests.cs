using ClosedXML.Excel;
using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using static DfE.FindInformationAcademiesTrusts.Services.Export.ExportColumns;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services.ExportServices
{

    public class ExportBuilderTests
    {
        private readonly ITrustService _mockTrustService;
        private readonly IAcademyService _mockAcademyService;

        private readonly ExportBuilder _sut;

        public ExportBuilderTests()
        {
            _mockTrustService = Substitute.For<ITrustService>();
            _mockAcademyService = Substitute.For<IAcademyService>();

            _sut = new AcademiesExportService(_mockTrustService, _mockAcademyService);

            _sut.Worksheet = Substitute.For<IXLWorksheet>();
        }

        [Fact]
        public void OnBuild_ShouldAdjustContents()
        {
            _sut.Build();

            _sut.Worksheet.Columns().Received().AdjustToContents();
        }

        [Fact]
        public void WhenWritingTrustInformation_ShouldSetFontStyleToBold()
        {
            _sut.WriteTrustInformation(new TrustSummaryServiceModel("123", "test trust", "something", 2));

            _sut.Worksheet.Row(1).Style.Font.Bold.Should().BeTrue();
        }

        [Fact]
        public void WhenWritingHeaders_ShouldSetFontStyleToBold()
        {
            _sut.WriteHeaders([]);

            _sut.Worksheet.Row(0).Style.Font.Bold.Should().BeTrue();
        }
        
        [Fact]
        public void WhenWritingDateCell_ShouldSetCorrectDateFormat()
        {
            var dateValue = DateTime.Now;
            var column = AcademyColumns.DateOfCurrentInspection;

            _sut.SetDateCell(column, dateValue);

            var cell = _sut.Worksheet.Cell(0, (int)column);
            cell.Style.NumberFormat.Received().SetFormat(StringFormatConstants.DisplayDateFormat);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData(true, "Yes")]
        [InlineData(false, "No")]
        public void SetBoolCell_should_set_correct_data(bool? value, string expected)
        {
            _sut.SetBoolCell((AcademyColumns)1, value);

            _sut.Worksheet.CellValue(0, 1).Should().Be(expected);
        }
    }
}
