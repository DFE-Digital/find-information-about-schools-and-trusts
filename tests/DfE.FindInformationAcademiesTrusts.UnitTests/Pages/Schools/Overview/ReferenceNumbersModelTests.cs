using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Overview;

public class ReferenceNumbersModelTests : BaseOverviewAreaModelTests<ReferenceNumbersModel>
{
    private const string Laestab = "123/4567";
    private const string Ukprn = "12345678";

    public ReferenceNumbersModelTests()
    {
        MockGetReferenceNumbersMethodToReturn(Laestab, Ukprn);

        Sut = new ReferenceNumbersModel(MockSchoolService, MockTrustService, MockDataSourceService, MockSchoolNavMenu)
        {
            Urn = SchoolUrn
        };
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Reference numbers");
    }

    [Fact]
    public async Task OnGetAsync_should_set_laestab_correctly()
    {
        await Sut.OnGetAsync();

        Sut.Laestab.Should().Be(Laestab);
    }

    [Fact]
    public async Task OnGetAsync_should_handle_null_laestab()
    {
        MockGetReferenceNumbersMethodToReturn(null, Ukprn);

        await Sut.OnGetAsync();

        Sut.Laestab.Should().Be("Not available");
    }

    [Fact]
    public async Task OnGetAsync_should_set_ukprn()
    {
        await Sut.OnGetAsync();

        Sut.Ukprn.Should().Be(Ukprn);
    }

    [Fact]
    public async Task OnGetAsync_should_handle_null_ukprn()
    {
        MockGetReferenceNumbersMethodToReturn(Laestab, null);

        await Sut.OnGetAsync();

        Sut.Ukprn.Should().Be("Not available");
    }

    private void MockGetReferenceNumbersMethodToReturn(string? laestab, string? ukprn)
    {
        MockSchoolService.GetReferenceNumbersAsync(Arg.Any<int>())
            .Returns(a => new SchoolReferenceNumbersServiceModel(a.Arg<int>(), laestab, ukprn));
    }
}
