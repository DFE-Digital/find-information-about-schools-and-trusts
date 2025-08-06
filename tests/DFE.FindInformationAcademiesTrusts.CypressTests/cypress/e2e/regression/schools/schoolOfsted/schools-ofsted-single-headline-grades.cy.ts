import schoolOfstedPage from "../../../../pages/schools/schoolOfstedPage";
import navigation from "../../../../pages/navigation";
import { testSchoolData } from "../../../../support/test-data-store";

describe("Testing the School Ofsted Single Headline Grades page", () => {

    describe("Basic page functionality and navigation", () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/singleheadlinegrades?urn=${urn}`);
            });

            it(`Checks the correct Ofsted single headline grades subpage header is present for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkOfstedSingleHeadlineGradesPageHeaderPresent();
            });

            it(`Checks the breadcrumb shows the correct page name for ${typeOfSchool}`, () => {
                navigation
                    .checkPageNameBreadcrumbPresent("Ofsted");
            });

            it(`Checks the correct Ofsted single headline grades table headers are present for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkSingleHeadlineGradesTableHeadersPresent();
            });
        });
    });

    describe("Inspection data components and validation", () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/singleheadlinegrades?urn=${urn}`);
            });

            it(`Checks that the current full inspection section is present for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkCurrentFullInspectionPresent()
                    .checkCurrentFullInspectionData();
            });

            it(`Checks that the previous full inspection section is present for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkPreviousFullInspectionPresent()
                    .checkPreviousFullInspectionData();
            });

            it(`Checks that the Ofsted grades are valid for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkCurrentFullInspectionGradeValid()
                    .checkPreviousFullInspectionGradeValid()
                    .checkRecentShortInspectionGradeIfExists();
            });

            it(`Checks that the inspection dates are valid for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkCurrentFullInspectionDateValid()
                    .checkPreviousFullInspectionDateValid()
                    .checkRecentShortInspectionDateIfExists();
            });
        });
    });

    describe("Details sections and interactive elements", () => {
        testSchoolData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/singleheadlinegrades?urn=${urn}`);
            });

            it(`Checks that the 'Why single headline grade might not be available' details section is present for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkWhySingleHeadlineNotAvailableDetailsPresent();
            });

            it(`Checks that the 'Why short inspection data might not be available' details section is present for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkWhyShortInspectionNotAvailableDetailsPresent();
            });

            it(`Checks that clicking 'Why single headline grade might not be available' details expands the content for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .clickWhySingleHeadlineNotAvailableDetails()
                    .checkWhySingleHeadlineDetailsIsOpen();
            });

            it(`Checks that clicking 'Why short inspection data might not be available' details expands the content for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .clickWhyShortInspectionNotAvailableDetails()
                    .checkWhyShortInspectionDetailsIsOpen();
            });

            it(`Checks that the inspection reports link is present for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkInspectionReportsLinkPresent();
            });

            it(`Checks that the 'See the inspection reports at Ofsted' link has the correct href for ${typeOfSchool}`, () => {
                schoolOfstedPage
                    .checkInspectionReportsLinkValid();
            });
        });
    });

    describe("Academy-specific elements", () => {
        const academyData = testSchoolData.find(data => data.schoolOrAcademy === 'academy');

        if (academyData) {
            it(`Checks that the academy joined trust text is present for ${academyData.typeOfSchool}`, () => {
                cy.visit(`/schools/ofsted/singleheadlinegrades?urn=${academyData.urn}`);

                schoolOfstedPage
                    .checkJoinedTrustTextPresent();
            });
        }
    });

    // Additional tests for edge cases across all school types
    testSchoolData.forEach(({ typeOfSchool, urn, schoolOrAcademy }) => {
        describe(`Testing data validation for ${typeOfSchool} (${schoolOrAcademy})`, () => {
            beforeEach(() => {
                cy.visit(`/schools/ofsted/singleheadlinegrades?urn=${urn}`);
            });

            it(`Should handle 'Not available' data states appropriately for ${typeOfSchool}`, () => {
                // Check that current and previous inspections are always present (even if data shows "Not available")
                schoolOfstedPage
                    .checkCurrentFullInspectionPresent()
                    .checkPreviousFullInspectionPresent();

                // Validate that any visible data has proper formatting
                schoolOfstedPage
                    .checkCurrentFullInspectionGradeValid()
                    .checkPreviousFullInspectionGradeValid()
                    .checkCurrentFullInspectionDateValid()
                    .checkPreviousFullInspectionDateValid();
            });
        });
    });

}); 
