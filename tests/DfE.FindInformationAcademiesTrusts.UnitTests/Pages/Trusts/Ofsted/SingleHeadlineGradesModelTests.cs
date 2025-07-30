using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public class SingleHeadlineGradesModelTests : BaseOfstedAreaModelTests<SingleHeadlineGradesModel>
{
    public SingleHeadlineGradesModelTests()
    {
        Sut = new SingleHeadlineGradesModel(MockDataSourceService,
                MockTrustService,
                MockAcademyService,
                MockOfstedTrustDataExportService,
                MockDateTimeProvider
            )
            { Uid = TrustUid };
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Single headline grades");
    }
}
