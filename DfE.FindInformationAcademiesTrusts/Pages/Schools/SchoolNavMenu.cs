using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using Microsoft.FeatureManagement;
using GovernanceAreaModel = DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance.GovernanceAreaModel;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools;

public interface ISchoolNavMenu
{
    Task<NavLink[]> GetServiceNavLinksAsync(ISchoolAreaModel activePage);
    Task<NavLink[]> GetSubNavLinksAsync(ISchoolAreaModel activePage);
}

public class SchoolNavMenu(IVariantFeatureManager featureManager) : ISchoolNavMenu
{
    public async Task<NavLink[]> GetServiceNavLinksAsync(ISchoolAreaModel activePage)
    {
        var contactLink = await ContactsInDfeForSchoolsEnabled()
            ? "/Schools/Contacts/InDfe"
            : "/Schools/Contacts/InSchool";

        return
        [
            GetServiceNavLinkTo<OverviewAreaModel>(OverviewAreaModel.PageName, "/Schools/Overview/Details",
                activePage),
            GetServiceNavLinkTo<ContactsAreaModel>(ContactsAreaModel.PageName, contactLink, activePage),
            GetServiceNavLinkTo<GovernanceAreaModel>(GovernanceAreaModel.PageName, "/Schools/Governance/Current",
                activePage),
            GetServiceNavLinkTo<OfstedAreaModel>(OfstedAreaModel.PageName, "/Schools/Ofsted/SingleHeadlineGrades",
                activePage)
        ];
    }

    private static NavLink GetServiceNavLinkTo<T>(string linkDisplayText, string aspPage, ISchoolAreaModel activePage)
    {
        return new NavLink(activePage is T,
            activePage.SchoolCategory.ToDisplayString(),
            linkDisplayText,
            aspPage,
            $"{linkDisplayText}-nav".Kebabify(),
            new Dictionary<string, string> { { "urn", activePage.Urn.ToString() } });
    }

    public async Task<NavLink[]> GetSubNavLinksAsync(ISchoolAreaModel activePage)
    {
        return activePage switch
        {
            OverviewAreaModel => BuildLinksForOverviewPage(activePage),
            ContactsAreaModel => await BuildLinksForContactsPageAsync(activePage),
            GovernanceAreaModel governanceAreaModel => BuildLinksForGovernancePage(governanceAreaModel),
            OfstedAreaModel => BuildLinksForOfstedPage(activePage),
            _ => throw new ArgumentOutOfRangeException(nameof(activePage), activePage, "Page type is not supported.")
        };
    }

    private static NavLink[] BuildLinksForOverviewPage(ISchoolAreaModel activePage)
    {
        var links = new List<NavLink>
        {
            GetSubNavLinkTo<DetailsModel>(
                OverviewAreaModel.PageName,
                DetailsModel.SubPageName(activePage.SchoolCategory),
                "/Schools/Overview/Details",
                activePage,
                "overview-details-subnav"
            )
        };

        if (activePage.IsPartOfAFederation)
        {
            links.Add(GetSubNavLinkTo<FederationModel>(
                OverviewAreaModel.PageName,
                FederationModel.SubPageName,
                "/Schools/Overview/Federation",
                activePage,
                activePage.SchoolCategory,
                "overview-federation-subnav"
            ));
        }

        links.Add(GetSubNavLinkTo<ReferenceNumbersModel>(
            OverviewAreaModel.PageName,
            ReferenceNumbersModel.SubPageName,
            "/Schools/Overview/ReferenceNumbers",
            activePage,
            "overview-reference-numbers-subnav"
        ));

        links.Add(GetSubNavLinkTo<SenModel>(
            OverviewAreaModel.PageName,
            SenModel.SubPageName,
            "/Schools/Overview/Sen",
            activePage,
            "overview-sen-subnav"
        ));

        links.Add(GetSubNavLinkTo<ReligiousCharacteristicsModel>(
            OverviewAreaModel.PageName,
            ReligiousCharacteristicsModel.SubPageName,
            "/Schools/Overview/ReligiousCharacteristics",
            activePage,
            "overview-religious-characteristics-subnav"
        ));


        return links.ToArray();
    }

