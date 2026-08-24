using System.Collections.ObjectModel;
using Dfe.AcademiesApi.Client.Contracts;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;

public interface IGetEstablishments
{
    Task<List<EstablishmentDto>> SearchEstablishments(string searchQuery);

    Task<EstablishmentDto> GetEstablishment(int urn);

    Task<EstablishmentResponse> GetEstablishmentWithSenData(int urn);
}
