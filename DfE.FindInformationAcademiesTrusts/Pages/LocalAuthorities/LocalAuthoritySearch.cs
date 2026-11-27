using DfE.FindInformationAcademiesTrusts.HttpServices;
using GovUK.Dfe.AcademiesApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;

public static class LocalAuthoritySearch
{
    // All-digit input is treated as an LA code, anything else as a name
    public static Task<List<NameAndCodeDto>> Find(this IGetLocalAuthorities getLocalAuthorities, string keywords)
    {
        var term = keywords.Trim();
        var isCode = term.All(char.IsDigit);
        return getLocalAuthorities.SearchLocalAuthorities(isCode ? null : term, isCode ? term : null);
    }
}
