using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.FeatureManagement;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Contacts;

public abstract class BaseContactsAreaModelTests<T> : BaseSchoolPageTests<T> where T : ContactsAreaModel
{
    protected readonly IVariantFeatureManager MockFeatureManager = Substitute.For<IVariantFeatureManager>();

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Contacts");
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await OnGetAsync_sets_correct_data_source_list_for_academy_when_ContactsInDfeForSchools_feature_flag_is_enabled();
        MockDataSourceService.ClearReceivedCalls();
        MockTrustService.ClearReceivedCalls();

        await OnGetAsync_sets_correct_data_source_list_for_academy_when_ContactsInDfeForSchools_feature_flag_is_disabled();
        MockDataSourceService.ClearReceivedCalls();
        MockTrustService.ClearReceivedCalls();

        await OnGetAsync_sets_correct_data_source_list_for_school_when_ContactsInDfeForSchools_feature_flag_is_enabled();
        MockDataSourceService.ClearReceivedCalls();

        await OnGetAsync_sets_correct_data_source_list_for_school_when_ContactsInDfeForSchools_feature_flag_is_disabled();
        MockDataSourceService.ClearReceivedCalls();
    }

    private async Task OnGetAsync_sets_correct_data_source_list_for_academy_when_ContactsInDfeForSchools_feature_flag_is_enabled()
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);
        MockTrustService.GetTrustSummaryAsync(AcademyUrn)
            .Returns(new TrustSummaryServiceModel("4321", "Some Trust", "Some trust type", 1));

        Sut.Urn = AcademyUrn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);
        await MockDataSourceService.Received(1)
            .GetTrustContactDataSourceAsync(4321, TrustContactRole.TrustRelationshipManager);
        await MockDataSourceService.Received(1)
            .GetTrustContactDataSourceAsync(4321, TrustContactRole.SfsoLead);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In DfE", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Fiat, "Trust relationship manager"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Fiat,
                    "SFSO (Schools financial support and oversight) lead")
            ]),
            new DataSourcePageListEntry("In this academy", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Head teacher name")
            ])
        ]);
    }

    private async Task OnGetAsync_sets_correct_data_source_list_for_academy_when_ContactsInDfeForSchools_feature_flag_is_disabled()
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(false);
        MockTrustService.GetTrustSummaryAsync(AcademyUrn)
            .Returns(new TrustSummaryServiceModel("4321", "Some Trust", "Some trust type", 1));

        Sut.Urn = AcademyUrn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);
        await MockDataSourceService.DidNotReceive()
            .GetTrustContactDataSourceAsync(4321, TrustContactRole.TrustRelationshipManager);
        await MockDataSourceService.DidNotReceive()
            .GetTrustContactDataSourceAsync(4321, TrustContactRole.SfsoLead);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In this academy", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Head teacher name")
            ])
        ]);
    }

    private async Task OnGetAsync_sets_correct_data_source_list_for_school_when_ContactsInDfeForSchools_feature_flag_is_enabled()
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);

        Sut.Urn = SchoolUrn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);
        await MockDataSourceService.Received(1)
            .GetSchoolContactDataSourceAsync(SchoolUrn, SchoolContactRole.RegionsGroupLocalAuthorityLead);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In DfE", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Fiat, "Regions group LA lead")
            ]),
            new DataSourcePageListEntry("In this school", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias, DataField: "Head teacher name")
            ])
        ]);
    }

    private async Task OnGetAsync_sets_correct_data_source_list_for_school_when_ContactsInDfeForSchools_feature_flag_is_disabled()
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(false);

        Sut.Urn = SchoolUrn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);
        await MockDataSourceService.DidNotReceive()
            .GetSchoolContactDataSourceAsync(SchoolUrn, SchoolContactRole.RegionsGroupLocalAuthorityLead);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("In this school", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias, DataField: "Head teacher name")
            ])
        ]);
    }
}
