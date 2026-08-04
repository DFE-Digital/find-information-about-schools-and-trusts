import accessibleAutocomplete from 'accessible-autocomplete'

export class Autocomplete {
  suggest = async (query, populateResults, inputId) => {
    // Clear selected trust if we then search for new trust
    // Avoids the uid turning up in the url
    const searchInput = document.getElementById(`${inputId}-selected-id`)
    if (searchInput.hasAttribute('value')) {
      searchInput.removeAttribute('value')
    }
    const ukprnInput = document.getElementById(`${inputId}-selected-ukprn`)
    if (ukprnInput && ukprnInput.hasAttribute('value')) {
      ukprnInput.removeAttribute('value')
    }

    if (query) {
      const response = await fetch(`/search?handler=populateautocomplete&keywords=${query}`)
      const results = await response.json()
      populateResults(results)
    }
  }

  getName = (result) => {
    // This check for a string is related to a known issue in autocomplete when setting a default value https://github.com/alphagov/accessible-autocomplete/issues/424
    if (result && typeof (result) !== 'string') return result.name
    return result
  }

  getHint = (result) => {
    const hintText = this.getName(result)
    if (result?.address) {
      return `${hintText}<span class="autocomplete__option-hint">${result.address}</span>`
    }
    return hintText
  }

  loadTrustSearch = async (inputId, defaultValue, placeholderText) => {
    const autocompleteTemplate = document.getElementById(`${inputId}-js-autocomplete-template`)
    const autocompleteTemplateContents = autocompleteTemplate.content.cloneNode(true)
    const elementToReplace = document.getElementById(`${inputId}-no-js-search-container`)

    elementToReplace.replaceWith(autocompleteTemplateContents)

    accessibleAutocomplete({
      defaultValue,
      element: document.getElementById(`${inputId}-autocomplete-container`),
      id: inputId,
      name: 'keywords',
      source: async (query, populateResults) => this.suggest(query, populateResults, inputId),
      autoselect: false,
      confirmOnBlur: false,
      displayMenu: 'overlay',
      placeholder: placeholderText || '',
      showNoOptionsFound: true,
      minLength: 3,
      templates: {
        inputValue: this.getName,
        suggestion: this.getHint
      },
      onConfirm: (selected) => {
        if (selected) {
          document.getElementById(`${inputId}-selected-id`).value = (selected.id)
          document.getElementById(`${inputId}-selected-searchResultType`).value = (selected.resultType)
          const ukprnEl = document.getElementById(`${inputId}-selected-ukprn`)
          if (ukprnEl) ukprnEl.value = selected.ukprn ?? ''
        }
      }
    })

    // Fix WCAG accessibility failure 'Required ARIA attribute not present: aria-controls'
    // related issue on Accessible Autocomplete component: https://github.com/alphagov/accessible-autocomplete/issues/565
    document.getElementById(inputId).setAttribute('aria-controls', `${inputId}__listbox`)
  }
}
