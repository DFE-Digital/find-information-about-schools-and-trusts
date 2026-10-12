using System.ComponentModel.DataAnnotations;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.Contact;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using DfE.FindInformationAcademiesTrusts.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

public class EditModel(IMediator mediator, ISchoolService schoolService) : BasePageModel, ISchoolAreaModel, IEditContactForm
{
    [BindProperty(SupportsGet = true)] public int Urn { get; set; }

    [BindProperty(SupportsGet = true)] public SchoolContactRole Role { get; set; }

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

    public string CancelPage => "/Schools/Contacts/InDfe";

    public Dictionary<string, string> CancelRouteValues => new() { { "urn", Urn.ToString() } };

    public SchoolSummaryServiceModel SchoolSummary { get; set; } = null!;
    public string SchoolName => SchoolSummary.Name;
    public string SchoolType => SchoolSummary.Type;
    public SchoolCategory SchoolCategory => SchoolSummary.Category;
    public TrustSummaryServiceModel? TrustSummary => null;
    public bool IsPartOfAFederation => false;
    public NavLink[] ServiceNavLinks => [];
    public NavLink[] SubNavLinks => [];
    public List<DataSourcePageListEntry> DataSourcesPerPage => [];

    public PageMetadata PageMetadata =>
        new(SchoolName, ModelState.IsValid, InDfeModel.PageName, $"Edit {RoleName} details");

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await LoadSchoolAsync()) return NotFound();

        var contact = await mediator.Send(new GetSchoolContactQuery(Urn, Role));
        Name = contact?.Name;
        Email = contact?.Email;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await LoadSchoolAsync()) return NotFound();

        if (!ModelState.IsValid) return Page();

        var result = await mediator.Send(new UpdateSchoolContactCommand(Urn, Role, Name, Email));

        ContactUpdatedMessage = result.ToMessage(RoleName);

        return RedirectToPage(CancelPage, new { Urn });
    }

    private async Task<bool> LoadSchoolAsync()
    {
        if (!Enum.IsDefined(Role)) return false;

        var schoolSummary = await schoolService.GetSchoolSummaryAsync(Urn);
        if (schoolSummary is null) return false;

        SchoolSummary = schoolSummary;
        return true;
    }
}
