using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;

public class OfstedAreaModel(
    ISchoolService schoolService,
    ISchoolOverviewDetailsService schoolOverviewDetailsService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    ISchoolNavMenu schoolNavMenu)
    : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    protected readonly ISchoolService SchoolService = schoolService;

    public const string PageName = "Ofsted";
    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public DateTime? DateJoinedTrust { get; set; }
    public string OfstedReportUrl { get; set; } = null!;

    public NavLink[] TabList { get; set; } = [];
    public string? PowerBiLink { get; set; } = null;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var schoolOverview = await schoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(Urn, SchoolCategory);
        if (schoolOverview.DateJoinedTrust is not null)
        {
            DateJoinedTrust = schoolOverview.DateJoinedTrust.Value.ToDateTime(new TimeOnly());
        }

        OfstedReportUrl = otherServicesLinkBuilder.OfstedReportLinkForSchool(Urn);

        var giasDataSource = await dataSourceService.GetAsync(Source.Gias);
        var misDataSource = await dataSourceService.GetAsync(Source.Mis);

        DataSourcesPerPage =
        [
            new DataSourcePageListEntry(OfstedOverviewModel.SubPageName, [
                new DataSourceListEntry(giasDataSource, "Date joined trust"),
                new DataSourceListEntry(misDataSource, "All inspection types"),
                new DataSourceListEntry(misDataSource, "All inspection dates")
            ]),
            new DataSourcePageListEntry(CurrentReportCardsModel.SubPageName, [
                new DataSourceListEntry(misDataSource, "Current report card ratings"),
                new DataSourceListEntry(misDataSource, "Previous report card ratings")
            ]),
            new DataSourcePageListEntry(PreviousRatingsModel.SubPageName, [
                new DataSourceListEntry(misDataSource, "Inspection ratings after September 24"),
                new DataSourceListEntry(misDataSource, "Inspection ratings before September 24")
            ])
        ];

        return Page();
    }
}
