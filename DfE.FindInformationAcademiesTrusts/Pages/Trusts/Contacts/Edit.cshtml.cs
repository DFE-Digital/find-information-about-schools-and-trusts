using System.ComponentModel.DataAnnotations;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.Contact;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using DfE.FindInformationAcademiesTrusts.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;

public class EditModel(IMediator mediator, IDataSourceService dataSourceService, ITrustService trustService)
    : TrustsAreaModel(dataSourceService, trustService), IEditContactForm
{
    [BindProperty(SupportsGet = true)] public TrustContactRole Role { get; set; }

    [BindProperty]
    [BindRequired]
    [MaxLength(500)]
    public string? Name { get; set; }

    [BindProperty]
    [BindRequired]
    [DfeEmailAddress]
    [MaxLength(320)]
    public string? Email { get; set; }

    public string RoleName => Role.MapRoleToViewString();

    [TempData] public string ContactUpdatedMessage { get; set; } = string.Empty;

    public string CancelPage => "/Trusts/Contacts/InDfe";

    public Dictionary<string, string> CancelRouteValues =>
        new() { { "uid", Uid }, { "referencenumber", ReferenceNumber } };

    public override PageMetadata PageMetadata =>
        base.PageMetadata with { PageName = InDfeModel.PageName, SubPageName = $"Edit {RoleName} details" };

    public override async Task<IActionResult> OnGetAsync()
    {
        if (!Enum.IsDefined(Role)) return NotFound();

        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var contact = await mediator.Send(new GetTrustContactQuery(int.Parse(Uid), Role));
        Name = contact?.Name;
        Email = contact?.Email;

        return pageResult;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Enum.IsDefined(Role)) return NotFound();

        if (!ModelState.IsValid)
        {
            return await base.OnGetAsync();
        }

        var result = await mediator.Send(new UpdateTrustContactCommand(int.Parse(Uid), Role, Name, Email));

        ContactUpdatedMessage = result.ToMessage(RoleName);

        return RedirectToPage(CancelPage, new { Uid, ReferenceNumber });
    }
}
