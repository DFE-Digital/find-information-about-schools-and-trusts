using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Academies.InTrust;

public abstract class AcademiesInTrustAreaModelTests<T> : BaseAcademiesAreaModelTests<T>, ITestTabPages
    where T : AcademiesInTrustAreaModel
{
    protected readonly IDateTimeProvider MockDateTimeProvider = Substitute.For<IDateTimeProvider>();
    protected readonly IAcademiesExportService MockAcademiesExportService = Substitute.For<IAcademiesExportService>();

    [Fact]
    public override async Task OnGetExportAsync_ShouldReturnFileResult_WhenUidIsValid()
    {
        // Arrange
        byte[] expectedBytes = [1, 2, 3];

        MockAcademiesExportService.BuildAsync(TrustUid).Returns(expectedBytes);

        // Act
        var result = await Sut.OnGetExportAsync(TrustUid);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult?.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        fileResult?.FileContents.Should().BeEquivalentTo(expectedBytes);
    }

    [Fact]
    public override async Task OnGetExportAsync_ShouldReturnNotFound_WhenUidIsInvalid()
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
    public override async Task OnGetExportAsync_ShouldSanitizeTrustName_WhenTrustNameContainsIllegalCharacters()
    {
        // Arrange
        var uid = TrustUid;
        var expectedBytes = new byte[] { 1, 2, 3 };

        MockTrustService.GetTrustSummaryAsync(uid).Returns(DummyTrustSummary with { Name = "Sample/Trust:Name?" });
        MockAcademiesExportService.BuildAsync(uid).Returns(expectedBytes);

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
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await Sut.OnGetAsync();

        await MockDataSourceService.Received(1).GetAsync(Source.Gias);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Details",
                [new DataSourceListEntry(GiasDataSource)]),
            new DataSourcePageListEntry("Pupil numbers",
            [
                new DataSourceListEntry(CompareDataSource, "Pupil numbers"),
                new DataSourceListEntry(GiasDataSource, "All other information was")
            ]),
            new DataSourcePageListEntry("Free school meals",
            [
                new DataSourceListEntry(GiasDataSource, "Pupils eligible for free school meals"),
                new DataSourceListEntry(EesDataSource, "Local authority average 2023/24"),
                new DataSourceListEntry(EesDataSource, "National average 2023/24")
            ])
        ]);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("In this trust");
    }

    [Fact]
    public override async Task OnGetAsync_should_populate_TabList_to_tabs()
    {
        _ = await Sut.OnGetAsync();

        Sut.TabList.Should()
            .SatisfyRespectively(
                l =>
                {
                    l.LinkDisplayText.Should().Be("Details");
                    l.AspPage.Should().Be("./Details");
                    l.TestId.Should().Be("in-this-trust-details-tab");
                },
                l =>
                {
                    l.LinkDisplayText.Should().Be("Pupil numbers");
                    l.AspPage.Should().Be("./PupilNumbers");
                    l.TestId.Should().Be("in-this-trust-pupil-numbers-tab");
                },
                l =>
                {
                    l.LinkDisplayText.Should().Be("Free school meals");
                    l.AspPage.Should().Be("./FreeSchoolMeals");
                    l.TestId.Should().Be("in-this-trust-free-school-meals-tab");
                });
    }
}
