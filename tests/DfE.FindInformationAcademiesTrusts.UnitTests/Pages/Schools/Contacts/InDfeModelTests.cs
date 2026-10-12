using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;
using Microsoft.FeatureManagement;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Contacts;

public class InDfeModelTests : BaseSchoolPageTests<InDfeModel>
{
    private readonly IMediator _mockMediator = Substitute.For<IMediator>();
    private readonly IVariantFeatureManager _mockFeatureManager = Substitute.For<IVariantFeatureManager>();

    private static readonly DateTime LastModified = new(2025, 6, 1);

    private readonly SchoolDfeContacts _contacts = new(
        new SchoolContact
        {
            Urn = SchoolUrn, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead, Name = "Aaron Aaronson",
            Email = "aa@education.gov.uk", LastModifiedAtTime = LastModified, LastModifiedByEmail = "a@b.com"
        },
        new TrustContact
        {
            Uid = 4321, Role = TrustContactRole.TrustRelationshipManager, Name = "Bertha Billingsley",
            Email = "bb@education.gov.uk", LastModifiedAtTime = LastModified, LastModifiedByEmail = "a@b.com"
        },
        new TrustContact
        {
            Uid = 4321, Role = TrustContactRole.SfsoLead, Name = "Carlton Coriander", Email = "cc@education.gov.uk",
            LastModifiedAtTime = LastModified, LastModifiedByEmail = "a@b.com"
        });

    private DataSourceServiceModel ContactDataSource => new(Source.FiatDb, LastModified, null, "a@b.com");

    public InDfeModelTests()
    {
        _mockMediator.Send(Arg.Any<GetSchoolDfeContactsQuery>()).Returns(_contacts);
        _mockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);
        MockTrustService.GetTrustSummaryAsync(AcademyUrn)
            .Returns(new TrustSummaryServiceModel("4321", "TR4321", "Some Trust", "Some trust type", 1));

        Sut = new InDfeModel(_mockMediator, MockDataSourceService, _mockFeatureManager, MockSchoolService,
            MockTrustService, MockSchoolNavMenu) { Urn = SchoolUrn };
    }

    [Fact]
    public async Task OnGetAsync_should_set_contacts()
    {
        await Sut.OnGetAsync();

        Sut.Contacts.Should().Be(_contacts);
    }

    [Fact]
    public async Task OnGetAsync_should_query_trust_contacts_for_academy()
    {
        Sut.Urn = AcademyUrn;

        await Sut.OnGetAsync();

        await _mockMediator.Received(1).Send(new GetSchoolDfeContactsQuery(AcademyUrn, 4321));
    }

    [Fact]
    public async Task OnGetAsync_should_not_query_trust_contacts_for_school_without_trust()
    {
        await Sut.OnGetAsync();

        await _mockMediator.Received(1).Send(new GetSchoolDfeContactsQuery(SchoolUrn, null));
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

        Sut.PageMetadata.SubPageName.Should().Be("In DfE");
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await Sut.OnGetAsync();

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In DfE", [new DataSourceListEntry(ContactDataSource, "Regions group LA lead")]),
            new DataSourcePageListEntry("In this school",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Head teacher name")])
        ]);
    }

    [Fact]
    public async Task OnGetAsync_sets_correct_data_source_list_for_academy()
    {
        Sut.Urn = AcademyUrn;

        await Sut.OnGetAsync();

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In DfE", [
                new DataSourceListEntry(ContactDataSource, "Trust relationship manager"),
                new DataSourceListEntry(ContactDataSource, "SFSO (Schools financial support and oversight) lead")
            ]),
            new DataSourcePageListEntry("In this academy",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Head teacher name")])
        ]);
    }

    [Fact]
    public async Task OnGetAsync_omits_in_dfe_data_sources_when_feature_flag_is_disabled()
    {
        _mockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(false);

        await Sut.OnGetAsync();

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In this school",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Head teacher name")])
        ]);
    }
}
