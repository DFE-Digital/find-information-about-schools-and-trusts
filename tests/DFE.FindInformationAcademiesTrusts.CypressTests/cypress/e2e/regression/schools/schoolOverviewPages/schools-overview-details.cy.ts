import schoolsPage from "../../../../pages/schools/schoolsPage";
import { testSchoolData } from "../../../../support/test-data-store";

describe("Overview details page", () => {

    testSchoolData.forEach(({ typeOfSchool, urn }) => {
        it(`Checks the page name is correct for a ${typeOfSchool} on the urn ${urn}`, () => {
            cy.visit(`/schools/overview/details?urn=${urn}`);
            schoolsPage
                .checkOverviewPageNamePresent();
        });

        it(`Checks the information from other services components are present for a ${typeOfSchool} on the urn ${urn}`, () => {
            cy.visit(`/schools/overview/details?urn=${urn}`);
            schoolsPage
                .checkDetailsOtherServicesItemsPresent();
        });
    });

    it(`Checks the tab name is present and correct for a school`, () => {
        cy.visit(`/schools/overview/details?urn=${testSchoolData[0].urn}`);
        schoolsPage
            .checkSchoolDetailsTabCorrect();
    });

    it(`Checks the tab name is present and correct for a academy`, () => {
        cy.visit(`/schools/overview/details?urn=${testSchoolData[1].urn}`);
        schoolsPage
            .checkAcademyDetailsTabCorrect();
    });

    describe('Testing the school/academy details page', () => {
        it('Checks the school header is present on a school type', () => {
            cy.visit(`/schools/overview/details?urn=${testSchoolData[0].urn}`);
            schoolsPage
                .checkSchoolDetailsHeaderPresent();
        });

        it('Checks the school header is present on a academy type', () => {
            cy.visit(`/schools/overview/details?urn=${testSchoolData[1].urn}`);
            schoolsPage
                .checkAcademyDetailsHeaderPresent();
        });

        it(`Checks the details page detail data components are present for a School`, () => {
            cy.visit(`/schools/overview/details?urn=${testSchoolData[0].urn}`);
            schoolsPage
                .checkDetailsSchoolDataItemsPresent();
        });

        it(`Checks the details page detail data components are present for a School`, () => {
            cy.visit(`/schools/overview/details?urn=${testSchoolData[1].urn}`);
            schoolsPage
                .checkDetailsAcademyDataItemsPresent();
        });

        it(`Checks the academy details page detail data components are not present for a School`, () => {
            cy.visit(`/schools/overview/details?urn=${testSchoolData[0].urn}`);
            schoolsPage
                .checkDetailsAcademyDataItemsNotPresent();
        });
    });

    const schoolInSATs = [
        { urn: 140884, type: 'school in SAT' }
    ];

    schoolInSATs.forEach(({ urn, type }) => {
        context(`${type} should have trust details`, () => {

            it('Checks the school header is present on a school type', () => {
                cy.visit(`/schools/overview/details?urn=${urn}`);
                schoolsPage
                    .checkSchoolDetailsHeaderPresent()
                    .checkDetailsAcademyDataItemsPresent();
            });

        });
    });
});
