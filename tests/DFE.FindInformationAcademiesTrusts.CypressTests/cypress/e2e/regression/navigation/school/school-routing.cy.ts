import { TestDataStore } from "../../../../support/test-data-store";
import commonPage from "../../../../pages/commonPage";

describe('School Routing and 404 Tests', () => {

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
    });
}); 
