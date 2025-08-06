using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.SchoolNavMenu;

public class SchoolNavMenuServiceNavTests : SchoolNavMenuTestsBase
{
    [Theory]
    [InlineData(123456)]
    [InlineData(567878)]
    public async Task GetServiceNavLinksAsync_should_set_route_data_to_urn(int expectedUrn)
    {
        var activePage = GetMockSchoolPage(typeof(DetailsModel), expectedUrn);

        var results = await Sut.GetServiceNavLinksAsync(activePage);

        results.Should().AllSatisfy(link =>
        {
            var route = link.AspAllRouteData.Should().ContainSingle().Subject;
            route.Key.Should().Be("urn");
            route.Value.Should().Be(expectedUrn.ToString());
        });
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "School")]
    [InlineData(SchoolCategory.Academy, "Academy")]
    public async Task GetServiceNavLinksAsync_should_set_hidden_text_to_school_type_on_details_page(
        SchoolCategory schoolCategory,
        string expectedHiddenText)
    {
        var activePage = GetMockSchoolPage(typeof(DetailsModel), schoolCategory: schoolCategory);

        var results = await Sut.GetServiceNavLinksAsync(activePage);

        results.Should().AllSatisfy(link => { link.VisuallyHiddenLinkText.Should().Be(expectedHiddenText); });
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "School")]
    [InlineData(SchoolCategory.Academy, "Academy")]
    public async Task GetServiceNavLinksAsync_should_set_hidden_text_to_school_type_on_contacts_page(
        SchoolCategory schoolCategory,
        string expectedHiddenText)
    {
        var activePage = GetMockSchoolPage(typeof(InSchoolModel), schoolCategory: schoolCategory);

        var results = await Sut.GetServiceNavLinksAsync(activePage);

        results.Should().AllSatisfy(link => { link.VisuallyHiddenLinkText.Should().Be(expectedHiddenText); });
    }

    [Fact]
    public async Task
        GetServiceNavLinksAsync_should_return_expected_links_when_ContactsInDfeForSchools_feature_flag_is_disabled()
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(false);
        var activePage = GetMockSchoolPage(typeof(DetailsModel));

        var results = await Sut.GetServiceNavLinksAsync(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be("Overview");
                l.AspPage.Should().Be("/Schools/Overview/Details");
                l.TestId.Should().Be("overview-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Contacts");
                l.AspPage.Should().Be("/Schools/Contacts/InSchool");
                l.TestId.Should().Be("contacts-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Governance");
                l.AspPage.Should().Be("/Schools/Governance/Current");
                l.TestId.Should().Be("governance-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Ofsted");
                l.AspPage.Should().Be("/Schools/Ofsted/SingleHeadlineGrades");
                l.TestId.Should().Be("ofsted-nav");
            }
        );
    }

    [Fact]
    public async Task
        GetServiceNavLinksAsync_should_return_expected_links_when_ContactsInDfeForSchools_feature_flag_is_enabled()
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);
        var activePage = GetMockSchoolPage(typeof(DetailsModel));

        var results = await Sut.GetServiceNavLinksAsync(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be("Overview");
                l.AspPage.Should().Be("/Schools/Overview/Details");
                l.TestId.Should().Be("overview-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Contacts");
                l.AspPage.Should().Be("/Schools/Contacts/InDfe");
                l.TestId.Should().Be("contacts-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Governance");
                l.AspPage.Should().Be("/Schools/Governance/Current");
                l.TestId.Should().Be("governance-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Ofsted");
                l.AspPage.Should().Be("/Schools/Ofsted/SingleHeadlineGrades");
                l.TestId.Should().Be("ofsted-nav");
            }
        );
    }

    [Theory]
    [MemberData(nameof(ContactsInDfeForSchoolsEnabledSubPageTypes))]
    public async Task
        GetServiceNavLinksAsync_should_set_active_page_link_when_on_any_subpage_and_ContactsInDfeForSchools_feature_flag_is_disabled(
            Type activePageType)
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(false);
        var activePage = GetMockSchoolPage(activePageType);
        var expectedActivePageLink = GetExpectedServiceNavAspLink(activePageType, false);

        var results = await Sut.GetServiceNavLinksAsync(activePage);

        results.Should().ContainSingle(l => l.LinkIsActive).Which.AspPage.Should().Be(expectedActivePageLink);
    }

    [Theory]
    [MemberData(nameof(ContactsInDfeForSchoolsEnabledSubPageTypes))]
    public async Task
        GetServiceNavLinksAsync_should_set_active_page_link_when_on_any_subpage_and_ContactsInDfeForSchools_feature_flag_is_enabled(
            Type activePageType)
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);
        var activePage = GetMockSchoolPage(activePageType);
        var expectedActivePageLink = GetExpectedServiceNavAspLink(activePageType, true);

        var results = await Sut.GetServiceNavLinksAsync(activePage);

        results.Should().ContainSingle(l => l.LinkIsActive).Which.AspPage.Should().Be(expectedActivePageLink);
    }

    private static string GetExpectedServiceNavAspLink(Type pageType, bool contactsInDfeForSchoolsFeatureFlagEnabled)
    {
        var contactLink = contactsInDfeForSchoolsFeatureFlagEnabled
            ? "/Schools/Contacts/InDfe"
            : "/Schools/Contacts/InSchool";

        return pageType.Name switch
        {
            nameof(DetailsModel) => "/Schools/Overview/Details",
            nameof(InDfeModel) => contactLink,
            nameof(InSchoolModel) => contactLink,
            nameof(SenModel) => "/Schools/Overview/Details",
            nameof(FederationModel) => "/Schools/Overview/Details",
            nameof(ReferenceNumbersModel) => "/Schools/Overview/Details",
            nameof(CurrentModel) => "/Schools/Governance/Current",
            _ => throw new ArgumentException("Couldn't get expected service nav asp link for given page type",
                nameof(pageType))
        };
    }
}
