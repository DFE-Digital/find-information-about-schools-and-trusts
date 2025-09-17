import schoolPupilsPage from "../../../../pages/schools/schoolPupilsPage";
import schoolsPage from "../../../../pages/schools/schoolsPage";
import { testSchoolData } from "../../../../support/test-data-store";

describe("School Pupils Pages", () => {

    describe('Population page', () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            it(`Checks population page basics for ${typeOfSchool} on urn ${urn}`, () => {
                cy.visit(`/schools/pupils/population?urn=${urn}`);

                schoolPupilsPage
                    .checkPupilsPageNamePresent()
                    .checkPopulationSubpageHeaderPresent()
                    .checkPupilsNavTabsPresent();

                schoolsPage
                    .checkCorrectSchoolTypePresent();
            });

            it(`Checks population table and data for ${typeOfSchool} on urn ${urn}`, () => {
                cy.visit(`/schools/pupils/population?urn=${urn}`);

                schoolPupilsPage
                    .checkPopulationTablePresent()
                    .checkPopulationDataRowsPresent()
                    .verifyPopulationDataIntegrity();
            });
        });

        it(`Verifies population table has 5 data rows`, () => {
            cy.visit(`/schools/pupils/population?urn=${testSchoolData[0].urn}`);
            schoolPupilsPage.elements.population.censusTable()
                .find('tbody tr')
                .should('have.length', 5);
        });

        it(`Checks spring census information is displayed`, () => {
            cy.visit(`/schools/pupils/population?urn=${testSchoolData[0].urn}`);
            schoolPupilsPage
                .checkPopulationCensusTextPresent();
        });
    });

    describe('Attendance page', () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            it(`Checks attendance page basics for ${typeOfSchool} on urn ${urn}`, () => {
                cy.visit(`/schools/pupils/attendance?urn=${urn}`);

                schoolPupilsPage
                    .checkPupilsPageNamePresent()
                    .checkAttendanceSubpageHeaderPresent()
                    .checkPupilsNavTabsPresent();

                schoolsPage
                    .checkCorrectSchoolTypePresent();
            });

            it(`Checks attendance table and data for ${typeOfSchool} on urn ${urn}`, () => {
                cy.visit(`/schools/pupils/attendance?urn=${urn}`);

                schoolPupilsPage
                    .checkAttendanceTablePresent()
                    .checkAttendanceDataRowsPresent()
                    .verifyAttendanceDataIntegrity();
            });
        });

        it(`Verifies attendance table has 2 data rows`, () => {
            cy.visit(`/schools/pupils/attendance?urn=${testSchoolData[0].urn}`);
            schoolPupilsPage.elements.attendance.censusTable()
                .find('tbody tr')
                .should('have.length', 2);
        });

        it(`Checks autumn census information is displayed`, () => {
            cy.visit(`/schools/pupils/attendance?urn=${testSchoolData[0].urn}`);
            schoolPupilsPage
                .checkAttendanceCensusTextPresent();
        });
    });

});
