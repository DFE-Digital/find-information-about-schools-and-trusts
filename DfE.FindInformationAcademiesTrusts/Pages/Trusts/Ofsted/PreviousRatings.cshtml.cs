using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;

public class PreviousRatingsModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IOfstedTrustDataExportService ofstedTrustDataExportService,
    IDateTimeProvider dateTimeProvider) : OfstedAreaModel(dataSourceService, trustService,
    academyService, ofstedTrustDataExportService, dateTimeProvider)
{
    public const string SubPageName = "Previous ratings";
    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };
}
