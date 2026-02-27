using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.Pipeline;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.FinancialDocuments;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Overview;
using Sut = DfE.FindInformationAcademiesTrusts.Pages.Trusts.TrustNavMenu;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.TrustNavMenu;

public class ServiceNav : TrustNavMenuTestsBase
{
    [Theory]
    [InlineData("1234")]
    [InlineData("5678")]
    public void GetServiceNavLinks_should_set_route_data_to_uid(string expectedUid)
    {
        var activePage = GetMockTrustPage(typeof(TrustDetailsModel), expectedUid);

        var results = Sut.GetServiceNavLinks(activePage);

        results.Should().AllSatisfy(link =>
        {
            var route = link.AspAllRouteData.Should().ContainSingle().Subject;
            route.Key.Should().Be("uid");
            route.Value.Should().Be(expectedUid);
        });
    }

    [Fact]
    public void GetServiceNavLinks_should_set_hidden_text_to_trust()
    {
        var activePage = GetMockTrustPage(typeof(InTrustModel));

        var results = Sut.GetServiceNavLinks(activePage);

        results.Should().AllSatisfy(link => { link.VisuallyHiddenLinkText.Should().Be("Trust"); });
    }

    [Fact]
    public void GetServiceNavLinks_should_return_expected_links()
    {
        var activePage = GetMockTrustPage(typeof(TrustDetailsModel));

        var results = Sut.GetServiceNavLinks(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be("Overview");
                l.AspPage.Should().Be("/Trusts/Overview/TrustDetails");
                l.TestId.Should().Be("overview-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Contacts");
                l.AspPage.Should().Be("/Trusts/Contacts/InDfe");
                l.TestId.Should().Be("contacts-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Academies (3)");
                l.AspPage.Should().Be("/Trusts/Academies/InTrust/Details");
                l.TestId.Should().Be("academies-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Ofsted");
                l.AspPage.Should().Be("/Trusts/Ofsted/Overview");
                l.TestId.Should().Be("ofsted-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Financial documents");
                l.AspPage.Should().Be("/Trusts/FinancialDocuments/FinancialStatements");
                l.TestId.Should().Be("financial-documents-nav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Governance");
                l.AspPage.Should().Be("/Trusts/Governance/TrustLeadership");
                l.TestId.Should().Be("governance-nav");
            }
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    public void GetServiceNavLinks_should_show_number_of_academies_in_display_text(int numberOfAcademies)
    {
        var activePage = GetMockTrustPage(typeof(TrustDetailsModel), "1234", numberOfAcademies);

        var results = Sut.GetServiceNavLinks(activePage);

        results.Should().ContainSingle(l => l.TestId == "academies-nav")
            .Which.LinkDisplayText.Should().Be($"Academies ({numberOfAcademies})");
    }

    [Theory]
    [MemberData(nameof(SubPageTypes))]
    public void GetServiceNavLinks_should_set_active_page_link_when_on_any_subpage(Type activePageType)
    {
        var activePage = GetMockTrustPage(activePageType);
        var expectedActivePageLink = GetExpectedServiceNavAspLink(activePageType);

        var results = Sut.GetServiceNavLinks(activePage);

        results.Should().ContainSingle(l => l.LinkIsActive).Which.AspPage.Should().Be(expectedActivePageLink);
    }

    private static string GetExpectedServiceNavAspLink(Type pageType)
    {
        return pageType.Name switch
        {
            nameof(TrustDetailsModel) or
                nameof(TrustSummaryModel) or
                nameof(ReferenceNumbersModel) => "/Trusts/Overview/TrustDetails",
            nameof(InDfeModel) or
                nameof(InTrustModel) => "/Trusts/Contacts/InDfe",
            nameof(AcademiesInTrustDetailsModel) or
                nameof(PupilNumbersModel) or
                nameof(FreeSchoolMealsModel) or
                nameof(PreAdvisoryBoardModel) or
                nameof(PostAdvisoryBoardModel) or
                nameof(FreeSchoolsModel) => "/Trusts/Academies/InTrust/Details",
            nameof(OverviewModel) => "/Trusts/Ofsted/Overview",
            nameof(FinancialStatementsModel) or
                nameof(ManagementLettersModel) or
                nameof(InternalScrutinyReportsModel) or
                nameof(SelfAssessmentChecklistsModel) => "/Trusts/FinancialDocuments/FinancialStatements",
            nameof(TrustLeadershipModel) or
                nameof(TrusteesModel) or
                nameof(MembersModel) or
                nameof(HistoricMembersModel) => "/Trusts/Governance/TrustLeadership",
            _ => throw new ArgumentException("Couldn't get expected service nav asp link for given page type",
                nameof(pageType))
        };
    }
}
