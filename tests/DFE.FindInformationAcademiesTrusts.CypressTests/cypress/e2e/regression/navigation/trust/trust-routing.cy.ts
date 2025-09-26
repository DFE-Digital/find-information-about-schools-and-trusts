import commonPage from "../../../../pages/commonPage";
import { TestDataStore } from "../../../../support/test-data-store";

describe('Trust Routing and 404 Tests', () => {

    describe("Routing tests", () => {
        const groupsNotToShow = [
            { uid: 86042, scenario: "Open Children's Centres Collaboration" },
            { uid: 86074, scenario: "Open Children's Centres Group" },
            { uid: 5701, scenario: "Open Federation" },
            { uid: 5702, scenario: "Closed Multi-academy trust" },
            { uid: 5717, scenario: "Open School sponsor" },
            { uid: 5707, scenario: "Closed Single-academy trust" },
            { uid: 5672, scenario: "Open Trust (old-style trust, not SAT or MAT)" },
            { uid: 5440, scenario: "Open Umbrella trust" }
        ];

        groupsNotToShow.forEach(({ uid, scenario }) => {
            describe(`${scenario}`, () => {
                TestDataStore.GetAllTrustSubpagesForUid(uid).forEach(({ pageName, subpages }) => {
                    describe(`${pageName}`, () => {
                        subpages.forEach(({ subpageName, url }) => {
                            it(`Should check that navigating to subpages for ${scenario} displays the 404 not found page - ${pageName} > ${subpageName}`, () => {
                                // Set up an interceptor to check that the page response is a 404
                                commonPage.interceptAndVerifyResponseHas404Status(url);

                                // Go to the given subpage
                                cy.visit(url, { failOnStatusCode: false });

                                // Check that the 404 response interceptor was called
                                cy.wait('@checkTheResponseIs404');

                                // Check that the data sources component has a subheading for each subnav
                                commonPage
                                    .checkPageNotFoundDisplayed();
                            });
                        });
                    });
                });
            });
        });

        context("Tests a trust with a school within it that has no pupils on role within it to ensure the issue of a 500 page appearing does not happen", () => {
            beforeEach(() => {
                commonPage
                    .interceptAndVerifyNo500Errors();
            });

            it(`Should have no 500 error on the pupils population page for a school with no pupils`, () => {
                cy.visit('/trusts/overview/trust-details?uid=15776');

                // Verify page loads successfully without 500 errors
                cy.get('body').should('be.visible');

                // The interceptor will automatically fail if any 500 errors occur
                cy.wait('@allRequests');
            });
        });
    });
}); 
