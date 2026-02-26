using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted;

public abstract class BaseOfstedAreaModelTests<T> : BaseSchoolPageTests<T> where T : OfstedAreaModel
{
    protected readonly ISchoolOverviewDetailsService MockSchoolOverviewDetailsService =
        Substitute.For<ISchoolOverviewDetailsService>();

    protected readonly IOtherServicesLinkBuilder MockOtherServicesLinkBuilder =
        Substitute.For<IOtherServicesLinkBuilder>();

    protected readonly IOfstedService MockOfstedService = Substitute.For<IOfstedService>();

    protected readonly IPowerBiLinkBuilderService MockPowerBiLinkBuilderService = Substitute.For<IPowerBiLinkBuilderService>();

    protected readonly SchoolOverviewServiceModel DummySchoolDetails =
        new("Cool school", "some street, in a town", "Yorkshire", "Leeds", "Secondary", new AgeRange(11, 18),
            NurseryProvision.NotRecorded);

    protected BaseOfstedAreaModelTests()
    {
        MockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(Arg.Any<int>(), Arg.Any<SchoolCategory>())
            .Returns(DummySchoolDetails);

        MockOtherServicesLinkBuilder.OfstedReportLinkForSchool(Arg.Any<int>())
            .Returns(call => $"https://reports.ofsted.gov.uk/test/{call.ArgAt<int>(0)}");

        MockPowerBiLinkBuilderService.BuildReportCardsLink(Arg.Any<int>())
            .Returns("https://powerbi.com/reportcards");

        MockPowerBiLinkBuilderService.BuildOfstedPublishedLink(Arg.Any<int>())
            .Returns("https://powerbi.com/ofstedpublished");

    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Ofsted");
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await VerifyCorrectDataSources(SchoolUrn);

        MockDataSourceService.ClearReceivedCalls();

        await VerifyCorrectDataSources(AcademyUrn);
    }

    [Fact]
    public async Task OnGetAsync_sets_correct_DateJoinedTrust_for_academy()
    {
        DummySchoolDetails.DateJoinedTrust = DateOnly.Parse("2011-04-03");
        Sut.Urn = AcademyUrn;

        _ = await Sut.OnGetAsync();

        Sut.DateJoinedTrust.Should().NotBeNull();
        Sut.DateJoinedTrust.Should().Be(DateTime.Parse("2011-04-03"));
    }

    [Fact]
    public async Task OnGetAsync_does_not_set_DateJoinedTrust_for_school()
    {
        _ = await Sut.OnGetAsync();

        Sut.DateJoinedTrust.Should().BeNull();
    }

    [Fact]
    public async Task OnGetASync_sets_correct_OfstedReportUrl_for_school()
    {
        Sut.Urn = SchoolUrn;

        _ = await Sut.OnGetAsync();

        Sut.OfstedReportUrl.Should().Be($"https://reports.ofsted.gov.uk/test/{SchoolUrn}");
    }

    [Fact]
    public async Task OnGetASync_sets_correct_OfstedReportUrl_for_academy()
    {
        Sut.Urn = AcademyUrn;

        _ = await Sut.OnGetAsync();

        Sut.OfstedReportUrl.Should().Be($"https://reports.ofsted.gov.uk/test/{AcademyUrn}");
    }


    [Fact]
    public abstract Task OnGetAsync_should_call_populate_tablist();

    private async Task VerifyCorrectDataSources(int urn)
    {
        Sut.Urn = urn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);
        await MockDataSourceService.Received(1).GetAsync(Source.Mis);


        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Overview", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Date joined trust"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "All inspection types"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "All inspection dates")
            ]),
            new DataSourcePageListEntry("Report cards", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "Current report card ratings"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "Previous report card ratings")
            ]),
            new DataSourcePageListEntry("Older inspections (before November 2025)", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "Inspection ratings after September 24"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "Inspection ratings before September 24")
            ])
        ]);
    }

    [Fact]
    public abstract Task OnGetAsync_ShouldSetPowerBiLinkUrl();
}
