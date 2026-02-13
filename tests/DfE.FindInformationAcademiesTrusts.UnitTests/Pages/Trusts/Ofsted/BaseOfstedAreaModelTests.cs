using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public abstract class BaseOfstedAreaModelTests<T> : BaseTrustPageTests<T>, ITestSubpages, ITestExport
    where T : OfstedAreaModel
{

    protected readonly IOfstedTrustDataExportService
        MockOfstedTrustDataExportService = Substitute.For<IOfstedTrustDataExportService>();

    protected readonly IDateTimeProvider MockDateTimeProvider = Substitute.For<IDateTimeProvider>();

    protected readonly IOfstedService MockOfstedService = Substitute.For<IOfstedService>();

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await Sut.OnGetAsync();

        await MockDataSourceService.Received(1).GetAsync(Source.Gias);
        await MockDataSourceService.Received(1).GetAsync(Source.Mis);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Overview", [
                    new DataSourceListEntry(GiasDataSource, "Date joined trust"),
                    new DataSourceListEntry(MisDataSource, "All inspection types"),
                    new DataSourceListEntry(MisDataSource, "All inspection dates")
                ]
            ),
            new DataSourcePageListEntry("Report cards", [
                    new DataSourceListEntry(MisDataSource, "Current report card ratings"),
                    new DataSourceListEntry(MisDataSource, "Previous report card ratings")
                ]
            ),
            new DataSourcePageListEntry("Older inspections (before November 2025)", [
                    new DataSourceListEntry(MisDataSource, "Inspection ratings after September 24"),
                    new DataSourceListEntry(MisDataSource, "Inspection ratings before September 24")
                ]
            )
        ]);
    }

    [Fact]
    public async Task OnGetExportAsync_ShouldReturnFileResult_WhenUidIsValid()
    {
        // Arrange
        byte[] expectedBytes = [1, 2, 3];
        MockOfstedTrustDataExportService.BuildAsync(TrustUid).Returns(expectedBytes);

        // Act
        var result = await Sut.OnGetExportAsync(TrustUid);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult?.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        fileResult?.FileContents.Should().BeEquivalentTo(expectedBytes);
    }

    [Fact]
    public async Task OnGetExportAsync_ShouldReturnNotFound_WhenUidIsInvalid()
    {
        // Arrange
        var uid = "invalid-uid";

        MockTrustService.GetTrustSummaryAsync(uid).ReturnsNull();

        // Act
        var result = await Sut.OnGetExportAsync(uid);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetExportAsync_ShouldSanitizeTrustName_WhenTrustNameContainsIllegalCharacters()
    {
        // Arrange
        var uid = TrustUid;
        var expectedBytes = new byte[] { 1, 2, 3 };

        MockTrustService.GetTrustSummaryAsync(uid)!.Returns(DummyTrustSummary with { Name = "Sample/Trust:Name?" });
        MockOfstedTrustDataExportService.BuildAsync(uid).Returns(expectedBytes);

        // Act
        var result = await Sut.OnGetExportAsync(uid);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult?.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        fileResult?.FileContents.Should().BeEquivalentTo(expectedBytes);
        fileResult?.FileDownloadName.Should().NotBeEmpty();

        // Verify that the file name is sanitized (no illegal characters)
        var fileDownloadName = fileResult?.FileDownloadName ?? string.Empty;
        var invalidFileNameChars = Path.GetInvalidFileNameChars();

        // Check that the file name doesn't contain any invalid characters
        var containsInvalidChars = fileDownloadName.Any(c => invalidFileNameChars.Contains(c));
        containsInvalidChars.Should().BeFalse("the file name should not contain any illegal characters");
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_PageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Ofsted");
    }

    [Fact]
    public abstract Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName();
}
