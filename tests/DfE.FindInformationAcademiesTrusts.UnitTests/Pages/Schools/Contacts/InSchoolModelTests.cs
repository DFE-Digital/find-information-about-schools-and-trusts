using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
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

    private readonly SchoolContact _headTeacher = new("Head Teacher", "head@school.org.uk");

    private static DataSourceServiceModel EmptyContactDataSource => new(Source.FiatDb, null, null);

    public InSchoolModelTests()
    {
        _mockSchoolRepository.GetSchoolContactsAsync(Arg.Any<int>()).Returns(_headTeacher);
        _mockMediator.Send(Arg.Any<GetSchoolDfeContactsQuery>()).Returns(new SchoolDfeContacts(null, null, null));
        MockTrustService.GetTrustSummaryAsync(AcademyUrn)
            .Returns(new TrustSummaryServiceModel("4321", "TR4321", "Some Trust", "Multi-academy trust", 1));

        Sut = new InSchoolModel(_mockMediator, _mockSchoolRepository, MockDataSourceService, _mockFeatureManager,
            MockSchoolService, MockTrustService, MockSchoolNavMenu) { Urn = SchoolUrn };
    }

    [Fact]
    public async Task OnGetAsync_sets_head_teacher()
    {
        await Sut.OnGetAsync();

        Sut.HeadTeacher.Should().Be(_headTeacher);
    }

    [Fact]
    public async Task OnGetAsync_sets_headteacher_to_empty_object_when_there_is_no_head_teacher()
    {
        _mockSchoolRepository.GetSchoolContactsAsync(Arg.Any<int>()).ReturnsNull();

        await Sut.OnGetAsync();

        Sut.HeadTeacher.Should().Be(new SchoolContact("", ""));
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Contacts");
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();
        Sut.PageMetadata.SubPageName.Should().Be("Contacts in this school");

        Sut.Urn = AcademyUrn;
        await Sut.OnGetAsync();
        Sut.PageMetadata.SubPageName.Should().Be("Contacts in this academy");
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "Contacts in this school", "In this school")]
    [InlineData(SchoolCategory.Academy, "Contacts in this academy", "In this academy")]
    public void Sub_page_names_include_school_type(SchoolCategory schoolCategory, string fullSubPageName,
        string subPageName)
    {
        InSchoolModel.FullSubPageName(schoolCategory).Should().Be(fullSubPageName);
        InSchoolModel.SubPageName(schoolCategory).Should().Be(subPageName);
    }

    [Fact]
    public void Sub_page_names_throw_for_unrecognised_school_type()
    {
        var fullSubPageName = () => InSchoolModel.FullSubPageName((SchoolCategory)999);
        var subPageName = () => InSchoolModel.SubPageName((SchoolCategory)999);

        fullSubPageName.Should().Throw<ArgumentOutOfRangeException>();
        subPageName.Should().Throw<ArgumentOutOfRangeException>();
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

    [Fact]
    public async Task OnGetAsync_does_not_query_dfe_contacts_when_feature_flag_is_disabled()
    {
        await Sut.OnGetAsync();

        await _mockMediator.DidNotReceive().Send(Arg.Any<GetSchoolDfeContactsQuery>());
    }

    [Fact]
    public async Task OnGetAsync_sets_in_dfe_data_sources_for_school_when_feature_flag_is_enabled()
    {
        _mockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);

        await Sut.OnGetAsync();

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In DfE",
                [new DataSourceListEntry(EmptyContactDataSource, "Regions group LA lead")]),
            new DataSourcePageListEntry("In this school",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Head teacher name")])
        ]);
        await _mockMediator.Received(1).Send(new GetSchoolDfeContactsQuery(SchoolUrn, null));
    }

    [Fact]
    public async Task OnGetAsync_sets_in_dfe_data_sources_for_academy_when_feature_flag_is_enabled()
    {
        _mockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);
        Sut.Urn = AcademyUrn;

        await Sut.OnGetAsync();

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In DfE", [
                new DataSourceListEntry(EmptyContactDataSource, "Trust relationship manager"),
                new DataSourceListEntry(EmptyContactDataSource, "SFSO (Schools financial support and oversight) lead")
            ]),
            new DataSourcePageListEntry("In this academy",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Head teacher name")])
        ]);
        await _mockMediator.Received(1).Send(new GetSchoolDfeContactsQuery(AcademyUrn, 4321));
    }
}
