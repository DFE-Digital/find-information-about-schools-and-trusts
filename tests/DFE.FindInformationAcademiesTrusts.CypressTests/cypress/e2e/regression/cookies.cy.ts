import commonPage from "../../pages/commonPage";
import cookiesPage from "../../pages/cookiesPage";
import navigation from "../../pages/navigation";

describe('Cookie page and consent tests', () => {

    beforeEach(() => {
        cy.clearAllCookies();

        cy.visit('/cookies');
    });

    it("Checks the browser title is correct", () => {
        commonPage
            .checkThatBrowserTitleMatches('Cookies - Find information about schools and trusts');
    });

    it('should check that both mandatory and optional cookies both exist after accepting optional cookies', () => {
        cookiesPage
            .acceptCookies()
            .clickSaveChangesButton();

        commonPage
            .checkSuccessPopup('You’ve set your cookie preferences');

        cy.visit('/trusts/overview/trust-details?uid=5712');

        //check optional cookies exist
        cy.getCookie('ai_user').should('exist');
        cy.getCookie('ai_session').should('exist');
        cy.getCookie('ai_authUser').should('exist');
        cy.getCookie('_ga').should('exist');

        //check mandatory cookies exist after saving
        cy.getCookie('.FindInformationAcademiesTrusts.CookieConsent').should('exist');
        cy.getCookie('ASLBSA').should('exist');
        cy.getCookie('ASLBSACORS').should('exist');
        cy.getCookie('.FindInformationAcademiesTrusts.Antiforgery').should('exist');

    });

    it('should check that mandatory cookies exist but optional cookies do not after rejecting optional cookies', () => {
        cookiesPage
            .rejectCookies()
            .clickSaveChangesButton();

        commonPage
            .checkSuccessPopup('You’ve set your cookie preferences');

        cy.visit('/trusts/overview/trust-details?uid=5712');

        //check optional cookies do not  exist
        cy.getCookie('ai_user').should('not.exist');
        cy.getCookie('ai_session').should('not.exist');
        cy.getCookie('ai_authUser').should('not.exist');
        cy.getCookie('_ga').should('not.exist');

        //check mandatory cookies do not exist after saving
        cy.getCookie('.FindInformationAcademiesTrusts.CookieConsent').should('exist');
        cy.getCookie('ASLBSA').should('exist');
        cy.getCookie('ASLBSACORS').should('exist');
        cy.getCookie('.FindInformationAcademiesTrusts.Antiforgery').should('exist');
    });

    it('should check that the return to previous page button actually takes me to my previous page after accept cookies', () => {
        cy.visit('/search');

        navigation
            .clickCookiesLink()
            .checkCurrentURLIsCorrect('/cookies');

        cookiesPage
            .acceptCookies()
            .clickSaveChangesButton()
            .clickReturnToPreviousPageButton();

        navigation
            .checkCurrentURLIsCorrect('/search');
    });

    it('should check that the return to previous page button actually takes me to my previous page after reject cookies', () => {
        cy.visit('/trusts/contacts/in-dfe?uid=5712');

        navigation
            .clickCookiesLink()
            .checkCurrentURLIsCorrect('/cookies');

        cookiesPage
            .rejectCookies()
            .clickSaveChangesButton()
            .clickReturnToPreviousPageButton();

        navigation
            .checkCurrentURLIsCorrect('/trusts/contacts/in-dfe?uid=5712');
    });

    it('should check that both cookie accept and reject radio buttons are in their empty state when coming in from scratch', () => {
        cookiesPage
            .checkCookiesAcceptIsNotChecked()
            .checkCookiesRejectIsNotChecked();
    });

    it('should check that the accept cookies button stays checked after enabling it', () => {
        cookiesPage
            .acceptCookies()
            .clickSaveChangesButton();

        commonPage
            .checkSuccessPopup('You’ve set your cookie preferences');

        cy.visit('/trusts/overview/trust-details?uid=5712');

        navigation
            .clickCookiesLink()
            .checkCurrentURLIsCorrect('/cookies');

        cookiesPage
            .checkCookiesAcceptIsInteracted();
    });

    it('should check that the accept cookies button stays checked after enabling it', () => {
        cookiesPage
            .rejectCookies()
            .clickSaveChangesButton();

        commonPage
            .checkSuccessPopup('You’ve set your cookie preferences');

        cy.visit('/trusts/overview/trust-details?uid=5712');

        navigation
            .clickCookiesLink()
            .checkCurrentURLIsCorrect('/cookies');

        cookiesPage
            .checkCookiesRejectIsInteracted();
    });
});
