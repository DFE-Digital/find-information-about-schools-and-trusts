import { TestDataStore } from "../../../../support/test-data-store";
import commonPage from "../../../../pages/commonPage";

describe('School Routing and error handling Tests', () => {

    describe("School routing tests", () => {

        context("404 routing for unsupported and closed school types", () => {
            const schoolTypesNotToShow = [
                { urn: 136083, unsupportedSchoolType: "Independent schools" },
                { urn: 150163, unsupportedSchoolType: "Online provider" },
                { urn: 131832, unsupportedSchoolType: "Other types" },
                { urn: 133793, unsupportedSchoolType: "Universities" },
                { urn: 137210, unsupportedSchoolType: "Closed academy" },
                { urn: 142109, unsupportedSchoolType: "Closed school" },
            ];

            schoolTypesNotToShow.forEach(({ urn, unsupportedSchoolType }) => {
                TestDataStore.GetAllSchoolSubpagesForUrn(urn).forEach(({ pageName, subpages }) => {

                    describe(pageName, () => {

                        subpages.forEach(({ subpageName, url }) => {
                            it(`Should check that navigating to subpages for unsupported and closed school types display the 404 not found page ${pageName} > ${subpageName} > ${unsupportedSchoolType}`, () => {
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

        context("Tests a school that has no pupils on role within it to ensure the issue of a 500 page appearing does not happen", () => {
            beforeEach(() => {
                commonPage
                    .interceptAndVerifyNo500Errors();
            });

            it(`Should have no 500 error on the pupils population page for a school with no pupils`, () => {
                cy.visit('/schools/pupils/population?urn=147855');

                // Verify page loads successfully without 500 errors
                cy.get('body').should('be.visible');

                // The interceptor will automatically fail if any 500 errors occur
                cy.wait('@allRequests');
            });
        });
    });
});
