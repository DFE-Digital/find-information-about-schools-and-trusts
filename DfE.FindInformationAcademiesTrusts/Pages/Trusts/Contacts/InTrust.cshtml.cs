using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;

public class InTrustModel(IMediator mediator, IDataSourceService dataSourceService, ITrustService trustService)
    : TrustsAreaModel(dataSourceService, trustService)
{
    public const string SubPageName = "In this trust";

    public override PageMetadata PageMetadata =>
        base.PageMetadata with { PageName = InDfeModel.PageName, SubPageName = SubPageName };

    public Person? AccountingOfficer { get; set; }
    public Person? ChairOfTrustees { get; set; }
    public Person? ChiefFinancialOfficer { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        (AccountingOfficer, ChairOfTrustees, ChiefFinancialOfficer) =
            await TrustService.GetTrustContactsAsync(Uid, ReferenceNumber);

        var uid = int.Parse(Uid);
        var trustRelationshipManager =
            await mediator.Send(new GetTrustContactQuery(uid, TrustContactRole.TrustRelationshipManager));
        var sfsoLead = await mediator.Send(new GetTrustContactQuery(uid, TrustContactRole.SfsoLead));

        var giasDataSource = await DataSourceService.GetAsync(Source.Gias);
        var mstrDataSource = await DataSourceService.GetAsync(Source.Mstr);

        DataSourcesPerPage =
        [
            new DataSourcePageListEntry(InDfeModel.SubPageName, [
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb, trustRelationshipManager?.LastModifiedAtTime, null,
                        trustRelationshipManager?.LastModifiedByEmail),
                    TrustContactRole.TrustRelationshipManager.MapRoleToViewString()),
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb, sfsoLead?.LastModifiedAtTime, null,
                        sfsoLead?.LastModifiedByEmail),
                    TrustContactRole.SfsoLead.MapRoleToViewString())
            ]),
            new DataSourcePageListEntry(SubPageName, [
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
