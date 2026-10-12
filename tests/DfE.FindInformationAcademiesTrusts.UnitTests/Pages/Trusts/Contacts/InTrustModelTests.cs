using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Contacts;

public class InTrustModelTests : BaseTrustPageTests<InTrustModel>, ITestSubpages
{
    private readonly IMediator _mockMediator = Substitute.For<IMediator>();

    public InTrustModelTests()
    {
        MockTrustService.GetTrustContactsAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(new TrustContacts(null, null, null));

        Sut = new InTrustModel(_mockMediator, MockDataSourceService, MockTrustService) { Uid = TrustUid };
    }

    [Fact]
    public async Task OnGetAsync_sets_trust_contacts()
    {
        var accountingOfficer = new Person("Accounting Officer", "ao@test.com");
        var chairOfTrustees = new Person("Chair Of Trustees", "cot@test.com");
        var chiefFinancialOfficer = new Person("Chief Financial Officer", "cfo@test.com");
        MockTrustService.GetTrustContactsAsync(TrustUid, TrustReference)
            .Returns(new TrustContacts(accountingOfficer, chairOfTrustees, chiefFinancialOfficer));

        await Sut.OnGetAsync();

        Sut.AccountingOfficer.Should().Be(accountingOfficer);
        Sut.ChairOfTrustees.Should().Be(chairOfTrustees);
        Sut.ChiefFinancialOfficer.Should().Be(chiefFinancialOfficer);
    }

    [Fact]
    public async Task OnGetAsync_sets_trust_contacts_to_null_when_there_are_none()
    {
        await Sut.OnGetAsync();

        Sut.AccountingOfficer.Should().BeNull();
        Sut.ChairOfTrustees.Should().BeNull();
        Sut.ChiefFinancialOfficer.Should().BeNull();
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
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Contacts");
    }

    [Fact]
    public async Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("In this trust");
    }
}
