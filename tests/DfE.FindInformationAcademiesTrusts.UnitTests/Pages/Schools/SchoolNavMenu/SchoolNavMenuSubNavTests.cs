using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Pages.Schools;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;
using DfE.FindInformationAcademiesTrusts.Services.School;

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
            nameof(ReligiousCharacteristicsModel) => "Overview",
            nameof(PopulationModel) => "Pupils",
            nameof(CurrentModel) => "Governance",
            nameof(HistoricModel) => "Governance",
            nameof(SingleHeadlineGradesModel) => "Ofsted",
            nameof(CurrentRatingsModel) => "Ofsted",
            nameof(PreviousRatingsModel) => "Ofsted",
            nameof(SafeguardingAndConcernsModel) => "Ofsted",
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
        var expectedActiveSubPageLink = GetSubPageLinkTo(activePageType);

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
        var expectedActiveSubPageLink = GetSubPageLinkTo(activePageType);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().ContainSingle(l => l.LinkIsActive).Which.AspPage.Should().Be(expectedActiveSubPageLink);
    }

    private static string GetSubPageLinkTo(Type pageType)
    {
        return pageType.Name switch
        {
            nameof(DetailsModel) => "/Schools/Overview/Details",
            nameof(InDfeModel) => "/Schools/Contacts/InDfe",
            nameof(InSchoolModel) => "/Schools/Contacts/InSchool",
            nameof(SenModel) => "/Schools/Overview/Sen",
            nameof(FederationModel) => "/Schools/Overview/Federation",
            nameof(ReferenceNumbersModel) => "/Schools/Overview/ReferenceNumbers",
            nameof(PopulationModel) => "/Schools/Pupils/Population",
            nameof(CurrentModel) => "/Schools/Governance/Current",
            nameof(HistoricModel) => "/Schools/Governance/Historic",
            nameof(SingleHeadlineGradesModel) => "/Schools/Ofsted/SingleHeadlineGrades",
            nameof(CurrentRatingsModel) => "/Schools/Ofsted/CurrentRatings",
            nameof(PreviousRatingsModel) => "/Schools/Ofsted/PreviousRatings",
            nameof(ReligiousCharacteristicsModel) => "/Schools/Overview/ReligiousCharacteristics",
            nameof(SafeguardingAndConcernsModel) => "/Schools/Ofsted/SafeguardingAndConcerns",
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
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Religious characteristics");
                l.AspPage.Should().Be("/Schools/Overview/ReligiousCharacteristics");
                l.TestId.Should().Be("overview-religious-characteristics-subnav");
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
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Religious characteristics");
                l.AspPage.Should().Be("/Schools/Overview/ReligiousCharacteristics");
                l.TestId.Should().Be("overview-religious-characteristics-subnav");
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

    public static TheoryData<int, int, string, string> GovernanceData => new()
    {
        { 0, 0, "Current governors (0)", "Historic governors (0)" },
        { 2, 0, "Current governors (2)", "Historic governors (0)" },
        { 0, 2, "Current governors (0)", "Historic governors (2)" },
        { 20, 10, "Current governors (20)", "Historic governors (10)" }
    };

    private static Governor[] GenerateGovernors(bool isCurrent, string role, int numberToGenerate)
    {
        return Enumerable.Repeat(new Governor(
            "9999",
            string.Empty,
            Role: role,
            FullName: "First Second Last",
            DateOfAppointment: DateTime.Today.AddYears(-3),
            DateOfTermEnd: isCurrent ? DateTime.Today.AddYears(1) : DateTime.Today.AddYears(-1),
            AppointingBody: "School board",
            Email: null
        ), numberToGenerate).ToArray();
    }

    [Theory]
    [MemberData(nameof(GovernanceData))]
    public async Task GetSubNavLinksAsync_for_governance_should_display_number_in_title(int numberOfCurrent,
        int numberOfHistoric, string expectedCurrent, string expectedHistoric)
    {
        var currentGovernance = GenerateGovernors(true, "member", numberOfCurrent);
        var historicGovernance = GenerateGovernors(false, "member", numberOfHistoric);

        var activePage = (GovernanceAreaModel)GetMockSchoolPage(typeof(CurrentModel));
        activePage.SchoolGovernance = new SchoolGovernanceServiceModel(currentGovernance, historicGovernance);

        var results = await Sut.GetSubNavLinksAsync(activePage);

        results.Should().SatisfyRespectively(l =>
            {
                l.LinkDisplayText.Should().Be(expectedCurrent);
                l.AspPage.Should().Be("/Schools/Governance/Current");
                l.TestId.Should().Be("current-governors-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be(expectedHistoric);
                l.AspPage.Should().Be("/Schools/Governance/Historic");
                l.TestId.Should().Be("historic-governors-subnav");
            }
        );
    }
}
