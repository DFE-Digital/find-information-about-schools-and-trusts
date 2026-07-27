using System.Collections.ObjectModel;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;

public interface IGetEstablishments
{
    Task<List<EstablishmentDto>> SearchEstablishments(string searchQuery);
    
    Task<EstablishmentDto> GetEstablishment(int urn);
}
