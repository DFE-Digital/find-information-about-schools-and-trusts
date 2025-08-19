using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Overview;

public class ReligiousCharacteristicsModelTests : BaseOverviewAreaModelTests<ReligiousCharacteristicsModel>
{
    private readonly SchoolReligiousCharacteristicsServiceModel _dummyCharacteristicsServiceModel =
        new("Diocese of Nottingham", "Roman Catholic", "Not applicable");

    public ReligiousCharacteristicsModelTests()
    {
        MockSchoolService.GetReligiousCharacteristicsAsync(Arg.Any<int>())
            .Returns(_dummyCharacteristicsServiceModel);

        Sut = new ReligiousCharacteristicsModel(MockSchoolService, MockTrustService, MockDataSourceService,
            MockSchoolNavMenu)
        {
            Urn = SchoolUrn
        };
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Religious characteristics");
    }

    [Fact]
    public async Task OnGetAsync_should_set_characteristic_strings()
    {
        await Sut.OnGetAsync();

        Sut.ReligiousCharacter.Should().Be(_dummyCharacteristicsServiceModel.ReligiousCharacter);
        Sut.ReligiousAuthority.Should().Be(_dummyCharacteristicsServiceModel.ReligiousAuthority);
        Sut.ReligiousEthos.Should().Be(_dummyCharacteristicsServiceModel.ReligiousEthos);
    }
}
