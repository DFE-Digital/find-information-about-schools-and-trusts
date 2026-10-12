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
        Email = "trm@test.com", LastModifiedAtTime = new DateTime(2025, 01, 20), LastModifiedByEmail = "test@email.com"
    };

    private readonly TrustContact _sfsoLead = new()
    {
        Uid = 1234, Role = TrustContactRole.SfsoLead, Name = "SFSO Lead", Email = "sfsol@test.com",
        LastModifiedAtTime = new DateTime(2024, 02, 21), LastModifiedByEmail = "test2@email.com"
    };

    public InDfeModelTests()
    {
        Sut = new InDfeModel(_mockMediator, MockDataSourceService, MockTrustService) { Uid = TrustUid };
    }

    private void SetUpContacts()
    {
        _mockMediator.Send(new GetTrustContactQuery(1234, TrustContactRole.TrustRelationshipManager))
            .Returns(_trustRelationshipManager);
        _mockMediator.Send(new GetTrustContactQuery(1234, TrustContactRole.SfsoLead)).Returns(_sfsoLead);
    }

    [Fact]
    public async Task OnGetAsync_sets_contacts_from_mediator()
    {
        SetUpContacts();

        await Sut.OnGetAsync();

        Sut.TrustRelationshipManager.Should().Be(_trustRelationshipManager);
        Sut.SfsoLead.Should().Be(_sfsoLead);
    }

    [Fact]
    public async Task OnGetAsync_sets_contacts_to_null_when_there_are_none()
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
    public async Task OnGetAsync_sets_last_modified_details_in_data_source_list()
    {
        SetUpContacts();

        await Sut.OnGetAsync();

        Sut.DataSourcesPerPage[0].DataSources[0].DataSources[0].Should().Be(
            new DataSourceServiceModel(Source.FiatDb, new DateTime(2025, 01, 20), null, "test@email.com"));
        Sut.DataSourcesPerPage[0].DataSources[1].DataSources[0].Should().Be(
            new DataSourceServiceModel(Source.FiatDb, new DateTime(2024, 02, 21), null, "test2@email.com"));
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
