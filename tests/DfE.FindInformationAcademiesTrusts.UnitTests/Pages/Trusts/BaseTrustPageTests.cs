using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts;

public abstract class BaseTrustPageTests<T> where T : TrustsAreaModel
{
    protected T Sut = default!;
    protected readonly ITrustService MockTrustService = Substitute.For<ITrustService>();
    protected readonly IDataSourceService MockDataSourceService = Mocks.MockDataSourceService.CreateSubstitute();

    protected readonly DataSourceServiceModel GiasDataSource =
        new(Source.Gias, new DateTime(2023, 11, 9), UpdateFrequency.Daily);

    protected readonly DataSourceServiceModel MstrDataSource =
        new(Source.Mstr, new DateTime(2023, 11, 9), UpdateFrequency.Daily);

    protected readonly DataSourceServiceModel MisDataSource =
        new(Source.Mis, new DateTime(2023, 11, 9), UpdateFrequency.Monthly);

    protected readonly DataSourceServiceModel EesDataSource = new(Source.ExploreEducationStatistics,
        new DateTime(2023, 11, 9), UpdateFrequency.Annually);

    protected readonly DataSourceServiceModel CompareDataSource =
        new(Source.CompareSchoolCollegePerformanceEnglandPopulation, new DateTime(2023, 11, 9), UpdateFrequency.Annually);

    protected const string TrustUid = "1234";
    protected const string TrustReference = "TR5678";
    protected readonly TrustSummaryServiceModel DummyTrustSummary = new(TrustUid, "My Trust", "Multi-academy trust", 3);

    private const string EmptyTrustUid = "";

    protected readonly TrustSummaryServiceModel DummyTrustSummaryEmptyUid =
        new(EmptyTrustUid, "My Trust", "Multi-academy trust", 3);

    protected BaseTrustPageTests()
    {
        MockTrustService.GetTrustSummaryAsync(TrustUid).Returns(DummyTrustSummary);
        MockTrustService.GetTrustReferenceNumberAsync(TrustUid).Returns(TrustReference);
    }

    [Fact]
    public void ShowHeaderSearch_should_be_true()
    {
        Sut.ShowHeaderSearch.Should().Be(true);
    }

    [Fact]
    public async Task OnGetAsync_should_fetch_a_trustsummary_by_uid()
    {
        Sut.Uid = DummyTrustSummary.Uid;

        await Sut.OnGetAsync();
        Sut.TrustSummary.Should().Be(DummyTrustSummary);
    }

    [Fact]
    public async Task OnGetAsync_should_return_not_found_result_if_trust_is_not_found()
    {
        MockTrustService.GetTrustSummaryAsync("1111").Returns(Task.FromResult<TrustSummaryServiceModel?>(null));

        Sut.Uid = "1111";
        var result = await Sut.OnGetAsync();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_should_return_not_found_result_if_Uid_is_not_provided()
    {
        MockTrustService.GetTrustSummaryAsync("").Returns(Task.FromResult((TrustSummaryServiceModel?)null));

        Sut.Uid = "";
        var result = await Sut.OnGetAsync();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_should_return_page_result_if_uid_exists()
    {
        var result = await Sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnGetAsync_should_configure_TrustPageMetadata_TrustName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.EntityName.Should().Be("My Trust");
    }

    [Fact]
    public abstract Task OnGetAsync_should_configure_TrustPageMetadata_PageName();

    [Fact]
    public abstract Task OnGetAsync_sets_correct_data_source_list();
}
