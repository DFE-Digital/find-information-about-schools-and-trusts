using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Contacts;

public class InSchoolModelTests : BaseSchoolPageTests<InSchoolModel>
{
    private readonly IMediator _mockMediator = Substitute.For<IMediator>();
    private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();
    private readonly IVariantFeatureManager _mockFeatureManager = Substitute.For<IVariantFeatureManager>();

    private readonly SchoolContact _dummyHeadTeacher = new("Aaron Aaronson", "aa@someschool.com");

    public InSchoolModelTests()
    {
        _mockSchoolRepository.GetSchoolContactsAsync(Arg.Any<int>()).Returns(_dummyHeadTeacher);
        _mockMediator.Send(Arg.Any<GetSchoolDfeContactsQuery>()).Returns(new SchoolDfeContacts(null, null, null));

        Sut = new InSchoolModel(_mockMediator, _mockSchoolRepository, MockDataSourceService, _mockFeatureManager,
                MockSchoolService, MockTrustService, MockSchoolNavMenu)
            { Urn = SchoolUrn };
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Contacts");
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await Sut.OnGetAsync();

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In this school",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Head teacher name")])
        ]);
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

        Sut.PageMetadata.SubPageName.Should().Be("Contacts in this school");
    }

    private async Task OnGetAsync_should_configure_PageMetadata_SubPageName_for_academy()
    {
        Sut.Urn = AcademyUrn;

        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Contacts in this academy");
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "Contacts in this school")]
    [InlineData(SchoolCategory.Academy, "Contacts in this academy")]
    public void FullSubPageName_should_include_school_type(SchoolCategory schoolCategory, string expectedSubPageName)
    {
        InSchoolModel.FullSubPageName(schoolCategory).Should().Be(expectedSubPageName);
    }

    [Fact]
    public void FullSubPageName_should_throw_for_unrecognised_school_type()
    {
        var act = () => InSchoolModel.FullSubPageName((SchoolCategory)999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "In this school")]
    [InlineData(SchoolCategory.Academy, "In this academy")]
    public void SubPageNavMenuName_should_include_school_type(SchoolCategory schoolCategory, string expectedSubPageName)
    {
        InSchoolModel.SubPageName(schoolCategory).Should().Be(expectedSubPageName);
    }

    [Fact]
    public void SubPageNavMenuName_should_throw_for_unrecognised_school_type()
    {
        var act = () => InSchoolModel.SubPageName((SchoolCategory)999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task OnGetAsync_should_set_Headteacher_contact_details()
    {
        await Sut.OnGetAsync();

        Sut.HeadTeacher.Should().Be(_dummyHeadTeacher);
    }

    [Fact]
    public async Task OnGetAsync_IfContactsIsNull_ShouldReturnNotFound()
    {
        _mockSchoolRepository.GetSchoolContactsAsync(Arg.Any<int>()).ReturnsNull();

        var response = await Sut.OnGetAsync();
        response.Should().BeOfType<NotFoundResult>();
    }
}
