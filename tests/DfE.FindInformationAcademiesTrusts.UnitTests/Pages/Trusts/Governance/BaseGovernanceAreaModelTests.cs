using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Governance;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Governance;

public abstract class BaseGovernanceAreaModelTests<T> : BaseTrustPageTests<T>, ITestSubpages
    where T : GovernanceAreaModel
{
    protected BaseGovernanceAreaModelTests()
    {
        MockTrustService.GetTrustGovernanceAsync(Arg.Any<string>())
            .Returns(Task.FromResult(new TrustGovernanceServiceModel([], [], [], [], 0)));
    }

    private static Governor[] GenerateGovernors(bool isCurrent, string role, int numberToGenerate)
    {
        return Enumerable.Repeat(new Governor(
            "9999",
            TrustUid,
            Role: role,
            FullName: "First Second Last",
            DateOfAppointment: DateTime.Today.AddYears(-3),
            DateOfTermEnd: isCurrent ? DateTime.Today.AddYears(1) : DateTime.Today.AddYears(-1),
            AppointingBody: "Nick Warms",
            Email: null
        ), numberToGenerate).ToArray();
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await Sut.OnGetAsync();

        await MockDataSourceService.Received(1).GetAsync(Source.Gias);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Trust leadership", [new DataSourceListEntry(GiasDataSource)]),
            new DataSourcePageListEntry("Trustees", [new DataSourceListEntry(GiasDataSource)]),
            new DataSourcePageListEntry("Members", [new DataSourceListEntry(GiasDataSource)]),
            new DataSourcePageListEntry("Historic members", [new DataSourceListEntry(GiasDataSource)])
        ]);
    }

    [Fact]
    public async Task OnGetAsync_sets_TrustGovernance()
    {
        var trustGovernanceServiceModelWithData = new TrustGovernanceServiceModel(
            GenerateGovernors(true, "Chair of Trustees", 4),
            GenerateGovernors(true, "Member", 3),
            GenerateGovernors(true, "Trustee", 2),
            GenerateGovernors(false, "Trustee", 1),
            10);

        MockTrustService.GetTrustGovernanceAsync(TrustUid)
            .Returns(Task.FromResult(trustGovernanceServiceModelWithData));

        await Sut.OnGetAsync();

        await MockTrustService.Received(1).GetTrustGovernanceAsync(TrustUid);
        Sut.TrustGovernance.Should().BeEquivalentTo(trustGovernanceServiceModelWithData);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_PageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Governance");
    }

    [Fact]
    public abstract Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName();
}
