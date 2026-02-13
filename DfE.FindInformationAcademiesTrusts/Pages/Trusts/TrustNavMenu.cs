using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Schools;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.Pipeline;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.FinancialDocuments;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Overview;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts;

public static class TrustNavMenu
{
    public static NavLink[] GetServiceNavLinks(ITrustsAreaModel activePage)
    {
        return
        [
            GetServiceNavLinkTo<OverviewAreaModel>(OverviewAreaModel.PageName, "/Trusts/Overview/TrustDetails",
                activePage),
            GetServiceNavLinkTo<ContactsAreaModel>(ContactsAreaModel.PageName, "/Trusts/Contacts/InDfe", activePage),
            GetServiceNavLinkTo<AcademiesAreaModel>(
                $"{AcademiesAreaModel.PageName} ({activePage.TrustSummary.NumberOfAcademies})",
                "/Trusts/Academies/InTrust/Details", activePage),
            GetServiceNavLinkTo<OfstedAreaModel>(OfstedAreaModel.PageName, "/Trusts/Ofsted/Overview",
                activePage),
            GetServiceNavLinkTo<FinancialDocumentsAreaModel>(FinancialDocumentsAreaModel.PageName,
                "/Trusts/FinancialDocuments/FinancialStatements", activePage),
            GetServiceNavLinkTo<GovernanceAreaModel>(GovernanceAreaModel.PageName, "/Trusts/Governance/TrustLeadership",
                activePage)
        ];
    }

    private static NavLink GetServiceNavLinkTo<T>(string linkDisplayText, string aspPage, ITrustsAreaModel activePage)
    {
        return new NavLink(activePage is T,
            "Trust",
            linkDisplayText,
            aspPage,
            $"{linkDisplayText}-nav".Kebabify(),
            new Dictionary<string, string> { { "uid", activePage.Uid } });
    }

    public static NavLink[] GetTabLinksForReportCardsOfstedPage(ITrustsAreaModel activePage)
    {
        return
        [
            GetSubNavLinkTo<CurrentReportCardsModel>("Report cards", "Current report card", "./currentreportcards", activePage),
            GetSubNavLinkTo<PreviousReportCardsModel>("Report cards", "Previous report card", "./previousreportcards", activePage)
        ];
    }

