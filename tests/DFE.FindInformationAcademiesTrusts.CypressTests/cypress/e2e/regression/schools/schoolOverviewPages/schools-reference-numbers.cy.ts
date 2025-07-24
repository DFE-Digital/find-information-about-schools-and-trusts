import schoolsPage from '../../../../pages/schools/schoolsPage';
import { referenceNumbersTestData } from '../../../../support/test-data-store';

describe("Testing the Schools Reference numbers pages", () => {

    describe("Basic page functionality and navigation", () => {
        referenceNumbersTestData.forEach(({ description, urn }) => {

            beforeEach(() => {
                cy.visit(`/schools/overview/referencenumbers?urn=${urn}`);
            });

            it(`Checks the page name is correct for a ${description} on the urn ${urn}`, () => {
                schoolsPage
                    .checkOverviewPageNamePresent();
            });

            it(`Checks the subpage header is present and correct for a ${description} on the urn ${urn}`, () => {
                schoolsPage
                    .checkSchoolReferenceNumbersHeaderPresent();
            });

            it(`Checks the school type is correct for a ${description} on the urn ${urn}`, () => {
                schoolsPage
                    .checkCorrectSchoolTypePresent();
            });
        });
    });

    describe("Reference numbers data components and headers", () => {
        referenceNumbersTestData.forEach(({ description, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/overview/referencenumbers?urn=${urn}`);
            });

            it(`Checks all reference numbers headers are present for a ${description}`, () => {
                schoolsPage
                    .checkReferenceNumbersCardItemsPresent();
            });

            it(`Checks the URN header is present and visible for a ${description}`, () => {
                schoolsPage
                    .checkUrnHeaderPresent();
            });

            it(`Checks the LAESTAB header is present and visible for a ${description}`, () => {
                schoolsPage
                    .checkLaestabHeaderPresent();
            });

            it(`Checks the UKPRN header is present and visible for a ${description}`, () => {
                schoolsPage
                    .checkUkprnHeaderPresent();
            });
        });
    });

    describe("Business rule validation: Data format and values", () => {
        referenceNumbersTestData.forEach(({ description, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/overview/referencenumbers?urn=${urn}`);
            });

            it(`Checks URN value is present and numeric for a ${description}`, () => {
                schoolsPage
                    .checkUrnValuePresent()
                    .checkUrnIsNumeric();
            });

            it(`Checks LAESTAB value is present and follows XXX/XXXX format for a ${description}`, () => {
                schoolsPage
                    .checkLaestabValuePresent()
                    .checkLaestabCorrectFormat();
            });

            it(`Checks UKPRN value is present and numeric for a ${description}`, () => {
                schoolsPage
                    .checkUkprnValuePresent()
                    .checkUkprnIsNumeric();
            });

            it(`Checks all reference number values are present and correctly formatted for a ${description}`, () => {
                schoolsPage
                    .checkAllReferenceNumbersDataPresent();
            });
        });
    });

    describe("Comprehensive business rule compliance", () => {
        referenceNumbersTestData.forEach(({ description, urn }) => {
            it(`Validates complete page compliance with business rules for a ${description}`, () => {
                cy.visit(`/schools/overview/referencenumbers?urn=${urn}`);

                schoolsPage
                    .checkReferenceNumbersPageCompleteWithBusinessRules();
            });
        });
    });

    describe("Testing school vs academy reference numbers differences", () => {
        it('Checks the page loads correctly with all business rules for a local authority maintained school', () => {
            const schoolData = referenceNumbersTestData.find(data => data.description === 'local authority maintained school');
            if (!schoolData) {
                throw new Error('Local authority maintained school test data not found');
            }
            cy.visit(`/schools/overview/referencenumbers?urn=${schoolData.urn}`);

            schoolsPage
                .checkOverviewPageNamePresent()
                .checkSchoolReferenceNumbersHeaderPresent()
                .checkReferenceNumbersPageCompleteWithBusinessRules();
        });

        it('Checks the page loads correctly with all business rules for a school in a trust', () => {
            const schoolData = referenceNumbersTestData.find(data => data.description === 'school in a trust');
            if (!schoolData) {
                throw new Error('School in a trust test data not found');
            }
            cy.visit(`/schools/overview/referencenumbers?urn=${schoolData.urn}`);

            schoolsPage
                .checkOverviewPageNamePresent()
                .checkSchoolReferenceNumbersHeaderPresent()
                .checkReferenceNumbersPageCompleteWithBusinessRules();
        });
    });

    describe("User story validation: Background information gathering", () => {
        referenceNumbersTestData.forEach(({ description, urn }) => {
            it(`Provides key reference number information in one place for ${description} to support briefing creation`, () => {
                cy.visit(`/schools/overview/referencenumbers?urn=${urn}`);

                // Validate all required reference numbers are available for briefing/reporting
                schoolsPage
                    .checkUrnValuePresent()
                    .checkLaestabValuePresent()
                    .checkUkprnValuePresent()
                    .checkLaestabCorrectFormat();
            });
        });
    });

});
