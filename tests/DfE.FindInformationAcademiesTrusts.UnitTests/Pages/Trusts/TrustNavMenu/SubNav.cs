using DfE.FindInformationAcademiesTrusts.Pages.Trusts;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.Pipeline;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.FinancialDocuments;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Overview;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Sut = DfE.FindInformationAcademiesTrusts.Pages.Trusts.TrustNavMenu;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.TrustNavMenu;

public class SubNav : TrustNavMenuTestsBase
{
    [Theory]
    [InlineData("1234")]
    [InlineData("5678")]
    public void GetSubNavLinks_should_set_route_data_to_uid(string expectedUid)
    {
        var activePage = GetMockTrustPage(typeof(TrustDetailsModel), expectedUid);

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().AllSatisfy(link =>
        {
            var route = link.AspAllRouteData.Should().ContainSingle().Subject;
            route.Key.Should().Be("uid");
            route.Value.Should().Be(expectedUid);
        });
    }

    [Theory]
    [MemberData(nameof(SubPageTypes))]
    public void GetSubNavLinks_should_set_hidden_text_to_page_name(Type activePageType)
    {
        var activePage = GetMockTrustPage(activePageType);
        var expectedPageName = GetExpectedPageName(activePageType);

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().AllSatisfy(link => { link.VisuallyHiddenLinkText.Should().Be(expectedPageName); });
    }

    private static string GetExpectedPageName(Type pageType)
    {
        return pageType.Name switch
        {
            nameof(TrustDetailsModel) or
                nameof(TrustSummaryModel) or
                nameof(ReferenceNumbersModel) => "Overview",
            nameof(InDfeModel) or
                nameof(InTrustModel) => "Contacts",
            nameof(AcademiesInTrustDetailsModel) or
                nameof(PupilNumbersModel) or
                nameof(FreeSchoolMealsModel) or
                nameof(PreAdvisoryBoardModel) or
                nameof(PostAdvisoryBoardModel) or
                nameof(FreeSchoolsModel) => "Academies",
            nameof(SingleHeadlineGradesModel) or
                nameof(CurrentRatingsModel) or
                nameof(PreviousRatingsModel) or
                nameof(SafeguardingAndConcernsModel) => "Ofsted",
            nameof(FinancialStatementsModel) or
                nameof(ManagementLettersModel) or
                nameof(InternalScrutinyReportsModel) or
                nameof(SelfAssessmentChecklistsModel) => "Financial documents",
            nameof(TrustLeadershipModel) or
                nameof(TrusteesModel) or
                nameof(MembersModel) or
                nameof(HistoricMembersModel) => "Governance",
            _ => throw new ArgumentException("Couldn't get expected name for given page type", nameof(pageType))
        };
    }

    [Theory]
    [MemberData(nameof(SubPageTypes))]
    public void GetSubNavLinks_should_set_active_sub_page_link(Type activePageType)
    {
        var activePage = GetMockTrustPage(activePageType);
        var expectedActiveSubPageLink = GetSubPageLinkTo(activePageType);

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().ContainSingle(l => l.LinkIsActive).Which.AspPage.Should().Be(expectedActiveSubPageLink);
    }

    private static string GetSubPageLinkTo(Type pageType)
    {
        return pageType.Name switch
        {
            nameof(TrustDetailsModel) => "/Trusts/Overview/TrustDetails",
            nameof(TrustSummaryModel) => "/Trusts/Overview/TrustSummary",
            nameof(ReferenceNumbersModel) => "/Trusts/Overview/ReferenceNumbers",
            nameof(InDfeModel) => "/Trusts/Contacts/InDfE",
            nameof(InTrustModel) => "/Trusts/Contacts/InTrust",
            nameof(AcademiesInTrustDetailsModel) => "/Trusts/Academies/InTrust/Details",
            nameof(PupilNumbersModel) => "/Trusts/Academies/InTrust/Details",
            nameof(FreeSchoolMealsModel) => "/Trusts/Academies/InTrust/Details",
            nameof(PreAdvisoryBoardModel) => "/Trusts/Academies/Pipeline/PreAdvisoryBoard",
            nameof(PostAdvisoryBoardModel) => "/Trusts/Academies/Pipeline/PreAdvisoryBoard",
            nameof(FreeSchoolsModel) => "/Trusts/Academies/Pipeline/PreAdvisoryBoard",
            nameof(SingleHeadlineGradesModel) => "/Trusts/Ofsted/SingleHeadlineGrades",
            nameof(CurrentRatingsModel) => "/Trusts/Ofsted/CurrentRatings",
            nameof(PreviousRatingsModel) => "/Trusts/Ofsted/PreviousRatings",
            nameof(SafeguardingAndConcernsModel) => "/Trusts/Ofsted/SafeguardingAndConcerns",
            nameof(FinancialStatementsModel) => "/Trusts/FinancialDocuments/FinancialStatements",
            nameof(ManagementLettersModel) => "/Trusts/FinancialDocuments/ManagementLetters",
            nameof(InternalScrutinyReportsModel) => "/Trusts/FinancialDocuments/InternalScrutinyReports",
            nameof(SelfAssessmentChecklistsModel) => "/Trusts/FinancialDocuments/SelfAssessmentChecklists",
            nameof(TrustLeadershipModel) => "/Trusts/Governance/TrustLeadership",
            nameof(TrusteesModel) => "/Trusts/Governance/Trustees",
            nameof(MembersModel) => "/Trusts/Governance/Members",
            nameof(HistoricMembersModel) => "/Trusts/Governance/HistoricMembers",
            _ => throw new ArgumentException("Couldn't get expected sub page nav asp link for given page type",
                nameof(pageType))
        };
    }

