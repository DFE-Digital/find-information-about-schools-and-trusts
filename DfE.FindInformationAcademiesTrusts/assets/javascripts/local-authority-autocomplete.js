import accessibleAutocomplete from 'accessible-autocomplete'

export function loadLocalAuthorityAutocomplete (inputId, endpoint, defaultValue = '', placeholderText = '') {
  const template = document.getElementById(`${inputId}-js-autocomplete-template`)
  const noJsContainer = document.getElementById(`${inputId}-no-js-container`)
  noJsContainer.replaceWith(template.content.cloneNode(true))

  const codeInput = document.getElementById(`${inputId}-selected-code`)
  let timer = null
  let controller = null

  const source = (query, populateResults) => {
    codeInput.value = ''
    clearTimeout(timer)
    controller?.abort()

    timer = setTimeout(async () => {
      controller = new AbortController()
      try {
        const separator = endpoint.includes('?') ? '&' : '?'
        const response = await fetch(`${endpoint}${separator}keywords=${encodeURIComponent(query)}`, { signal: controller.signal })
        populateResults(await response.json())
      } catch (error) {
        if (error.name !== 'AbortError') throw error
      }
    }, 300)
  }

  const label = (la) => (la && typeof la !== 'string' ? la.name : la)

  accessibleAutocomplete({
    element: document.getElementById(`${inputId}-autocomplete-container`),
    id: inputId,
    name: 'keywords',
    defaultValue,
    placeholder: placeholderText,
    source,
    minLength: 3,
    autoselect: false,
    confirmOnBlur: false,
    displayMenu: 'overlay',
    showNoOptionsFound: true,
    templates: {
      inputValue: label,
      suggestion: (la) => (la && typeof la !== 'string' ? la.name : la)
    },
    onConfirm: (selected) => {
      if (selected) codeInput.value = selected.code
    }
  })

  // Fix WCAG failure 'Required ARIA attribute not present: aria-controls'
  document.getElementById(inputId).setAttribute('aria-controls', `${inputId}__listbox`)
}
