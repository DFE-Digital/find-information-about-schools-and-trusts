import { AutocompleteHelper } from '../support/autocompleteHelper';

class HeaderPage {

    elements = {
        headerSearchToggle: () => cy.get('#super-search-menu-toggle'),
        headerSearchButton: () => cy.get('.gem-c-search__submit'),
        mainSearchBox: () => cy.get('#header-search'),
        headerAutocomplete: () => cy.get('#header-search__listbox')
    };

    public clickHeaderSearchToggle(): this {
        this.elements.headerSearchToggle().click();
        return this;
    }

    public clickHeaderSearchButton(): this {
        this.elements.headerSearchButton().click();
        return this;
    }

    public checkHeaderSearchButtonPresent(): this {
        this.elements.headerSearchButton().should('be.visible').should('be.enabled');
        return this;
    }

    public checkHeaderSearchTogglePresent(): this {
        this.elements.headerSearchToggle().should('be.visible').should('be.enabled');
        return this;
    }

    public checkHeaderAutocompleteIsPresent(): this {
        this.elements.headerAutocomplete().should('be.visible');
        return this;
    }

    public checkAutocompleteContainsTypedText(searchText: string): this {
        cy.log(`Searching for text: "${searchText}" in autocomplete suggestions`);

        AutocompleteHelper.waitForResponse();

        this.elements.headerAutocomplete()
            .should('be.visible', { timeout: 10000 })
            .and('not.be.empty')
            .should('contain.text', searchText);

        return this;
    }

    public selectTheFisrtOptionInDropdown(): this {
        cy.wait(6000); // addeding explicit wait to ensure autocomplete options are loaded
        this.elements.headerAutocomplete()
            .first()
            .click();
        
        this.clickSearchIcon();

        cy.request({ url: window.location.href, failOnStatusCode: false }).then((response) => {
            expect(response.status).not.to.equal(404);
        });
        return this;
    }

    public clickSearchIcon(): this {
        this.elements.headerSearchButton().click();
        return this;
    }

    public enterHeaderSearchText(searchText: string): this {
        AutocompleteHelper.typeWithAutocomplete(this.elements.mainSearchBox, searchText);
        return this;
    }

}

const headerPage = new HeaderPage();
export default headerPage;
