using System.Collections.ObjectModel;
using Dfe.AcademiesApi.Client.Contracts;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;

namespace DfE.FindInformationAcademiesTrusts.HttpServices;

public interface IGetEstablishmentsTemp
{
    Task<List<EstablishmentDto>> SearchEstablishments(string searchQuery);

    Task<EstablishmentDto> GetEstablishment(int urn);
    
    Task<EstablishmentDto[]> GetEstablishmentsByTrustReferenceNumber(string trustReferenceNumber);

    Task<EstablishmentResponse> GetEstablishmentWithSenData(int urn);

    Task<List<EstablishmentDto>> GetEstablishmentsByUrns(List<int> urns);
}
