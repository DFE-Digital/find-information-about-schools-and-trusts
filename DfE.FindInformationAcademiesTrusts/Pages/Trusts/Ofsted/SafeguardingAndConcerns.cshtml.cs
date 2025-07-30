using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;

public class SafeguardingAndConcernsModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IOfstedTrustDataExportService ofstedTrustDataExportService,
    IDateTimeProvider dateTimeProvider) : OfstedAreaModel(dataSourceService, trustService,
    academyService, ofstedTrustDataExportService, dateTimeProvider)
{
    public const string SubPageName = "Safeguarding and concerns";

    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };
}
