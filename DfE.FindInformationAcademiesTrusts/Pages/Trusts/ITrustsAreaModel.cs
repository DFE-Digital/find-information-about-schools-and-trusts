using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts;

public interface ITrustsAreaModel
{
    string Uid { get; }

    string ReferenceNumber { get; }
    
    TrustSummaryServiceModel TrustSummary { get; }

    List<DataSourcePageListEntry> DataSourcesPerPage { get; }

    PageMetadata PageMetadata { get; }
}
