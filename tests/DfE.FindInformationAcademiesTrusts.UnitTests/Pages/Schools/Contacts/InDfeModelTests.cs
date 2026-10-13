using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Contacts;

public class InDfeModelTests : BaseSchoolPageTests<InDfeModel>
{
    private readonly IMediator _mockMediator = Substitute.For<IMediator>();

    private static readonly DateTime LastModified = new(2025, 6, 1);
    private const string LastModifiedBy = "editor@education.gov.uk";

    private readonly SchoolDfeContacts _contacts = new(
        new SchoolContact
        {
            Urn = SchoolUrn, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead, Name = "Regions Lead",
            Email = "rgl@education.gov.uk", LastModifiedAtTime = LastModified, LastModifiedByEmail = LastModifiedBy
        },
        new TrustContact
        {
            Uid = 4321, Role = TrustContactRole.TrustRelationshipManager, Name = "Trust Relationship Manager",
            Email = "trm@education.gov.uk", LastModifiedAtTime = LastModified, LastModifiedByEmail = LastModifiedBy
        },
        new TrustContact
        {
            Uid = 4321, Role = TrustContactRole.SfsoLead, Name = "SFSO Lead", Email = "sfso@education.gov.uk",
            LastModifiedAtTime = LastModified, LastModifiedByEmail = LastModifiedBy
        });

    private static DataSourceServiceModel ContactDataSource => new(Source.FiatDb, LastModified, null, LastModifiedBy);

    public InDfeModelTests()
    {
        _mockMediator.Send(Arg.Any<GetSchoolDfeContactsQuery>()).Returns(_contacts);
        MockTrustService.GetTrustSummaryAsync(AcademyUrn)
            .Returns(new TrustSummaryServiceModel("4321", "TR4321", "Some Trust", "Multi-academy trust", 1));

        Sut = new InDfeModel(_mockMediator, MockDataSourceService, MockSchoolService, MockTrustService,
            MockSchoolNavMenu) { Urn = SchoolUrn };
    }

    [Fact]
    public async Task OnGetAsync_sets_dfe_contacts()
    {
        await Sut.OnGetAsync();

        Sut.Contacts.Should().Be(_contacts);
    }

    [Fact]
    public async Task OnGetAsync_queries_with_trust_uid_for_academy()
    {
        Sut.Urn = AcademyUrn;

        await Sut.OnGetAsync();

        await _mockMediator.Received(1).Send(new GetSchoolDfeContactsQuery(AcademyUrn, 4321));
    }

    [Fact]
    public async Task OnGetAsync_queries_without_trust_uid_for_school_not_in_a_trust()
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
}
