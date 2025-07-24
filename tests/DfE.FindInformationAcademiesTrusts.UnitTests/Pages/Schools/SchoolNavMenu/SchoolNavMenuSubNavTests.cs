using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.SchoolNavMenu;

public class SchoolNavMenuSubNavTests : SchoolNavMenuTestsBase
{
    [Theory]
    [InlineData(123456)]
    [InlineData(567890)]
    public async Task GetSubNavLinksAsync_should_set_route_data_to_urn(int expectedUrn)
    {
        var activePage = GetMockSchoolPage(typeof(DetailsModel), expectedUrn);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().AllSatisfy(link =>
        {
            var route = link.AspAllRouteData.Should().ContainSingle().Subject;
            route.Key.Should().Be("urn");
            route.Value.Should().Be(expectedUrn.ToString());
        });
    }

    [Theory]
    [MemberData(nameof(ContactsInDfeForSchoolsEnabledSubPageTypes))]
    public async Task GetSubNavLinksAsync_should_set_hidden_text_to_page_name(Type activePageType)
    {
        var activePage = GetMockSchoolPage(activePageType);
        var expectedPageName = GetExpectedPageName(activePageType);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().AllSatisfy(link => { link.VisuallyHiddenLinkText.Should().Be(expectedPageName); });
    }

    private static string GetExpectedPageName(Type pageType)
    {
        return pageType.Name switch
        {
            nameof(DetailsModel) => "Overview",
            nameof(InDfeModel) => "Contacts",
            nameof(InSchoolModel) => "Contacts",
            nameof(SenModel) => "Overview",
            nameof(FederationModel) => "Overview",
            nameof(ReferenceNumbersModel) => "Overview",
            _ => throw new ArgumentException("Couldn't get expected name for given page type", nameof(pageType))
        };
    }

    [Theory]
    [MemberData(nameof(ContactsInDfeForSchoolsDisabledSubPageTypes))]
    public async Task
        GetSubNavLinksAsync_should_set_active_sub_page_link_when_ContactsInDfeForSchools_feature_flag_is_disabled(
            Type activePageType)
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(false);
        var activePage = GetMockSchoolPage(activePageType);
        var expectedActiveSubPageLink = GetSubPageLinkTo(activePageType, false);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().ContainSingle(l => l.LinkIsActive).Which.AspPage.Should().Be(expectedActiveSubPageLink);
    }

    [Theory]
    [MemberData(nameof(ContactsInDfeForSchoolsEnabledSubPageTypes))]
    public async Task
        GetSubNavLinksAsync_should_set_active_sub_page_link_when_ContactsInDfeForSchools_feature_flag_is_enabled(
            Type activePageType)
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);
        var activePage = GetMockSchoolPage(activePageType);
        var expectedActiveSubPageLink = GetSubPageLinkTo(activePageType, true);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().ContainSingle(l => l.LinkIsActive).Which.AspPage.Should().Be(expectedActiveSubPageLink);
    }

    private static string GetSubPageLinkTo(Type pageType, bool contactsInDfeForSchoolsFeatureFlagEnabled)
    {
        return pageType.Name switch
        {
            nameof(DetailsModel) => "/Schools/Overview/Details",
            nameof(InDfeModel) => "/Schools/Contacts/InDfe",
            nameof(InSchoolModel) => "/Schools/Contacts/InSchool",
            nameof(SenModel) => "/Schools/Overview/Sen",
            nameof(FederationModel) => "/Schools/Overview/Federation",
            nameof(ReferenceNumbersModel) => "/Schools/Overview/ReferenceNumbers",
            _ => throw new ArgumentException("Couldn't get expected sub page nav asp link for given page type",
                nameof(pageType))
        };
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "School details")]
    [InlineData(SchoolCategory.Academy, "Academy details")]
    public async Task GetSubNavLinksAsync_should_return_expected_links_for_overview_when_federation(
        SchoolCategory schoolCategory,
        string expectedText)
    {
        var activePage = GetMockSchoolPage(typeof(DetailsModel), schoolCategory: schoolCategory);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be(expectedText);
                l.AspPage.Should().Be("/Schools/Overview/Details");
                l.TestId.Should().Be("overview-details-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Federation details");
                l.AspPage.Should().Be("/Schools/Overview/Federation");
                l.TestId.Should().Be("overview-federation-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Reference numbers");
                l.AspPage.Should().Be("/Schools/Overview/ReferenceNumbers");
                l.TestId.Should().Be("overview-reference-numbers-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("SEN (special educational needs)");
                l.AspPage.Should().Be("/Schools/Overview/Sen");
                l.TestId.Should().Be("overview-sen-subnav");
            }
        );
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "School details")]
    [InlineData(SchoolCategory.Academy, "Academy details")]
    public async Task GetSubNavLinksAsync_should_return_expected_links_for_overview_when_not_federation(
        SchoolCategory schoolCategory,
        string expectedText)
    {
        var activePage = GetMockSchoolPage(typeof(DetailsModel), schoolCategory: schoolCategory, isFederation: false);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be(expectedText);
                l.AspPage.Should().Be("/Schools/Overview/Details");
                l.TestId.Should().Be("overview-details-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Reference numbers");
                l.AspPage.Should().Be("/Schools/Overview/ReferenceNumbers");
                l.TestId.Should().Be("overview-reference-numbers-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("SEN (special educational needs)");
                l.AspPage.Should().Be("/Schools/Overview/Sen");
                l.TestId.Should().Be("overview-sen-subnav");
            }
        );
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "In this school")]
    [InlineData(SchoolCategory.Academy, "In this academy")]
    public async Task
        GetSubNavLinksAsync_should_return_expected_links_for_contacts_when_ContactsInDfeForSchools_feature_flag_is_disabled(
            SchoolCategory schoolCategory,
            string expectedText)
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(false);
        var activePage = GetMockSchoolPage(typeof(InSchoolModel), schoolCategory: schoolCategory);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().SatisfyRespectively(l =>
            {
                l.LinkDisplayText.Should().Be(expectedText);
                l.AspPage.Should().Be("/Schools/Contacts/InSchool");
                l.TestId.Should().Be("contacts-in-this-school-subnav");
            }
        );
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "In this school")]
    [InlineData(SchoolCategory.Academy, "In this academy")]
    public async Task
        GetSubNavLinksAsync_should_return_expected_links_for_contacts_when_ContactsInDfeForSchools_feature_flag_is_enabled(
            SchoolCategory schoolCategory,
            string expectedText)
    {
        MockFeatureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools).Returns(true);
        var activePage = GetMockSchoolPage(typeof(InSchoolModel), schoolCategory: schoolCategory);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().SatisfyRespectively(l =>
            {
                l.LinkDisplayText.Should().Be("In DfE");
                l.AspPage.Should().Be("/Schools/Contacts/InDfe");
                l.TestId.Should().Be("contacts-in-dfe-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be(expectedText);
                l.AspPage.Should().Be("/Schools/Contacts/InSchool");
                l.TestId.Should().Be("contacts-in-this-school-subnav");
            }
        );
    }

    [Fact]
    public async Task GetSubNavLinksAsync_should_throw_if_page_not_supported()
    {
        var activePage = Substitute.For<ISchoolAreaModel>();

        var action = async () => await Sut.GetSubNavLinksAsync(activePage);

        var result = await action.Should().ThrowAsync<ArgumentException>();
        result.Which.Message.Should().StartWith("Page type is not supported.");
    }
}
