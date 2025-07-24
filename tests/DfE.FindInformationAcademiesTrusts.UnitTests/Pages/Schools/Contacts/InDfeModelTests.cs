using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Contacts;

public class InDfeModelTests : BaseContactsAreaModelTests<InDfeModel>
{
    private readonly ISchoolContactsService _mockSchoolContactsService = Substitute.For<ISchoolContactsService>();

    private readonly SchoolInternalContactsServiceModel _dummySchoolContactsServiceModel =
        new(new Person("Aaron Aaronson", "aa@education.gov.uk"),
            new Person("Bertha Billingsley", "bb@education.gov.uk"),
            new Person("Carlton Coriander", "cc@education.gov.uk"));

    public InDfeModelTests()
    {
        _mockSchoolContactsService.GetInternalContactsAsync(Arg.Any<int>()).Returns(_dummySchoolContactsServiceModel);

        Sut = new InDfeModel(MockSchoolService, MockTrustService, _mockSchoolContactsService, MockDataSourceService,
                MockSchoolNavMenu, MockFeatureManager)
        { Urn = SchoolUrn };
    }

    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await OnGetAsync_should_configure_PageMetadata_SubPageName_for_school();
        await OnGetAsync_should_configure_PageMetadata_SubPageName_for_academy();
    }

    private async Task OnGetAsync_should_configure_PageMetadata_SubPageName_for_school()
    {
        Sut.Urn = SchoolUrn;

        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("In DfE");
    }

    private async Task OnGetAsync_should_configure_PageMetadata_SubPageName_for_academy()
    {
        Sut.Urn = AcademyUrn;

        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("In DfE");
    }

    [Fact]
    public void SubPageNavMenuName_should_be_InDfE()
    {
        InDfeModel.SubPageName.Should().Be("In DfE");
    }

    [Fact]
    public async Task OnGetAsync_should_set_RegionsGroupLocalAuthorityLead_contact_details()
    {
        await Sut.OnGetAsync();

        Sut.RegionsGroupLocalAuthorityLead.Should().Be(_dummySchoolContactsServiceModel.RegionsGroupLocalAuthorityLead);
    }

    [Fact]
    public async Task OnGetAsync_should_set_TrustRelationshipManager_contact_details()
    {
        await Sut.OnGetAsync();

        Sut.TrustRelationshipManager.Should().Be(_dummySchoolContactsServiceModel.TrustRelationshipManager);
    }

    [Fact]
    public async Task OnGetAsync_should_set_SfsoLead_contact_details()
    {
        await Sut.OnGetAsync();

        Sut.SfsoLead.Should().Be(_dummySchoolContactsServiceModel.SfsoLead);
    }
}
