using DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Governance;

public class HistoricModelTests : BaseGovernanceAreaModelTests<HistoricModel>
{
    public HistoricModelTests()
    {
        Sut = new HistoricModel(MockSchoolService, MockTrustService, MockDataSourceService,
                MockSchoolNavMenu)
            { Urn = SchoolUrn };
    }

    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Historic governors");
    }
}
