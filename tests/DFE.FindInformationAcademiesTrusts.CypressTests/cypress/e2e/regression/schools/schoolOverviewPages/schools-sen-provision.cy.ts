import schoolsPage from "../../../../pages/schools/schoolsPage";
import { senSchoolData } from "../../../../support/test-data-store";

describe("Testing the Schools Overview SEN pages", () => {

    describe("SEN page tab", () => {
        senSchoolData.forEach(({ typeOfSchool, urn }) => {
            it(`Checks the page name is correct for a ${typeOfSchool} on the urn ${urn}`, () => {
                cy.visit(`/schools/overview/sen?urn=${urn}`);
                schoolsPage
                    .checkOverviewPageNamePresent();
            });

            it(`Checks the tab name is present and correct for a school and academy`, () => {
                cy.visit(`/schools/overview/sen?urn=${urn}`);
                schoolsPage
                    .checkSENTabNameCorrect();
            });

            it(`Checks the tab name is present and correct for a school and academy`, () => {
                cy.visit(`/schools/overview/sen?urn=${urn}`);
                schoolsPage
                    .checkSENSubpageHeaderCorrect();
            });

            it(`Checks the subpage name is present and correct for a school and academy`, () => {
                cy.visit(`/schools/overview/sen?urn=${urn}`);
                schoolsPage
                    .checkSENSubpageHeaderCorrect();
            });

            it(`Checks the details page detail data components are present for a school and academy`, () => {
                cy.visit(`/schools/overview/sen?urn=${urn}`);
                schoolsPage
                    .checkSENDataItemsPresent();
            });

            it('Check SEN list only contains valid SEN provision types', () => {
                cy.visit(`/schools/overview/sen?urn=${urn}`);
                schoolsPage
                    .checkCorrectSENTypePresent();
            });
        });
    });
});
