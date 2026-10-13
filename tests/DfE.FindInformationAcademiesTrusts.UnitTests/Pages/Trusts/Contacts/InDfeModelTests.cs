using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Contacts;

public class InDfeModelTests : BaseTrustPageTests<InDfeModel>, ITestSubpages
{
    private readonly IMediator _mockMediator = Substitute.For<IMediator>();

    private readonly TrustContact _trustRelationshipManager = new()
    {
        Uid = 1234, Role = TrustContactRole.TrustRelationshipManager, Name = "Trust Relationship Manager",
        Email = "trm@education.gov.uk", LastModifiedAtTime = new DateTime(2025, 1, 20),
        LastModifiedByEmail = "editor.one@education.gov.uk"
    };

    private readonly TrustContact _sfsoLead = new()
    {
        Uid = 1234, Role = TrustContactRole.SfsoLead, Name = "SFSO Lead", Email = "sfso@education.gov.uk",
        LastModifiedAtTime = new DateTime(2024, 2, 21), LastModifiedByEmail = "editor.two@education.gov.uk"
    };

    public InDfeModelTests()
    {
        Sut = new InDfeModel(_mockMediator, MockDataSourceService, MockTrustService) { Uid = TrustUid };
    }

    private void GivenTrustHasDfeContacts()
    {
        _mockMediator.Send(new GetTrustContactQuery(1234, TrustContactRole.TrustRelationshipManager))
            .Returns(_trustRelationshipManager);
        _mockMediator.Send(new GetTrustContactQuery(1234, TrustContactRole.SfsoLead))
            .Returns(_sfsoLead);
    }

    [Fact]
    public async Task OnGetAsync_sets_dfe_contacts()
    {
        GivenTrustHasDfeContacts();

        await Sut.OnGetAsync();

        Sut.TrustRelationshipManager.Should().Be(_trustRelationshipManager);
        Sut.SfsoLead.Should().Be(_sfsoLead);
    }

    [Fact]
    public async Task OnGetAsync_leaves_dfe_contacts_null_when_trust_has_none()
    {
        await Sut.OnGetAsync();

        Sut.TrustRelationshipManager.Should().BeNull();
        Sut.SfsoLead.Should().BeNull();
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await Sut.OnGetAsync();

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In DfE", [
                new DataSourceListEntry(new DataSourceServiceModel(Source.FiatDb, null, null),
                    "Trust relationship manager"),
                new DataSourceListEntry(new DataSourceServiceModel(Source.FiatDb, null, null),
                    "SFSO (Schools financial support and oversight) lead")
            ]),
            new DataSourcePageListEntry("In this trust", [
                new DataSourceListEntry(GiasDataSource, "Accounting officer name"),
                new DataSourceListEntry(GiasDataSource, "Chief financial officer name"),
                new DataSourceListEntry(GiasDataSource, "Chair of trustees name"),
                new DataSourceListEntry(MstrDataSource, "Accounting officer email"),
                new DataSourceListEntry(MstrDataSource, "Chief financial officer email"),
                new DataSourceListEntry(MstrDataSource, "Chair of trustees email")
            ])
        ]);
    }

    [Fact]
    public async Task OnGetAsync_takes_last_updated_details_from_the_dfe_contacts()
    {
        GivenTrustHasDfeContacts();

        await Sut.OnGetAsync();

        var inDfe = Sut.DataSourcesPerPage[0].DataSources;
        inDfe[0].DataSources[0].Should().Be(new DataSourceServiceModel(Source.FiatDb, new DateTime(2025, 1, 20),
            null, "editor.one@education.gov.uk"));
        inDfe[1].DataSources[0].Should().Be(new DataSourceServiceModel(Source.FiatDb, new DateTime(2024, 2, 21),
            null, "editor.two@education.gov.uk"));
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Contacts");
    }

    [Fact]
    public async Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("In DfE");
    }
}
