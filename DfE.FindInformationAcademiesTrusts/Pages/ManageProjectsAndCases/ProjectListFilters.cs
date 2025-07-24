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
        public List<string> AvailableProjectTypes { get; set; } = new();

        public List<string> AvailableSystems { get; set; } = new();
        
        [BindProperty]
        public string[] SelectedProjectTypes { get; set; } = Array.Empty<string>();

        [BindProperty]
        public string[] SelectedSystems { get; set; } = Array.Empty<string>();

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

                SelectedProjectTypes = Array.Empty<string>();
                SelectedSystems = Array.Empty<string>();

                return;
            }

            if (query.ContainsKey("remove"))
            {
                SelectedProjectTypes = GetAndRemove(FilterProjectTypes, GetFromQuery(nameof(SelectedProjectTypes)), true);
                SelectedSystems = GetAndRemove(FilterSystems, GetFromQuery(nameof(SelectedSystems)), true);

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
                SelectedProjectTypes = Get(FilterProjectTypes, true);
                SelectedSystems = Get(FilterSystems, true);
            }

            string[] GetFromQuery(string key)
            {
                return query.ContainsKey(key) ? query[key]! : Array.Empty<string>();
            }
        }

        private string[] Get(string key, bool persist = false)
        {
            if (_store.ContainsKey(key) is false) return Array.Empty<string>();

            string[]? value = (string[]?)_store[key];
            if (persist) Cache(key, value);

            return value ?? Array.Empty<string>();
        }

        private string[] GetAndRemove(string key, string[]? value, bool persist = false)
        {
            if (_store.ContainsKey(key) is false) return Array.Empty<string>();

            string[]? currentValues = (string[]?)_store[key];

            if (value is not null && value.Length > 0 && currentValues is not null)
            {
                currentValues = currentValues.Where(x => !value.Contains(x)).ToArray();
            }

            if (persist) Cache(key, currentValues);

            return currentValues ?? Array.Empty<string>();
        }

        private string[] Cache(string key, string[]? value)
        {
            if (value is null || value.Length == 0)
                _store.Remove(key);
            else
                _store[key] = value;

            return value ?? Array.Empty<string>();
        }

        private void ClearFilters()
        {
            Cache(FilterProjectTypes, default);
            Cache(FilterSystems, default);
        }

        /// <summary>
        ///    Removes all project list filters from the store
        /// </summary>
        /// <param name="store">the store used to cache filters between pages</param>
        /// <remarks>
        ///    Note that, when using TempData, this won't take effect until after the next request context that returns a 2xx response!
        /// </remarks>
        public static void ClearFiltersFrom(IDictionary<string, object?> store)
        {
            new ProjectListFilters().PersistUsing(store).ClearFilters();
        }
    }

}
