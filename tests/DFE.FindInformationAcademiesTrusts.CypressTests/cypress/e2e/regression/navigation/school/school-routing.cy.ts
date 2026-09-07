import commonPage from "../../../../pages/commonPage";

describe('School Routing and error handling Tests', () => {

    describe("School routing tests", () => {

        context("Tests a school that has no pupils on role within it to ensure the issue of a 500 page appearing does not happen", () => {
            beforeEach(() => {
                commonPage
                    .interceptAndVerifyNo500Errors();
            });

            it(`Should have no 500 error on the pupils population page for a school with no pupils`, () => {
                cy.visit('/schools/pupils/population?urn=151140');

                // Verify page loads successfully without 500 errors
                cy.get('body').should('be.visible');

                // The interceptor will automatically fail if any 500 errors occur
                cy.wait('@allRequests');
            });
        });
    });
});
