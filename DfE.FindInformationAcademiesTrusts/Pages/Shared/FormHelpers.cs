using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfE.FindInformationAcademiesTrusts.Pages.Shared;

public static class FormHelpers
{
    public static string GetErrorClass(this ModelStateDictionary modelState, string key)
    {
        return modelState.ContainsKey(key) && modelState[key]!.Errors.Any() ? "govuk-form-group--error" : string.Empty;
    }

    public static string GenerateErrorAriaDescribedBy(this ModelStateDictionary modelState, string key)
    {
        return string.Join(" ", modelState.GetErrorList(key).Select(value => $"error-{key}-{value.index}"));
    }

    public static (int index, string errorMessage)[] GetErrorList(this ModelStateDictionary modelState, string key)
    {
        if (modelState.ContainsKey(key))
        {
            return modelState[key]!.Errors.Select((error, index) => (index, error.ErrorMessage)).ToArray();
        }

        return [];
    }
}
