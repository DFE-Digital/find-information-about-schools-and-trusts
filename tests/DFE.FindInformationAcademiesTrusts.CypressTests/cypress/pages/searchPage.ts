class SearchPage {

    elements = {
        searchResults: () => cy.get('#main-content'),
        mainAutocomplete: () => cy.get('#home-search__listbox', { timeout: 10000 }),
        searchPageSearchBox: () => cy.get('#search'),
        searchPageSearchButton: () => cy.get('[data-testid="search"]'),
        resultsInfo: () => cy.get('#results-details'),
        resultItems: () => cy.get('.govuk-list li'),
        nextButton: () => cy.get('[data-testid="next-page"]'),
        hasNextButton: () => cy.$$('[data-testid="next-page"]').length > 0,
        establishmentType: () => cy.get('[data-testid="establishment-type"]'),
        urn: () => cy.get('[data-testid="urn"]')
    };

    public checkSearchResultsReturned(searchText: string): this {
        this.elements.searchResults().each(($el) => {
            cy.wrap($el).should('include.text', searchText);
        });
        return this;
    }

    public checkNoSearchResultsFound(): this {
        this.elements.searchResults().should('include.text', "Check you've entered the name or reference number correctly and included any punctuation");
        return this;
    }

    public checkMainAutocompleteIsPresent(): this {
        this.elements.mainAutocomplete().should('be.visible');
        return this;
    }

    public checkAutocompleteContainsTypedText(searchText: string): this {
        cy.log(`Searching for text: "${searchText}" in autocomplete suggestions`);

        // First ensure the autocomplete is visible
        this.elements.mainAutocomplete().should('be.visible');

        // Get suggestions and log them
        this.elements.mainAutocomplete().then(($listbox) => {
            const suggestions = Array.from($listbox.children()).map(el => el.innerText);
            cy.log(`Found ${suggestions.length} suggestions:`);
            cy.log(suggestions.join('\n'));
        });

        // Verify suggestions contain our text
        this.elements.mainAutocomplete()
            .should('exist', { timeout: 10000 })
            .should(($listbox) => {
                const suggestions = Array.from($listbox.children()).map(el => el.innerText);
                expect(suggestions.length, 'Expected to find suggestions in dropdown').to.be.greaterThan(0);
                const hasMatch = suggestions.some(text => text.toLowerCase().includes(searchText.toLowerCase()));
                assert.isTrue(hasMatch, `Expected to find "${searchText}" in suggestions`);
            });

        return this;
    }

    public enterSearchResultsSearchText(searchText: string): this {
        this.elements.searchPageSearchBox().type(searchText);
        return this;
    }

    public clickSearchPageSearchButton(): this {
        this.elements.searchPageSearchButton().click();
        return this;
    }

    public checkSearchResultsInfoReturnsCorrectInfo(searchText: string): this {
        this.elements.resultsInfo().should('include.text', searchText);
        return this;
    }

    public validateSearchResultsCountWithPagination(): this {
        const countResultsOnPage = (expectedTotalResults: number, accumulatedResults: number): void => {
            this.elements.resultItems().then(($items: JQuery<HTMLElement>) => {
                accumulatedResults += $items.length;
                cy.log(`Accumulated Results: ${accumulatedResults}`);

                if (this.elements.hasNextButton()) {
                    this.elements.nextButton().then(($next) => {
                        if ($next.is(':visible') && !$next.is(':disabled')) {
                            cy.wrap($next).click();
                            countResultsOnPage(expectedTotalResults, accumulatedResults);
                        } else {
                            assert.fail('Next button is invisible or disabled');
                        }
                    });
                } else {
                    expect(accumulatedResults).to.equal(expectedTotalResults);
                }
            });
        };

        this.elements.resultsInfo().invoke('text').then((text: string) => {
            const trustMatch = /Found (\d+) trusts/.exec(text);
            const schoolMatch = /and (\d+) schools/.exec(text);

            if (trustMatch?.[1] && schoolMatch?.[1]) {
                const trustCount = parseInt(trustMatch[1], 10);
                const schoolCount = parseInt(schoolMatch[1], 10);
                const expectedTotalResults = trustCount + schoolCount;
                const accumulatedResults = 0;
                cy.log(`Expecting ${expectedTotalResults} total results (${trustCount} trusts + ${schoolCount} schools)`);
                countResultsOnPage(expectedTotalResults, accumulatedResults);
            } else {
                throw new Error('Could not extract the number of trusts and schools from the results text.');
            }
        });

        return this;
    }

    public checkEstablishmentType(establishmentType: string): this {
        this.elements.establishmentType().should('include.text', establishmentType);
        return this;
    }

    public checkCorrectURN(urn: string): this {
        this.elements.urn().should('include.text', urn);
        return this;
    }
}

const searchPage = new SearchPage();
export default searchPage;
