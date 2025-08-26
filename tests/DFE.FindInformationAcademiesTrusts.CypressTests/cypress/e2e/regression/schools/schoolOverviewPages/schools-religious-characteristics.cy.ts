import schoolsPage from '../../../../pages/schools/schoolsPage';
import { religiousCharacteristicsSchoolData } from '../../../../support/test-data-store';

describe("Testing the Schools religious characteristics pages", () => {

    describe("Basic page functionality and navigation", () => {
        religiousCharacteristicsSchoolData.forEach(({ typeOfSchool, urn }) => {

            beforeEach(() => {
                cy.visit(`/schools/overview/religiouscharacteristics?urn=${urn}`);
            });

            it(`Checks the page name is correct for a ${typeOfSchool} on the urn ${urn}`, () => {
                schoolsPage
                    .checkOverviewPageNamePresent();
            });

            it(`Checks the subpage header is present and correct for a ${typeOfSchool} on the urn ${urn}`, () => {
                schoolsPage
                    .checkReligiousCharacteristicsHeaderPresent();
            });
        });
    });

    describe("Religious characteristics data components and headers", () => {
        religiousCharacteristicsSchoolData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/overview/religiouscharacteristics?urn=${urn}`);
            });

            it(`Checks all religious characteristics headers are present for a ${typeOfSchool}`, () => {
                schoolsPage
                    .checkReligiousCharacteristicsCardItemsPresent();
            });

            it(`Checks all religious characteristics data items are present for a ${typeOfSchool}`, () => {
                schoolsPage
                    .checkReligiousCharacteristicsDataItemsPresent();
            });
        });
    });
});
