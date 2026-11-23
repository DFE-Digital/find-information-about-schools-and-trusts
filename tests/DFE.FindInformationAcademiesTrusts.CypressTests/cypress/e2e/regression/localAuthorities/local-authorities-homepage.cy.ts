import homePage from '../../../pages/homePage';
import localAuthoritiesPage from '../../../pages/local-authorities/localAuthoritiesPage';

describe('User navigates to the Local authorities tab ', () => {

    beforeEach(() => {
        cy.visit("/");
        homePage
            .verifyTrustAndSchoolTabIsDefault()
            .verifyHeading('Find information about schools and trusts')
            .clickLocalAuthoritiesTab();
    });

    it('user should be able to access the Local authorities page', () => {
        localAuthoritiesPage
            .localAuthoritiesPageAppears();
    });

    it('displays the heading and introductory text', () => {
        cy.contains('Use this tool to find information on local authorities.')
            .should('be.visible');
    });

    it('displays the search instructions and input', () => {
        localAuthoritiesPage.checkSearchInstructionsAndInput();
    });

    it('displays an enabled search button', () => {
        localAuthoritiesPage
          .assertSearchInputVisible()
        
    });

    it('displays the other tools and products links', () => {
        cy.contains('Other tools and products (links open in a new tab)')
            .should('be.visible');
        localAuthoritiesPage.assertOtherToolsAndProductsCardsVisible();
    });
});
