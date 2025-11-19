import { initAll as govukFrontendInitAll } from 'govuk-frontend'
import { Autocomplete } from './javascripts/autocomplete'
import $ from 'jquery'
import { SortableTable, nodeListForEach } from '@ministryofjustice/frontend'

window.$ = $

govukFrontendInitAll()

// Sortable table initialisation from the MOJ Frontend library
const sortableTables = document.querySelectorAll(
  '[data-module="moj-sortable-table"]'
)
nodeListForEach(
  sortableTables,
  (table) =>
    new SortableTable({
      table
    })
)

// DfE rebranding JS
document.addEventListener('DOMContentLoaded', function () {
  const navigationToggle = document.getElementById('super-navigation-menu-toggle')
  const searchToggle = document.getElementById('super-search-menu-toggle')
  const navigationMenu = document.getElementById('super-navigation-menu')
  const searchMenu = document.getElementById('super-search-menu')

  // Function to close all menus and reset button states
  function closeAllMenus () {
    // Close navigation
    if (navigationToggle && navigationMenu) {
      navigationToggle.setAttribute('aria-expanded', 'false')
      navigationMenu.setAttribute('hidden', 'hidden')
      navigationToggle.classList.remove('gem-c-layout-super-navigation-header__open-button')
    }

    // Close search
    if (searchToggle && searchMenu) {
      searchToggle.setAttribute('aria-expanded', 'false')
      searchMenu.setAttribute('hidden', 'hidden')
      searchToggle.classList.remove('gem-c-layout-super-navigation-header__open-button')
    }
  }

  // Toggle navigation menu
  if (navigationToggle && navigationMenu) {
    navigationToggle.addEventListener('click', function () {
      const isHidden = navigationMenu.hasAttribute('hidden')

      if (isHidden) {
        // Open navigation menu
        navigationToggle.setAttribute('aria-expanded', 'true')
        navigationMenu.removeAttribute('hidden')
        navigationToggle.classList.add('gem-c-layout-super-navigation-header__open-button')

        // Close search if open
        if (searchToggle && searchMenu) {
          searchToggle.setAttribute('aria-expanded', 'false')
          searchMenu.setAttribute('hidden', 'hidden')
          searchToggle.classList.remove('gem-c-layout-super-navigation-header__open-button')
        }
      } else {
        // Close navigation menu
        navigationToggle.setAttribute('aria-expanded', 'false')
        navigationMenu.setAttribute('hidden', 'hidden')
        navigationToggle.classList.remove('gem-c-layout-super-navigation-header__open-button')
      }
    })
  }

  // Toggle search panel
  if (searchToggle && searchMenu) {
    searchToggle.addEventListener('click', function () {
      const isHidden = searchMenu.hasAttribute('hidden')

      if (isHidden) {
        // Open search menu
        searchToggle.setAttribute('aria-expanded', 'true')
        searchMenu.removeAttribute('hidden')
        searchToggle.classList.add('gem-c-layout-super-navigation-header__open-button')

        // Close navigation if open
        if (navigationToggle && navigationMenu) {
          navigationToggle.setAttribute('aria-expanded', 'false')
          navigationMenu.setAttribute('hidden', 'hidden')
          navigationToggle.classList.remove('gem-c-layout-super-navigation-header__open-button')
        }
      } else {
        // Close search menu
        searchToggle.setAttribute('aria-expanded', 'false')
        searchMenu.setAttribute('hidden', 'hidden')
        searchToggle.classList.remove('gem-c-layout-super-navigation-header__open-button')
      }
    })
  }

  document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
      closeAllMenus()
    }
  })
})

// Add Autocomplete namespace so that exposed methods can be invoked from Razor Partial
window.Autocomplete = new Autocomplete()
