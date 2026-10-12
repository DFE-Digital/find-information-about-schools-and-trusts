using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
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

        var giasDataSource = await DataSourceService.GetAsync(Source.Gias);
        var mstrDataSource = await DataSourceService.GetAsync(Source.Mstr);

        DataSourcesPerPage =
        [
            new DataSourcePageListEntry(SubPageName, [
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb, TrustRelationshipManager?.LastModifiedAtTime, null,
                        TrustRelationshipManager?.LastModifiedByEmail),
                    TrustContactRole.TrustRelationshipManager.MapRoleToViewString()),
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb, SfsoLead?.LastModifiedAtTime, null,
                        SfsoLead?.LastModifiedByEmail),
                    TrustContactRole.SfsoLead.MapRoleToViewString())
            ]),
            new DataSourcePageListEntry(InTrustModel.SubPageName, [
                new DataSourceListEntry(giasDataSource, "Accounting officer name"),
                new DataSourceListEntry(giasDataSource, "Chief financial officer name"),
                new DataSourceListEntry(giasDataSource, "Chair of trustees name"),
                new DataSourceListEntry(mstrDataSource, "Accounting officer email"),
                new DataSourceListEntry(mstrDataSource, "Chief financial officer email"),
                new DataSourceListEntry(mstrDataSource, "Chair of trustees email")
            ])
        ];

        return pageResult;
    }
}
