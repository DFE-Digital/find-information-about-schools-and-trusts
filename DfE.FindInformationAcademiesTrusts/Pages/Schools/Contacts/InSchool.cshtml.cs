using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

public class InSchoolModel(
    IMediator mediator,
    ISchoolRepository schoolRepository,
    IDataSourceService dataSourceService,
    IVariantFeatureManager featureManager,
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolNavMenu schoolNavMenu)
    : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    public override PageMetadata PageMetadata => base.PageMetadata with
    {
        PageName = InDfeModel.PageName,
        SubPageName = FullSubPageName(SchoolCategory)
    };

    public static string FullSubPageName(SchoolCategory schoolCategory)
    {
        return schoolCategory switch
        {
            SchoolCategory.LaMaintainedSchool => "Contacts in this school",
            SchoolCategory.Academy => "Contacts in this academy",
            _ => throw new ArgumentOutOfRangeException(nameof(schoolCategory))
        };
    }

    public static string SubPageName(SchoolCategory schoolCategory)
    {
        return schoolCategory switch
        {
            SchoolCategory.LaMaintainedSchool => "In this school",
            SchoolCategory.Academy => "In this academy",
            _ => throw new ArgumentOutOfRangeException(nameof(schoolCategory))
        };
    }

    public SchoolContact HeadTeacher { get; private set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var headTeacher = await schoolRepository.GetSchoolContactsAsync(Urn);
        if (headTeacher is null) return NotFound();

        HeadTeacher = headTeacher;

        var trustUid = int.TryParse(TrustSummary?.Uid, out var uid) ? uid : (int?)null;
        var dfeContacts = await mediator.Send(new GetSchoolDfeContactsQuery(Urn, trustUid));
        DataSourcesPerPage =
            await ContactsDataSources.GetAsync(dataSourceService, featureManager, SchoolCategory, dfeContacts);

        return pageResult;
    }
}
