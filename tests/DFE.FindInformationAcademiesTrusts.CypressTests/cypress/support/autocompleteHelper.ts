export class AutocompleteHelper {
    static setupIntercept(): void {
        cy.intercept('GET', '/search?handler=populateautocomplete*').as('autocompleteRequest');
    }

    static waitForResponse(): void {
        cy.wait('@autocompleteRequest', { timeout: 15000 }).then((interception) => {
            const responseBody = interception.response?.body as unknown[];
            cy.log(`Autocomplete handler responded with ${responseBody?.length || 0} results`);
        });
    }

    static typeWithAutocomplete(element: () => Cypress.Chainable<JQuery<HTMLElement>>, text: string): void {
        this.setupIntercept();
        element().clear().type(text, { delay: 100 });
    }
}
