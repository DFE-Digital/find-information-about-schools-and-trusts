using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;

public class InDfeModel(IMediator mediator, IDataSourceService dataSourceService, ITrustService trustService)
    : TrustsAreaModel(dataSourceService, trustService)
{
    public const string PageName = "Contacts";
    public const string SubPageName = "In DfE";

    public override PageMetadata PageMetadata =>
        base.PageMetadata with { PageName = PageName, SubPageName = SubPageName };

    public TrustContact? TrustRelationshipManager { get; set; }
    public TrustContact? SfsoLead { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var uid = int.Parse(Uid);
        TrustRelationshipManager =
            await mediator.Send(new GetTrustContactQuery(uid, TrustContactRole.TrustRelationshipManager));
        SfsoLead = await mediator.Send(new GetTrustContactQuery(uid, TrustContactRole.SfsoLead));

        DataSourcesPerPage = await ContactsDataSources.GetAsync(DataSourceService, TrustRelationshipManager, SfsoLead);

        return pageResult;
    }
}
