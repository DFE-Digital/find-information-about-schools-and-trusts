class HeaderPage {

    elements = {
        headerSearchButton: () => cy.get('.dfe-search__submit'),
        mainSearchBox: () => cy.get('#header-search'),
        headerAutocomplete: () => cy.get('#header-search__listbox')
    };

    public clickHeaderSearchButton(): this {
        this.elements.headerSearchButton().click();
        return this;
    }

    public checkHeaderSearchButtonPresent(): this {
        this.elements.headerSearchButton().should('be.visible').should('be.enabled');
        return this;
    }

    public checkHeaderAutocompleteIsPresent(): this {
        this.elements.headerAutocomplete().should('be.visible');
        return this;
    }

    public checkAutocompleteContainsTypedText(searchText: string): this {
        cy.log(`Searching for text: "${searchText}" in autocomplete suggestions`);

        // First ensure the autocomplete is visible
        this.elements.headerAutocomplete().should('be.visible');

        // Get suggestions and log them
        this.elements.headerAutocomplete().then(($listbox) => {
            const suggestions = Array.from($listbox.children()).map(el => el.innerText);
            cy.log(`Found ${suggestions.length} suggestions:`);
            cy.log(suggestions.join('\n'));
        });

        // Verify suggestions contain our text
        this.elements.headerAutocomplete()
            .should('exist', { timeout: 20000 })
            .should(($listbox) => {
                const suggestions = Array.from($listbox.children()).map(el => el.innerText);
                expect(suggestions.length, 'Expected to find suggestions in dropdown').to.be.greaterThan(0);
                const hasMatch = suggestions.some(text => text.toLowerCase().includes(searchText.toLowerCase()));
                assert.isTrue(hasMatch, `Expected to find "${searchText}" in suggestions`);
            });

        return this;
    }

    public enterHeaderSearchText(searchText: string): this {
        this.elements.mainSearchBox().type(searchText);
        return this;
    }

}

const headerPage = new HeaderPage();
export default headerPage;
