using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools;

public abstract class BaseSchoolPageTests<T> where T : SchoolAreaModel
{
    protected T Sut = null!;
    protected readonly ISchoolService MockSchoolService = Substitute.For<ISchoolService>();
    protected readonly ITrustService MockTrustService = Substitute.For<ITrustService>();
    protected readonly IDataSourceService MockDataSourceService = Mocks.MockDataSourceService.CreateSubstitute();
    protected readonly ISchoolNavMenu MockSchoolNavMenu = Substitute.For<ISchoolNavMenu>();

    protected const int SchoolUrn = 123456;
    protected const int AcademyUrn = 888888;

    protected readonly SchoolSummaryServiceModel DummySchoolSummary =
        new(SchoolUrn, "Cool school", "Community school", SchoolCategory.LaMaintainedSchool);

    protected readonly SchoolSummaryServiceModel DummyAcademySummary =
        new(AcademyUrn, "Cool academy", "Academy school", SchoolCategory.Academy);

    protected BaseSchoolPageTests()
    {
        MockSchoolService.GetSchoolSummaryAsync(SchoolUrn).Returns(DummySchoolSummary);
        MockSchoolService.GetSchoolSummaryAsync(AcademyUrn).Returns(DummyAcademySummary);

        MockSchoolService.IsPartOfFederationAsync(SchoolUrn).Returns(true);
        MockSchoolService.IsPartOfFederationAsync(AcademyUrn).Returns(false);
    }

    [Fact]
    public abstract Task OnGetAsync_should_configure_PageMetadata_PageName();

    [Fact]
    public abstract Task OnGetAsync_should_configure_PageMetadata_SubPageName();

    [Fact]
    public abstract Task OnGetAsync_sets_correct_data_source_list();

    [Fact]
    public void ShowHeaderSearch_should_be_true()
    {
        Sut.ShowHeaderSearch.Should().Be(true);
    }

    [Fact]
    public async Task OnGetAsync_should_fetch_a_SchoolSummary_by_urn()
    {
        Sut.Urn = DummySchoolSummary.Urn;

        await Sut.OnGetAsync();
        Sut.SchoolSummary.Should().Be(DummySchoolSummary);
    }

    [Fact]
    public async Task OnGetAsync_should_return_not_found_result_if_school_is_not_found()
    {
        MockSchoolService.GetSchoolSummaryAsync(111111).Returns((SchoolSummaryServiceModel?)null);

        Sut.Urn = 111111;
        var result = await Sut.OnGetAsync();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_should_return_not_found_result_if_urn_is_not_provided()
    {
        MockSchoolService.GetSchoolSummaryAsync(0).Returns((SchoolSummaryServiceModel?)null);

        Sut.Urn = 0;
        var result = await Sut.OnGetAsync();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_should_return_page_result_if_urn_exists()
    {
        Sut.Urn = DummySchoolSummary.Urn;

        var result = await Sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnGetAsync_if_school_is_an_academy_should_get_trust_summary()
    {
        MockTrustService.GetTrustSummaryAsync(DummyAcademySummary.Urn)
            .Returns(new TrustSummaryServiceModel("TRUST", "Cool trust", "Multi-academy trust", 1));

        Sut.Urn = DummyAcademySummary.Urn;

        await Sut.OnGetAsync();

        await MockTrustService.Received(1).GetTrustSummaryAsync(DummyAcademySummary.Urn);

        Sut.TrustSummary.Should().NotBeNull();
    }

    [Fact]
    public async Task
        OnGetAsync_if_school_is_not_an_academy_should_get_trust_summary_as_la_maintained_could_be_a_trust()
    {
        Sut.Urn = DummySchoolSummary.Urn;

        await Sut.OnGetAsync();

        await MockTrustService.Received(1).GetTrustSummaryAsync(Sut.Urn);

        Sut.TrustSummary.Should().BeNull();
    }
}
