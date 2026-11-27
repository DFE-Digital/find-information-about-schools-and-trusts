using GovUK.Dfe.AcademiesApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.HttpServices;


public interface IGetLocalAuthorities
{
    Task<List<NameAndCodeDto>> SearchLocalAuthorities(string? name, string? code);
}


