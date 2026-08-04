import accessibleAutocomplete from 'accessible-autocomplete'

export class Autocomplete {
  debounceTimer = null
  abortController = null

  suggest = async (query, populateResults, inputId) => {
    // Clear selected trust if we then search for new trust
    // Avoids the uid turning up in the url
    const searchInput = document.getElementById(`${inputId}-selected-id`)
    if (searchInput.hasAttribute('value')) {
      searchInput.removeAttribute('value')
    }

    if (!query?.trim()) {
      populateResults([])
      return
    }

    // Cancel any previous search that is still running
    if (this.abortController) {
      this.abortController.abort()
    }

    this.abortController = new AbortController()

    try {
      const response = await fetch(
          `/search?handler=populateautocomplete&keywords=${encodeURIComponent(query)}`,
          {
            signal: this.abortController.signal
          }
      )

      const results = await response.json()
      populateResults(results)
    } catch (error) {
      // Ignore aborted requests
      if (error.name !== 'AbortError') {
        throw error
      }
    }
  }

  getName = (result) => {
    // This check for a string is related to a known issue in autocomplete when setting a default value
    // https://github.com/alphagov/accessible-autocomplete/issues/424
    if (result && typeof result !== 'string') return result.name
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
      source: (query, populateResults) => {
        clearTimeout(this.debounceTimer)

        // Clear suggestions immediately when input is empty
        if (!query?.trim()) {
          if (this.abortController) {
            this.abortController.abort()
          }

          populateResults([])
          return
        }

        this.debounceTimer = setTimeout(() => {
          this.suggest(query, populateResults, inputId)
        }, 500)
      },
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
          document.getElementById(`${inputId}-selected-id`).value = selected.id
          document.getElementById(`${inputId}-selected-searchResultType`).value = selected.resultType
        }
      }
    })

    // Fix WCAG accessibility failure 'Required ARIA attribute not present: aria-controls'
    // Related issue on Accessible Autocomplete component:
    // https://github.com/alphagov/accessible-autocomplete/issues/565
    document
      .getElementById(inputId)
      .setAttribute('aria-controls', `${inputId}__listbox`)
  }
}
