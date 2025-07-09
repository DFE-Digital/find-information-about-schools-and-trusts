import schoolsPage from '../../../../pages/schools/schoolsPage';
import { referenceNumbersTestData } from '../../../../support/test-data-store';

describe("Testing the Schools Reference numbers pages", () => {

    describe("Reference numbers page tab", () => {
        referenceNumbersTestData.forEach(({ description, urn }) => {

        beforeEach(() => {
           cy.visit(`/schools/overview/referencenumbers?urn=${urn}`);
        });

        it(`The page loads with the correct data for a ${description}`, () => {
            schoolsPage
                .checkSchoolReferenceNumbersHeaderPresent()
                .checkReferenceNumbersCardItemsPresent();
          });
        });
    });
});