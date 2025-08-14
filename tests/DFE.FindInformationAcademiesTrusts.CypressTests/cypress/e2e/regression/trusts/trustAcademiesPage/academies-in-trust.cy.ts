import academiesInTrustPage from "../../../../pages/trusts/academiesInTrustPage";
import commonPage from "../../../../pages/commonPage";
import { testTrustData } from "../../../../support/test-data-store";

describe("Testing the components of the Academies page", () => {

    describe("Details tab", () => {
        beforeEach(() => {
            cy.visit('/trusts/academies/in-trust/details?uid=5712');
        });

        it("Checks the browser title is correct", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Details - In this trust - Academies - {trustName} - Find information about schools and trusts');
        });

        it("Checks the correct details page headers are present", () => {
            academiesInTrustPage
                .checkDetailsHeadersPresent();
        });

        it("Checks that the correct school types are present", () => {
            academiesInTrustPage
                .checkSchoolTypesOnDetailsTable();
        });

        it("Checks that a different and larger trusts correct school types are present", () => {
            cy.visit('/trusts/academies/in-trust/details?uid=5143');
            academiesInTrustPage
                .checkSchoolTypesOnDetailsTable();
        });

        it("Checks that the removed England/Wales is not present on the trust details page", () => {
            academiesInTrustPage
                .checkEnglandWalesIdentifierNotPresent();
        });

        it("Checks the detail page sorting", () => {
            cy.visit('/trusts/academies/in-trust/details?uid=5143');
            academiesInTrustPage
                .checkTrustDetailsSorting();
        });

        it("Checks the trust joined date is present and valid", () => {
            academiesInTrustPage
                .checkTrustJoinedDatePresentAndValid();
        });

        it('should match the academy count in the sidebar with the actual table row count on the Details page', () => {
            academiesInTrustPage.getAcademyCountFromSidebar().then(expectedCount => {
                academiesInTrustPage.getTableRowCountOnDetailsPage().should('eq', expectedCount);
            });
        });

        it('should match the academy count in the sidebar with the actual table row count on the Details page after visiting', () => {
            cy.visit('/trusts/academies/in-trust/details?uid=5143');
            academiesInTrustPage.getAcademyCountFromSidebar().then(expectedCount => {
                academiesInTrustPage.getTableRowCountOnDetailsPage().should('eq', expectedCount);
            });
        });
        
        it('checks that each academy name is a link to the academy details page with the correct URN', () => {
            academiesInTrustPage.checkSchoolNamesAreCorrectLinksOnDetailsPage(); 
        });
    });


    describe("Pupil numbers tab", () => {
        beforeEach(() => {
            cy.visit('/trusts/academies/in-trust/pupil-numbers?uid=5712');
        });

        it("Checks the browser title is correct", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Pupil numbers - In this trust - Academies - {trustName} - Find information about schools and trusts');
        });

        it("Checks the correct Pupil numbers page headers are present", () => {
            academiesInTrustPage
                .checkPupilNumbersHeadersPresent();
        });

        it("Checks the Pupil numbers page sorting", () => {
            academiesInTrustPage
                .checkPupilNumbersSorting();
        });

        it('should match the academy count in the sidebar with the actual table row count on the Pupil numbers page', () => {
            academiesInTrustPage.getAcademyCountFromSidebar().then(expectedCount => {
                academiesInTrustPage.getTableRowCountOnPupilNumbersPage().should('eq', expectedCount);
            });
        });

        it('should match the academy count in the sidebar with the actual table row count on the Pupil numbers page after visiting', () => {
            cy.visit('/trusts/academies/in-trust/pupil-numbers?uid=5143');
            academiesInTrustPage.getAcademyCountFromSidebar().then(expectedCount => {
                academiesInTrustPage.getTableRowCountOnPupilNumbersPage().should('eq', expectedCount);
            });
        });

        it("Checks the Pupil numbers page sorting on a larger trust", () => {
            cy.visit('/trusts/academies/in-trust/pupil-numbers?uid=5143');
            academiesInTrustPage
                .checkPupilNumbersSorting();
        });

        it("Checks the valid phase types are present on an age range", () => {
            academiesInTrustPage
                .checkCorrectPhaseTypePresent();
        });

        it('checks that each academy name is a link to the academy details page with the correct URN', () => {
            academiesInTrustPage.checkSchoolNamesAreCorrectLinksOnPupilNumbersPage();
        });
    });

    describe("Free school meals", () => {
        beforeEach(() => {
            cy.visit('/trusts/academies/in-trust/free-school-meals?uid=5143');
        });

        it("Checks the browser title is correct", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Free school meals - In this trust - Academies - {trustName} - Find information about schools and trusts');
        });

        it("Checks the correct Free school meals page headers are present", () => {
            academiesInTrustPage
                .checkFreeSchoolMealsHeadersPresent();
        });

        it("Checks the Free school meals page sorting", () => {
            academiesInTrustPage
                .checkFreeSchoolMealsSorting();
        });

        it('should match the academy count in the sidebar with the actual table row count on the Free school meals page', () => {
            academiesInTrustPage.getAcademyCountFromSidebar().then(expectedCount => {
                academiesInTrustPage.getTableRowCountOnFreeSchoolMealsPage().should('eq', expectedCount);
            });
        });

        it('should match the academy count in the sidebar with the actual table row count on the Free school meals page after visiting', () => {
            cy.visit('/trusts/academies/in-trust/free-school-meals?uid=5143');
            academiesInTrustPage.getAcademyCountFromSidebar().then(expectedCount => {
                academiesInTrustPage.getTableRowCountOnFreeSchoolMealsPage().should('eq', expectedCount);
            });
        });

        it("Checks the Free school meals page sorting on a larger trust", () => {
            cy.visit('/trusts/academies/in-trust/free-school-meals?uid=5143');
            academiesInTrustPage
                .checkFreeSchoolMealsSorting();
        });

        it('checks that each academy name is a link to the academy details page with the correct URN', () => {
            academiesInTrustPage.checkSchoolNamesAreCorrectLinksOnFreeSchoolMealsPage();
        });
    });

    describe("Testing a trust that has no academies within it to ensure the issue of a 500 page appearing does not happen", () => {
        beforeEach(() => {
            commonPage.interceptAndVerifyNo500Errors();
        });

        ['/trusts/academies/in-trust/details?uid=17728', '/trusts/academies/in-trust/pupil-numbers?uid=17728', '/trusts/academies/in-trust/free-school-meals?uid=17728'].forEach((url) => {
            it(`Should have no 500 error on ${url}`, () => {
                cy.visit(url);
            });
        });
    });

    describe("Testing that no unknown entries are found for an academies various tables/pages", () => {
        testTrustData.forEach(({ typeOfTrust, uid }) => {

            [`/trusts/academies/in-trust/details?uid=${uid}`, `/trusts/academies/in-trust/pupil-numbers?uid=${uid}`, `/trusts/academies/in-trust/free-school-meals?uid=${uid}`].forEach((url) => {
                it(`Should have no unknown entries on ${url} for a ${typeOfTrust}`, () => {
                    cy.visit(url);
                    commonPage
                        .checkNoUnknownEntries();
                });
            });
        });
    });
});
