using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace DfE.FindInformationAcademiesTrusts.Pages.ManageProjectsAndCases;

public class ProjectListFilters
{
    public const string FilterProjectTypes = nameof(FilterProjectTypes);
    public const string FilterSystems = nameof(FilterSystems);

    private IDictionary<string, object?> _filterStore = new Dictionary<string, object?>();

    public List<string> AvailableProjectTypes { get; set; } = [];
    public List<string> AvailableSystems { get; set; } = [];

    [BindProperty] public string[] SelectedProjectTypes { get; set; } = [];

    [BindProperty] public string[] SelectedSystems { get; set; } = [];

    public bool IsVisible => SelectedProjectTypes.Length > 0 || SelectedSystems.Length > 0;

    public ProjectListFilters PersistUsing(IDictionary<string, object?> filterStore)
    {
        _filterStore = filterStore;
        SelectedProjectTypes = GetFilters(FilterProjectTypes);
        SelectedSystems = GetFilters(FilterSystems);

        return this;
    }

    public void PopulateFrom(IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
    {
        var query = new Dictionary<string, StringValues>(requestQuery, StringComparer.OrdinalIgnoreCase);

        if (query.ContainsKey("clear"))
        {
            ClearFilters();
            return;
        }

        if (query.ContainsKey("remove"))
        {
            SelectedProjectTypes =
                RemoveValuesFromFilters(FilterProjectTypes, ExtractQueryItems(nameof(SelectedProjectTypes)));
            SelectedSystems = RemoveValuesFromFilters(FilterSystems, ExtractQueryItems(nameof(SelectedSystems)));
            return;
        }

        if (query.ContainsKey(nameof(SelectedProjectTypes)) || query.ContainsKey(nameof(SelectedSystems)))
        {
            SelectedProjectTypes =
                UpdateAndGetStore(FilterProjectTypes, ExtractQueryItems(nameof(SelectedProjectTypes)));
            SelectedSystems = UpdateAndGetStore(FilterSystems, ExtractQueryItems(nameof(SelectedSystems)));
        }
        else
        {
            ClearFilters();
        }

        string[] ExtractQueryItems(string key)
        {
            return query.TryGetValue(key, out var value) ? value! : Array.Empty<string>();
        }
    }

    private string[] GetFilters(string filterType)
    {
        return _filterStore.TryGetValue(filterType, out var filters) && filters is string[] values
            ? values
            : [];
    }

    private string[] RemoveValuesFromFilters(string filterType, string[] valuesToRemove)
    {
        var currentFilters = GetFilters(filterType);

        if (valuesToRemove is { Length: > 0 })
        {
            currentFilters = currentFilters.Where(x => !valuesToRemove.Contains(x)).ToArray();
        }

        UpdateAndGetStore(filterType, currentFilters);

        return currentFilters;
    }

    private string[] UpdateAndGetStore(string key, string[]? value)
    {
        if (value is null || value.Length == 0)
        {
            _filterStore.Remove(key);
            return [];
        }

        _filterStore[key] = value;

        return value;
    }


    private void ClearFilters()
    {
        UpdateAndGetStore(FilterProjectTypes, null);
        UpdateAndGetStore(FilterSystems, null);

        SelectedProjectTypes = [];
        SelectedSystems = [];
    }
}