    public static NavLink[] GetSubNavLinks(ITrustsAreaModel activePage)
    {
        return activePage switch
        {
            OverviewAreaModel =>
            [
                GetSubNavLinkTo<TrustDetailsModel>(OverviewAreaModel.PageName, TrustDetailsModel.SubPageName,
                    "/Trusts/Overview/TrustDetails", activePage),
                GetSubNavLinkTo<TrustSummaryModel>(OverviewAreaModel.PageName, TrustSummaryModel.SubPageName,
                    "/Trusts/Overview/TrustSummary", activePage),
                GetSubNavLinkTo<ReferenceNumbersModel>(OverviewAreaModel.PageName, ReferenceNumbersModel.SubPageName,
                    "/Trusts/Overview/ReferenceNumbers", activePage)
            ],
            ContactsAreaModel =>
            [
                GetSubNavLinkTo<InDfeModel>(ContactsAreaModel.PageName, InDfeModel.SubPageName,
                    "/Trusts/Contacts/InDfE", activePage),
                GetSubNavLinkTo<InTrustModel>(ContactsAreaModel.PageName, InTrustModel.SubPageName,
                    "/Trusts/Contacts/InTrust", activePage)
            ],
            AcademiesAreaModel academiesAreaModel =>
            [
                GetSubNavLinkTo<AcademiesInTrustAreaModel>(AcademiesAreaModel.PageName,
                    $"{AcademiesInTrustAreaModel.SubPageName} ({academiesAreaModel.TrustSummary.NumberOfAcademies})",
                    "/Trusts/Academies/InTrust/Details", activePage),
                GetSubNavLinkTo<PipelineAcademiesAreaModel>(AcademiesAreaModel.PageName,
                    $"{PipelineAcademiesAreaModel.SubPageName} ({academiesAreaModel.PipelineSummary.Total})",
                    "/Trusts/Academies/Pipeline/PreAdvisoryBoard", activePage)
            ],
            OfstedAreaModel =>
            [
                GetSubNavLinkTo<OverviewModel>(OfstedAreaModel.PageName,
                    OverviewModel.SubPageName, "/Trusts/Ofsted/Overview", activePage),
                GetSubNavLinkTo<BaseReportCardsRatingsModel>(OfstedAreaModel.PageName, CurrentReportCardsModel.SubPageName,
                    "/Trusts/Ofsted/ReportCards/CurrentReportCards", activePage),
                GetSubNavLinkTo<OlderInspectionsModel>(OfstedAreaModel.PageName, OlderInspectionsModel.SubPageName,
                    "/Trusts/Ofsted/OlderInspections", activePage),
                GetSubNavLinkTo<SafeguardingAndConcernsModel>(OfstedAreaModel.PageName, SafeguardingAndConcernsModel.SubPageName,
                    "/Trusts/Ofsted/SafeguardingAndConcerns", activePage)
            ],
            FinancialDocumentsAreaModel =>
            [
                GetSubNavLinkTo<FinancialStatementsModel>(FinancialDocumentsAreaModel.PageName,
                    FinancialStatementsModel.SubPageName, "/Trusts/FinancialDocuments/FinancialStatements", activePage),
                GetSubNavLinkTo<ManagementLettersModel>(FinancialDocumentsAreaModel.PageName,
                    ManagementLettersModel.SubPageName, "/Trusts/FinancialDocuments/ManagementLetters", activePage),
                GetSubNavLinkTo<InternalScrutinyReportsModel>(FinancialDocumentsAreaModel.PageName,
                    InternalScrutinyReportsModel.SubPageName, "/Trusts/FinancialDocuments/InternalScrutinyReports",
                    activePage),
                GetSubNavLinkTo<SelfAssessmentChecklistsModel>(FinancialDocumentsAreaModel.PageName,
                    SelfAssessmentChecklistsModel.SubPageName, "/Trusts/FinancialDocuments/SelfAssessmentChecklists",
                    activePage)
            ],
            GovernanceAreaModel governanceAreaModel =>
            [
                GetSubNavLinkTo<TrustLeadershipModel>(GovernanceAreaModel.PageName,
                    $"{TrustLeadershipModel.SubPageName} ({governanceAreaModel.TrustGovernance.CurrentTrustLeadership.Length})",
                    "/Trusts/Governance/TrustLeadership", activePage),
                GetSubNavLinkTo<TrusteesModel>(GovernanceAreaModel.PageName,
                    $"{TrusteesModel.SubPageName} ({governanceAreaModel.TrustGovernance.CurrentTrustees.Length})",
                    "/Trusts/Governance/Trustees", activePage),
                GetSubNavLinkTo<MembersModel>(GovernanceAreaModel.PageName,
                    $"{MembersModel.SubPageName} ({governanceAreaModel.TrustGovernance.CurrentMembers.Length})",
                    "/Trusts/Governance/Members", activePage),
                GetSubNavLinkTo<HistoricMembersModel>(GovernanceAreaModel.PageName,
                    $"{HistoricMembersModel.SubPageName} ({governanceAreaModel.TrustGovernance.HistoricMembers.Length})",
                    "/Trusts/Governance/HistoricMembers", activePage)
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(activePage), activePage, "Page type is not supported.")
        };
    }

    private static NavLink GetSubNavLinkTo<T>(string serviceName, string linkDisplayText, string aspPage,
        ITrustsAreaModel activePage)
    {
        return new NavLink(
            activePage is T,
            serviceName,
            linkDisplayText,
            aspPage,
            $"{serviceName}-{linkDisplayText}-subnav".Kebabify(),
            new Dictionary<string, string> { { "uid", activePage.Uid } }
        );
    }
}
