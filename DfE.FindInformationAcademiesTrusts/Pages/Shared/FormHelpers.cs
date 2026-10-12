namespace DfE.FindInformationAcademiesTrusts.Pages.Shared;

public static class FormHelpers
{
    public static string GetErrorClass(this BasePageModel model, string key)
    {
        return model.ModelState.ContainsKey(key) && model.ModelState[key]!.Errors.Any() ? "govuk-form-group--error" : string.Empty;
    }

    public static string GenerateErrorAriaDescribedBy(this BasePageModel model, string key)
    {
        return string.Join(" ", Enumerable.Select<(int index, string errorMessage), string>(model.GetErrorList(key), value => $"error-{key}-{value.index}"));
    }

    public static (int index, string errorMessage)[] GetErrorList(this BasePageModel model, string key)
    {
        if (model.ModelState.ContainsKey(key))
        {
            return model.ModelState[key]!.Errors.Select((error, index) => (index, error.ErrorMessage)).ToArray();
        }

        return [];
    }
}