    [Fact]
    public void GetSubNavLinks_should_return_expected_links_for_overview()
    {
        var activePage = GetMockTrustPage(typeof(TrustDetailsModel));

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be("Trust details");
                l.AspPage.Should().Be("/Trusts/Overview/TrustDetails");
                l.TestId.Should().Be("overview-trust-details-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Trust summary");
                l.AspPage.Should().Be("/Trusts/Overview/TrustSummary");
                l.TestId.Should().Be("overview-trust-summary-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Reference numbers");
                l.AspPage.Should().Be("/Trusts/Overview/ReferenceNumbers");
                l.TestId.Should().Be("overview-reference-numbers-subnav");
            }
        );
    }

    [Fact]
    public void GetSubNavLinks_should_return_expected_links_for_contacts()
    {
        var activePage = GetMockTrustPage(typeof(InTrustModel));

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be("In DfE");
                l.AspPage.Should().Be("/Trusts/Contacts/InDfE");
                l.TestId.Should().Be("contacts-in-dfe-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("In this trust");
                l.AspPage.Should().Be("/Trusts/Contacts/InTrust");
                l.TestId.Should().Be("contacts-in-this-trust-subnav");
            }
        );
    }

    [Theory]
    [MemberData(nameof(AcademyPageTabTypes))]
    public void GetSubNavLinks_should_return_expected_links_for_academies_tabs(Type tabPageType)
    {
        var activePage = GetMockTrustPage(tabPageType);

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be("In this trust (3)");
                l.AspPage.Should().Be("/Trusts/Academies/InTrust/Details");
                l.TestId.Should().Be("academies-in-this-trust-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Pipeline academies (6)");
                l.AspPage.Should().Be("/Trusts/Academies/Pipeline/PreAdvisoryBoard");
                l.TestId.Should().Be("academies-pipeline-academies-subnav");
            }
        );
    }

    public static TheoryData<Type> AcademyPageTabTypes =>
    [
        typeof(AcademiesInTrustDetailsModel),
        typeof(PupilNumbersModel),
        typeof(FreeSchoolMealsModel),
        typeof(PreAdvisoryBoardModel),
        typeof(PostAdvisoryBoardModel),
        typeof(FreeSchoolsModel)
    ];

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    public void GetSubNavLinks_should_show_number_of_academies_in_this_trust_subnav_display_text(int numberOfAcademies)
    {
        var activePage = GetMockTrustPage(typeof(PostAdvisoryBoardModel), numberOfAcademies: numberOfAcademies);

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().ContainSingle(l => l.TestId == "academies-in-this-trust-subnav")
            .Which.LinkDisplayText.Should().Be($"In this trust ({numberOfAcademies})");
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(3, 0, 2, 5)]
    [InlineData(1, 10, 100, 111)]
    public void GetSubNavLinks_should_show_number_of_pipeline_academies_in_pipeline_subnav_display_text(int numPre,
        int numPost, int numFree, int expectedTotal)
    {
        var academyPipelineSummary = new AcademyPipelineSummaryServiceModel(numPre, numPost, numFree);
        var activePage = GetMockTrustPage(typeof(FreeSchoolMealsModel),
            academyPipelineSummarySummaryServiceModel: academyPipelineSummary);

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().ContainSingle(l => l.TestId == "academies-pipeline-academies-subnav")
            .Which.LinkDisplayText.Should().Be($"Pipeline academies ({expectedTotal})");
    }

    [Fact]
    public void GetSubNavLinks_should_return_expected_links_for_ofsted()
    {
        var activePage = GetMockTrustPage(typeof(SafeguardingAndConcernsModel));

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be("Single headline grades");
                l.AspPage.Should().Be("/Trusts/Ofsted/SingleHeadlineGrades");
                l.TestId.Should().Be("ofsted-single-headline-grades-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Current ratings");
                l.AspPage.Should().Be("/Trusts/Ofsted/CurrentRatings");
                l.TestId.Should().Be("ofsted-current-ratings-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Previous ratings");
                l.AspPage.Should().Be("/Trusts/Ofsted/PreviousRatings");
                l.TestId.Should().Be("ofsted-previous-ratings-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Safeguarding and concerns");
                l.AspPage.Should().Be("/Trusts/Ofsted/SafeguardingAndConcerns");
                l.TestId.Should().Be("ofsted-safeguarding-and-concerns-subnav");
            }
        );
    }

    [Fact]
    public void GetSubNavLinks_should_return_expected_links_for_financial_documents()
    {
        var activePage = GetMockTrustPage(typeof(ManagementLettersModel));

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be("Financial statements");
                l.AspPage.Should().Be("/Trusts/FinancialDocuments/FinancialStatements");
                l.TestId.Should().Be("financial-documents-financial-statements-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Management letters");
                l.AspPage.Should().Be("/Trusts/FinancialDocuments/ManagementLetters");
                l.TestId.Should().Be("financial-documents-management-letters-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Internal scrutiny reports");
                l.AspPage.Should().Be("/Trusts/FinancialDocuments/InternalScrutinyReports");
                l.TestId.Should().Be("financial-documents-internal-scrutiny-reports-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be("Self-assessment checklists");
                l.AspPage.Should().Be("/Trusts/FinancialDocuments/SelfAssessmentChecklists");
                l.TestId.Should().Be("financial-documents-self-assessment-checklists-subnav");
            }
        );
    }

    [Theory]
    [InlineData(11, 12, 13, 14)]
    [InlineData(0, 0, 0, 0)]
    [InlineData(10, 0, 100, 0)]
    public void GetSubNavLinks_should_return_expected_links_for_governance(int numTrustLeadership,
        int numCurrentMembers, int numCurrentTrustees, int numHistoricMembers)
    {
        const string uid = "1234";
        var governance = GetTrustGovernanceServiceModel(uid, numTrustLeadership, numCurrentMembers, numCurrentTrustees,
            numHistoricMembers);
        var activePage = GetMockTrustPage(typeof(HistoricMembersModel), trustGovernanceServiceModel: governance);

        var results = Sut.GetSubNavLinks(activePage);

        results.Should().SatisfyRespectively(
            l =>
            {
                l.LinkDisplayText.Should().Be($"Trust leadership ({numTrustLeadership})");
                l.AspPage.Should().Be("/Trusts/Governance/TrustLeadership");
                l.TestId.Should().Be("governance-trust-leadership-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be($"Trustees ({numCurrentTrustees})");
                l.AspPage.Should().Be("/Trusts/Governance/Trustees");
                l.TestId.Should().Be("governance-trustees-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be($"Members ({numCurrentMembers})");
                l.AspPage.Should().Be("/Trusts/Governance/Members");
                l.TestId.Should().Be("governance-members-subnav");
            },
            l =>
            {
                l.LinkDisplayText.Should().Be($"Historic members ({numHistoricMembers})");
                l.AspPage.Should().Be("/Trusts/Governance/HistoricMembers");
                l.TestId.Should().Be("governance-historic-members-subnav");
            }
        );
    }

    [Fact]
    public void GetSubNavLinks_should_throw_if_page_not_supported()
    {
        var activePage = GetMockTrustPage(typeof(SubNavUnsupportedTrustPageModel));

        var action = () => Sut.GetSubNavLinks(activePage);

        action.Should().Throw<ArgumentOutOfRangeException>()
            .Which.Message.Should().StartWith("Page type is not supported.");
    }

    private class SubNavUnsupportedTrustPageModel(
        IDataSourceService dataSourceService,
        ITrustService trustService) : TrustsAreaModel(dataSourceService, trustService);
}
