using System.Diagnostics.CodeAnalysis;
using Dfe.CaseAggregationService.Api.Client.Extensions;
using Dfe.CaseAggregationService.Client.Contracts;
using Dfe.CaseAggregationService.Client;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Hardcoded;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.DataSource;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PipelineAcademy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.TrustDocument;
using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Pages.Schools;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.FinancialDocument;
using DfE.FindInformationAcademiesTrusts.Services.ManageProjectsAndCases;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Search;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Setup;

[ExcludeFromCodeCoverage]
public static class Dependencies
{
    public static void AddDependenciesTo(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AcademiesDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("AcademiesDb") ??
                throw new InvalidOperationException("Connection string 'AcademiesDb' not found."),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        2, // retry up to a maximum of 2 times
                        TimeSpan.FromSeconds(5), // wait up to 5s for the server to respond before retry
                        null
                    );
                }
            ).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );

        builder.Services.AddScoped<IAcademiesDbContext>(provider =>
            provider.GetService<AcademiesDbContext>() ??
            throw new InvalidOperationException("AcademiesDbContext not registered"));

        builder.Services.AddDbContext<FiatDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("FIAT database connection string 'DefaultConnection' not found."),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        2, // retry up to a maximum of 2 times
                        TimeSpan.FromSeconds(5), // wait up to 5s for the server to respond before retry
                        null
                    );
                }
            )
        );

        builder.Services.AddScoped<SetChangedByInterceptor>();
        builder.Services.AddScoped<IUserDetailsProvider, HttpContextUserDetailsProvider>();

        builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        builder.Services.AddScoped<IStringFormattingUtilities, StringFormattingUtilities>();

        builder.Services.AddScoped<IAcademyRepository, AcademyRepository>();
        builder.Services.AddScoped<IOfstedRepository, OfstedRepository>();
        builder.Services.AddScoped<ITrustRepository, TrustRepository>();
        builder.Services.AddScoped<IDataSourceRepository, DataSourceRepository>();
        builder.Services.AddScoped<IFiatDataSourceRepository, FiatDataSourceRepository>();
        builder.Services.AddScoped<IContactRepository, ContactRepository>();
        builder.Services.AddScoped<IPipelineEstablishmentRepository, PipelineEstablishmentRepository>();
        builder.Services.AddScoped<ITrustDocumentRepository, TrustDocumentRepository>();
        builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();

        builder.Services.AddScoped<IDataSourceService, DataSourceService>();
        builder.Services.AddScoped<ITrustService, TrustService>();
        builder.Services.AddScoped<IAcademyService, AcademyService>();
        builder.Services.AddScoped<IFinancialDocumentService, FinancialDocumentService>();
        builder.Services.AddScoped<ISchoolService, SchoolService>();

        builder.Services.AddScoped<IPipelineAcademiesExportService, PipelineAcademiesExportService>();
        builder.Services.AddScoped<IOfstedTrustDataExportService, OfstedTrustDataExportService>();
        builder.Services.AddScoped<IOfstedSchoolDataExportService, OfstedSchoolDataExportService>();
        builder.Services.AddScoped<IAcademiesExportService, AcademiesExportService>();

        builder.Services.AddScoped<IOtherServicesLinkBuilder, OtherServicesLinkBuilder>();
        builder.Services.AddScoped<IFreeSchoolMealsAverageProvider, FreeSchoolMealsAverageProvider>();

        builder.Services.AddScoped<IGetCasesService, GetCasesService>();
        builder.Services.AddCaseAggregationApiClient<ICasesClient, CasesClient>(builder.Configuration);

        builder.Services.AddScoped<ISchoolOverviewDetailsService, SchoolOverviewDetailsService>();
        builder.Services.AddScoped<ISchoolContactsService, SchoolContactsService>();

        builder.Services.AddScoped<ISchoolOverviewSenService, SchoolOverviewSenService>();
        builder.Services.AddScoped<ISchoolOverviewFederationService, SchoolOverviewFederationService>();
        builder.Services.AddScoped<ISearchService, SearchService>();
        builder.Services.AddScoped<ITrustSchoolSearchRepository, TrustSchoolSearchRepository>();

        builder.Services.AddScoped<ISchoolNavMenu, SchoolNavMenu>();

        builder.Services.AddHttpContextAccessor();
    }
}
