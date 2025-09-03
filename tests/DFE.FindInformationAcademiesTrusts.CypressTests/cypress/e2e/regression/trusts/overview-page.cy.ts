import navigation from "../../../pages/navigation";
import overviewPage from "../../../pages/trusts/overviewPage";
import commonPage from "../../../pages/commonPage";

describe("Testing the components of the Trust overview page", () => {

    describe("Trust details", () => {
        beforeEach(() => {
            cy.visit('/trusts/overview/trust-details?uid=5712');
        });

        it("The page loads with the correct headings and data", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Trust details - Overview - {trustName} - Find information about schools and trusts');

            overviewPage
                .checkTrustDetailsSubHeaderPresent()
                .checkTrustDetailsPresent()
                .checkTrustDetailsItemsPresent()
                .checkInformationFromOtherServicesPresent();

            navigation
                .checkPageNameBreadcrumbPresent("Overview");
        });
    });

    describe("Trust summary", () => {
        beforeEach(() => {
            cy.visit('/trusts/overview/trust-summary?uid=5712');
        });

        it("The page loads with the correct headings and data", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Trust summary - Overview - {trustName} - Find information about schools and trusts');

            overviewPage
                .checkTrustSummarySubHeaderPresent()
                .checkOverviewHeaderPresent()
                .checkTrustSummaryDetailsPresent()
                .checkTrustSummaryItemsPresent();

            navigation
                .checkPageNameBreadcrumbPresent("Overview");
        });
    });

    describe("Reference numbers", () => {
        beforeEach(() => {
            cy.visit('/trusts/overview/reference-numbers?uid=5712');
        });

        it("The page loads with the correct headings and data", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Reference numbers - Overview - {trustName} - Find information about schools and trusts');

            overviewPage
                .checkReferenceNumbersSubHeaderPresent()
                .checkReferenceNumbersSectionPresent()
                .checkReferenceNumbersItemsPresent();

            navigation
                .checkPageNameBreadcrumbPresent("Overview");
        });
    });

    describe("Testing the Overview sub navigation", () => {

        it('Should check that the trust details navigation button takes me to the correct page', () => {
            cy.visit('/trusts/overview/trust-summary?uid=5527');

            overviewPage
                .clickTrustDetailsSubnavButton();

            navigation
                .checkCurrentURLIsCorrect('/trusts/overview/trust-details?uid=5527');

            overviewPage
                .checkAllSubNavItemsPresent()
                .checkTrustDetailsPresent();
        });

        it('Should check that the trust summary navigation button takes me to the correct page', () => {
            cy.visit('/trusts/overview/reference-numbers?uid=5527');

            overviewPage
                .clickTrustSummarySubnavButton();

            navigation
                .checkCurrentURLIsCorrect('/trusts/overview/trust-summary?uid=5527');

            overviewPage
                .checkAllSubNavItemsPresent()
                .checkTrustSummaryDetailsPresent();
        });

        it('Should check that the reference numbers navigation button takes me to the correct page', () => {
            cy.visit('/trusts/overview/trust-details?uid=5527');

            overviewPage
                .clickReferenceNumbersSubnavButton();

            navigation
                .checkCurrentURLIsCorrect('/trusts/overview/reference-numbers?uid=5527');

            overviewPage
                .checkAllSubNavItemsPresent()
                .checkReferenceNumbersSectionPresent();
        });

        it('Should check that the overview sub nav items are not present when I am not on the overview page', () => {
            cy.visit('/trusts/contacts/in-dfe?uid=5527');

            overviewPage
                .checkSubNavNotPresent();
        });
    });
});
