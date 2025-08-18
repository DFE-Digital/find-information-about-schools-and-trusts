import schoolOfstedPage from "../../../../pages/schools/schoolOfstedPage";
import navigation from "../../../../pages/navigation";
import { testSchoolData } from "../../../../support/test-data-store";

describe("School Ofsted Safeguarding and Concerns page", () => {

    describe("Basic page functionality", () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/safeguardingandconcerns?urn=${urn}`);
            });

            it(`Should display correct page elements for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkOfstedSafeguardingAndConcernsPageHeaderPresent()
                    .checkSafeguardingAndConcernsTablePresent();

                navigation
                    .checkPageNameBreadcrumbPresent("Ofsted");
            });

            it(`Should display core safeguarding data for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkAllSafeguardingDataPresent()
                    .checkAllSafeguardingDataValid();
            });
        });
    });

    describe("Academy-specific elements", () => {
        const academyData = testSchoolData.find(data => data.schoolOrAcademy === 'academy');

        if (academyData) {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/safeguardingandconcerns?urn=${academyData.urn}`);
            });

            it(`Should display before or after joining data for ${academyData.typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkBeforeOrAfterJoiningRowPresent()
                    .checkBeforeOrAfterJoiningLabelPresent()
                    .checkBeforeOrAfterJoiningValuePresent()
                    .checkBeforeOrAfterJoiningValueValid();
            });
        }
    });

    describe("School-specific elements", () => {
        const schoolData = testSchoolData.find(data => data.schoolOrAcademy === 'school');

        if (schoolData) {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/safeguardingandconcerns?urn=${schoolData.urn}`);
            });

            it(`Should NOT display before or after joining data for ${schoolData.typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkBeforeOrAfterJoiningRowNotPresent();
            });
        }
    });



    describe("Interactive elements", () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/safeguardingandconcerns?urn=${urn}`);
            });

            it(`Should expand 'What this information means' details for ${typeOfSchool}`, () => {
                // User story: Detail component should provide detailed explanation
                schoolOfstedPage
                    .checkWhatThisInformationMeansDetailsPresent()
                    .clickWhatThisInformationMeansDetails()
                    .checkWhatThisInformationMeansDetailsIsOpen();
            });
        });
    });
});
