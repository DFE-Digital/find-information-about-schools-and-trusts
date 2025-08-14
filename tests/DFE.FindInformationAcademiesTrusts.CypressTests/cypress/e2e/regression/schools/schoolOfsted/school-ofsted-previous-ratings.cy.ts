import navigation from "../../../../pages/navigation";
import schoolOfstedRatingsPage from "../../../../pages/schools/schoolOfstedRatingsPage";
import { testSchoolData } from "../../../../support/test-data-store";

describe("Testing the School Ofsted Previous ratings page", () => {

    describe("Basic page functionality and navigation", () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/previousratings?urn=${urn}`);
            });

            it(`Checks the correct Ofsted previous ratings subpage header is present for ${typeOfSchool}`, () => {
                schoolOfstedRatingsPage
                    .checkOfstedPreviousRatingsPageHeaderPresent();
            });

            it(`Checks the breadcrumb shows the correct page name for ${typeOfSchool}`, () => {
                navigation
                    .checkPageNameBreadcrumbPresent("Ofsted");
            });

        });
    });

        describe("Ratings data components and validation", () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/previousratings?urn=${urn}`);
            });

            it(`Checks that the quality of education section is present for ${typeOfSchool}`, () => {
                schoolOfstedRatingsPage
                    .checkQualityOfEducationSectionPresent();
            });

            it(`Check that the behaviour and attitudes section is present for ${typeOfSchool}`, () => {
                schoolOfstedRatingsPage
                    .checkBehaviourAndAttitudesSectionPresent();
            });

            it(`Checks that the personal development section is present for ${typeOfSchool}`, () => {
                schoolOfstedRatingsPage
                    .checkPersonalDevelopmentSectionPresent();
            });

            it(`Checks that the leadership and management section is present for ${typeOfSchool}`, () => {
                schoolOfstedRatingsPage
                    .checkLeadershipAndManagementSectionPresent();
            });

            it(`Checks that early years provision section is present for ${typeOfSchool}`, () => {
                schoolOfstedRatingsPage
                    .checkEarlyYearsProvisionSectionPresent();
            });

            it(`Checks that the sixth form provision section is present for ${typeOfSchool}`, () => {
                schoolOfstedRatingsPage
                    .checkSixthFormProvisionSectionPresent();
            });
        });
    });

        describe("Academy-specific elements", () => {
        const academyData = testSchoolData.find(data => data.schoolOrAcademy === 'academy');

        if (academyData) {
            it(`Checks that the before or after joining row is present for ${academyData.typeOfSchool}`, () => {
                cy.visit(`/schools/ofsted/previousratings?urn=${academyData.urn}`);

                schoolOfstedRatingsPage
                    .checkBeforeOrAfterJoiningRowPresent();
            });
        }
    });

});