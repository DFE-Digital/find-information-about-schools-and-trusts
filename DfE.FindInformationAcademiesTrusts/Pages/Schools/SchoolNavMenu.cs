using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using Microsoft.FeatureManagement;

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
            GetServiceNavLinkTo<ContactsAreaModel>(ContactsAreaModel.PageName, contactLink, activePage)
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
