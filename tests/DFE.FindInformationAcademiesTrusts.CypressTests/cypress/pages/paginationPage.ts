class PaginationPage {

    elements = {
        homeButton: () => cy.get('.govuk-header__link--homepage'),
        paginationButton: (index: number) => cy.get(`[data-testid="page-${index}"]`),
        totalPaginationButtons: () => cy.get('[data-testid^="page-"]'),
        pagination: () => cy.get('.govuk-pagination'),
        nextButton: () => cy.contains('Next'),
        previousButton: () => cy.contains('Previous'),
        results: () => cy.get('.govuk-list > :nth-child(1)')
    };

    public returnToHome(): this {
        this.elements.homeButton().click();
        return this;
    }

    public getPaginationButton(index: number): Cypress.Chainable<JQuery<HTMLElement>> {
        return this.elements.paginationButton(index);
    }

    public getTotalPaginationButtons(): Cypress.Chainable<number> {
        return this.elements.totalPaginationButtons().its('length');
    }

    public checkSingleResultOnlyHasOnePage(pageNumber: number): this {
        this.elements.pagination().should('contain', pageNumber);
        this.elements.pagination().should('not.contain', 2, 3);
        return this;
    }

    public checkExpectedPageNumberInPaginationBar(pageNumber: number): this {
        this.getPaginationButton(pageNumber).should('exist');
        return this;
    }

    public checkResultIsNotInPaginationBar(pageNumber: number): this {
        this.getPaginationButton(pageNumber).should('not.exist');
        return this;
    }

    public clickPageNumber(pageNumber: number): this {
        this.getPaginationButton(pageNumber).click();
        return this;
    }

    public clickNext(): this {
        this.elements.nextButton().click();
        return this;
    }

    public clickPrevious(): this {
        this.elements.previousButton().click();
        return this;
    }

    public checkPreviousButtonNotPresent(): this {
        this.elements.previousButton().should('not.exist');
        return this;
    }

    public checkNextButtonNotPresent(): this {
        this.elements.nextButton().should('not.exist');
        return this;
    }

    public getResults(): Cypress.Chainable<JQuery<HTMLElement>> {
        return this.elements.results();
    }

}

const paginationPage = new PaginationPage();
export default paginationPage;
