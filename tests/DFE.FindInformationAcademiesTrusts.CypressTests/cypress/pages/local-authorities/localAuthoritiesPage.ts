import { AutocompleteHelper } from '../../support/autocompleteHelper';

class LocalAuthoritiesPage {

    elements = {
        laSearchBox: () => cy.get('#la-search'),
        mainLaSearchButton: () => cy.get('[data-testid="search"]'),
        laSearchAutoComplete: () => cy.get('#la-search-autocomplete-container'),
        autoCompleteFirstOption: () => cy.get('#la-search__option--0'),
        searchInput:() => cy.get('input[placeholder="e.g. leeds"]')
    };

    localAuthoritiesPageAppears(): this {
        cy.get('h1').should('contain.text', 'Find information about local authorities');
    
        return this;
    }

    public enterMainSearchText(searchText: string): this {
        AutocompleteHelper.typeWithAutocomplete(this.elements.laSearchBox, searchText);
    
        return this;
    }

    checkSearchInstructionsAndInput(): this {
        cy.contains('Search for a local authority').should('be.visible');

        cy.get('.govuk-hint').contains('Search by LA Code or name. Include any punctuation in names.')
            .should('be.visible');

        cy.get('#la-search-autocomplete-container')
            .should('be.visible');
    
            return this;
    }

    assertPageHeading(expectedHeading: string): this {
        cy.get('h1').should('contain.text', expectedHeading);
    
        return this;
    }

    assertSearchInputVisible(): this {
        this.elements.laSearchBox().should('be.visible');
        this.elements.searchInput().should('be.visible');
    
            return this;
    }

    assertOtherToolsAndProductsCardsVisible(): this {
        const toolNames = [
            /Local Authority Interactive Tool\s+\(LAIT\)/,
            /Attainment Tool with Latest Available Schools Time-series\s+\(ATLAST\)/,
            /Explore Education Statistics\s+\(EES\)/,
            /Financial Benchmarking and Insights Tool\s+\(FBIT\)/,
            /Association of Directors of Children's Services\s+\(ADCS\)/, 
            /Data tools/,
        ];

        toolNames.forEach((name) => {
            cy.contains('a', name)
                .should('be.visible')
                .and('have.attr', 'target', '_blank');
        });

        return this;
    }

    searchFor(value: string): this {
        this.elements.laSearchBox().clear().type(value);
        this.elements.laSearchAutoComplete().should('be.visible');
        this.elements.autoCompleteFirstOption().should('be.visible').click();
        this.elements.mainLaSearchButton().click();
        cy.url().should('include', '/localauthorities/details');

        return this;
    }

    assertSearchResultsContain(value: string): this {

        cy.contains('a, td, th, li, span, div', value, { matchCase: false }).should('be.visible');
        return this;
    }

    assertNoResultsMessageVisible(value: string): this {
        this.elements.laSearchBox().clear().type(value);
        this.elements.mainLaSearchButton().click();
        cy.url().should('include', '/localauthorities/results');
        cy.get('h1').should('contain.text', 'Search');
        cy.get('#results-details')
            .invoke('text')
            .should('include', `No results for "${value}"`);

        return this;
    }

    searchForSqlInjection(value: string): this {
        this.elements.laSearchBox().clear().type(value);
        this.elements.mainLaSearchButton().click();
        cy.get('h1').should('contain.text', 'Sorry, there is a problem with the service');

        return this;
    }
}
const localAuthoritiesPage = new LocalAuthoritiesPage();
export default localAuthoritiesPage; 
