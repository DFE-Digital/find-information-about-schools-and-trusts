import 'wick-a11y';
import homePage from '../../../pages/homePage';
import localAuthoritiesPage from '../../../pages/local-authorities/localAuthoritiesPage';

describe('Local Authority Accessibility', () => {

    beforeEach(() => {
        cy.visit('/');
        homePage
         .verifyHeading('Find information about trusts and schools')
         .clickLocalAuthoritiesTab();
    });

    describe('Local Authority Page should be accessible', () => {
        it('should have no accessibility violations on the Local Authority page', () => {
            cy.get('body').should('be.visible');
            cy.checkAccessibility();
        });

        it('should have no accessibility violations on a Local Authority details page', () => {
            localAuthoritiesPage.searchFor('Leeds')
            cy.checkAccessibility();
        });

    });
});