using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace DfE.FindInformationAcademiesTrusts.Pages.ManageProjectsAndCases
{
    public class ProjectListFilters
    {
        #nullable enable
        public const string FilterProjectTypes = nameof(FilterProjectTypes);
        public const string FilterSystems = nameof(FilterSystems);

        private IDictionary<string, object?> _store = null!;
        public List<string> AvailableProjectTypes { get; set; } = [];

        public List<string> AvailableSystems { get; set; } = [];
        
        [BindProperty]
        public string[] SelectedProjectTypes { get; set; } = [];

        [BindProperty]
        public string[] SelectedSystems { get; set; } = [];

        public bool IsVisible => SelectedProjectTypes.Length > 0 ||
                                 SelectedSystems.Length > 0;
        
        public ProjectListFilters PersistUsing(IDictionary<string, object?> store)
        {
            _store = store;

            SelectedProjectTypes = Get(FilterProjectTypes);
            SelectedSystems = Get(FilterSystems);
        
            return this;
        }

        public void PopulateFrom(IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
        {
            Dictionary<string, StringValues>? query = new(requestQuery, StringComparer.OrdinalIgnoreCase);

            if (query.ContainsKey("clear"))
            {
                ClearFilters();
                return;
            }

            if (query.ContainsKey("remove"))
            {
                SelectedProjectTypes = GetAndRemove(FilterProjectTypes, GetFromQuery(nameof(SelectedProjectTypes)));
                SelectedSystems = GetAndRemove(FilterSystems, GetFromQuery(nameof(SelectedSystems)));

                return;
            }

            bool activeFilterChanges = query.ContainsKey(nameof(SelectedProjectTypes)) ||
                                       query.ContainsKey(nameof(SelectedSystems));

            if (activeFilterChanges)
            {
                SelectedProjectTypes = Cache(FilterProjectTypes, GetFromQuery(nameof(SelectedProjectTypes)));
                SelectedSystems = Cache(FilterSystems, GetFromQuery(nameof(SelectedSystems)));
            }
            else
            {
                ClearFilters();
            }

            return;
                    
            string[] GetFromQuery(string key)
            {
                return query.TryGetValue(key, out var value) ? value! : Array.Empty<string>();
            }
        }

        private string[] Get(string key)
        {
            if (!_store.TryGetValue(key, out var strValue)) return [];

            string[]? value = (string[]?)strValue;

            return value!;
        }

        private string[] GetAndRemove(string key, string[]? value)
        {
            if (!_store.TryGetValue(key, out var strValue)) return [];

            string[]? currentValues = (string[]?)strValue;

            if (value is not null && value.Length > 0 && currentValues is not null)
            {
                currentValues = currentValues!.Where(x => !value!.Contains(x)).ToArray();
            }

            Cache(key, currentValues);

            return currentValues ?? [];
        }

        private string[] Cache(string key, string[]? value)
        {
            if (value is null || value.Length == 0)
                _store.Remove(key);
            else
                _store[key] = value;

            return value ?? [];
        }

        private void ClearFilters()
        {
            Cache(FilterProjectTypes, default);
            Cache(FilterSystems, default);

            SelectedProjectTypes = [];
            SelectedSystems = [];

        }
    }

}