    private async Task<NavLink[]> BuildLinksForContactsPageAsync(ISchoolAreaModel activePage)
    {
        var inSchoolNavLink = GetSubNavLinkTo<InSchoolModel>(ContactsAreaModel.PageName,
            InSchoolModel.SubPageName(activePage.SchoolCategory), "/Schools/Contacts/InSchool", activePage,
            "contacts-in-this-school-subnav");
        return await ContactsInDfeForSchoolsEnabled()
            ?
            [
                GetSubNavLinkTo<InDfeModel>(ContactsAreaModel.PageName, InDfeModel.SubPageName,
                    "/Schools/Contacts/InDfe", activePage, "contacts-in-dfe-subnav"),
                inSchoolNavLink
            ]
            : [inSchoolNavLink];
    }

    private static NavLink[] BuildLinksForGovernancePage(GovernanceAreaModel activePage)
    {
        var currentNavLink = GetSubNavLinkTo<CurrentModel>(GovernanceAreaModel.PageName,
            CurrentModel.NavTitle(activePage.SchoolGovernance.Current.Length),
            "/Schools/Governance/Current", activePage,
            "current-governors-subnav");

        var historicNavLink = GetSubNavLinkTo<HistoricModel>(GovernanceAreaModel.PageName,
            HistoricModel.NavTitle(activePage.SchoolGovernance.Historic.Length),
            "/Schools/Governance/Historic", activePage,
            "historic-governors-subnav");

        return [currentNavLink, historicNavLink];
    }

    private static NavLink[] BuildLinksForOfstedPage(ISchoolAreaModel activePage)
    {
        return
        [
            GetSubNavLinkTo<SingleHeadlineGradesModel>(OfstedAreaModel.PageName, SingleHeadlineGradesModel.SubPageName,
                "/Schools/Ofsted/SingleHeadlineGrades", activePage, "ofsted-single-headline-grades-subnav"),
            GetSubNavLinkTo<CurrentRatingsModel>(OfstedAreaModel.PageName, CurrentRatingsModel.SubPageName,
                "/Schools/Ofsted/CurrentRatings", activePage, "ofsted-current-ratings-subnav"),
            GetSubNavLinkTo<PreviousRatingsModel>(OfstedAreaModel.PageName, PreviousRatingsModel.SubPageName,
                "/Schools/Ofsted/PreviousRatings", activePage, "ofsted-previous-ratings-subnav"),
            GetSubNavLinkTo<SafeguardingAndConcernsModel>(OfstedAreaModel.PageName,
                SafeguardingAndConcernsModel.SubPageName,
                "/Schools/Ofsted/SafeguardingAndConcerns", activePage, "ofsted-safeguarding-and-concerns-subnav")
        ];
    }

    private static NavLink GetSubNavLinkTo<T>(string serviceName, string linkDisplayText, string aspPage,
        ISchoolAreaModel activePage, string? testIdOverride = null)
    {
        return new NavLink(
            activePage is T,
            serviceName,
            linkDisplayText,
            aspPage,
            testIdOverride ?? $"{serviceName}-{linkDisplayText}-subnav".Kebabify(),
            new Dictionary<string, string> { { "urn", activePage.Urn.ToString() } }
        );
    }

    private static NavLink GetSubNavLinkTo<T>(string serviceName, string linkDisplayText, string aspPage,
        ISchoolAreaModel activePage, SchoolCategory? schoolCategory, string? testIdOverride = null)
    {
        return new NavLink(
            activePage is T,
            serviceName,
            linkDisplayText,
            aspPage,
            testIdOverride ?? $"{serviceName}-{linkDisplayText}-subnav".Kebabify(),
            new Dictionary<string, string> { { "urn", activePage.Urn.ToString() } },
            schoolCategory
        );
    }

    private async Task<bool> ContactsInDfeForSchoolsEnabled()
    {
        return await featureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools);
    }
}
