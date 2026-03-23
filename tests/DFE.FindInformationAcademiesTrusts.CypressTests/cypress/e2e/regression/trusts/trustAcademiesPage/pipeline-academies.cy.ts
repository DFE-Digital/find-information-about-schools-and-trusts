import commonPage from "../../../../pages/commonPage";
import pipelineAcademiesPage from "../../../../pages/trusts/pipelineAcademiesPage";
import { testPreAdvisoryData, testPostAdvisoryData, testFreeSchoolsData } from "../../../../support/test-data-store";

describe("Testing the Pipeline academies pages", () => {

    describe(`On the Pre decision page for a trust`, () => {
        testPreAdvisoryData.forEach(({ uid }) => {
            beforeEach(() => {
                cy.visit(`/trusts/academies/pipeline/pre-decision?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Pre decision - Pipeline academies - Academies - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Pre decision Pipeline academies subpage header is present", () => {
                pipelineAcademiesPage
                    .checkPreDecisionPageHeaderPresent();
            });

            it("Checks the correct Pipeline academies Pre decision table headers are present", () => {
                pipelineAcademiesPage
                    .checkPreDecisionTableHeadersPresent();
            });

            it("Checks the Pipeline academies Pre decision page sorting", () => {
                pipelineAcademiesPage
                    .checkPreDecisionTableSorting();
            });

            it("Checks the Pipeline academies Pre decision project type", () => {
                pipelineAcademiesPage
                    .checkPreDecisionCorrectProjectTypePresent();
            });

            it("Checks the Pipeline academies Pre decision Proposed conversion or transfer date", () => {
                pipelineAcademiesPage
                    .checkPreDecisionCorrectConversionTransferDatePresent();
            });

            it('checks that each academy name is a link to the academy details page with the correct URN', () => {
                pipelineAcademiesPage.checkSchoolNamesAreCorrectLinksOnPreDecisionPage();
            });
        });
    });

    describe(`On the Post decision page for a trust`, () => {
        testPostAdvisoryData.forEach(({ uid }) => {
            beforeEach(() => {
                cy.visit(`/trusts/academies/pipeline/post-decision?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Post decision - Pipeline academies - Academies - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Post decision Pipeline academies subpage header is present", () => {
                pipelineAcademiesPage
                    .checkPostDecisionPageHeaderPresent();
            });

            it("Checks the correct Pipeline academies Post decision table headers are present", () => {
                pipelineAcademiesPage
                    .checkPostDecisionTableHeadersPresent();
            });

            it("Checks the Pipeline academies Post decision page sorting", () => {
                pipelineAcademiesPage
                    .checkPostDecisionTableHeadersPresent();
            });

            it("Checks the Pipeline academies Post decision page sorting", () => {
                pipelineAcademiesPage
                    .checkPostDecisionTableSorting();
            });

            it("Checks the Pipeline academies Post decision project type", () => {
                pipelineAcademiesPage
                    .checkPostDecisionCorrectProjectTypePresent();
            });

            it("Checks the Pipeline academies Post decision Proposed conversion or transfer date", () => {
                pipelineAcademiesPage
                    .checkPostDecisionCorrectConversionTransferDatePresent();
            });

        });

        it('checks that each academy name is a link to the academy details page with the correct URN', () => {
            pipelineAcademiesPage.checkSchoolNamesAreCorrectLinksOnPostDecisionPage();
        });
    });

    describe(`On the Free schools page`, () => {
        testFreeSchoolsData.forEach(({ uid }) => {
            beforeEach(() => {
                cy.visit(`/trusts/academies/pipeline/free-schools?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Free schools - Pipeline academies - Academies - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Free schools Pipeline academies subpage header is present", () => {
                pipelineAcademiesPage
                    .checkFreeSchoolsPageHeaderPresent();
            });

            it("Checks the correct Pipeline academies Free schools table headers are present", () => {
                pipelineAcademiesPage
                    .checkFreeSchoolsTableHeadersPresent();
            });

            it("Checks the Pipeline academies Free schools page sorting", () => {
                pipelineAcademiesPage
                    .checkFreeSchoolsTableHeadersPresent();
            });

            it("Checks the Pipeline academies Free schools page sorting", () => {
                pipelineAcademiesPage
                    .checkFreeSchoolsTableSorting();
            });

            it("Checks the Pipeline academies Free schools project type", () => {
                pipelineAcademiesPage
                    .checkFreeSchoolsCorrectProjectTypePresent();
            });

            it("Checks the Pipeline academies Free schools Proposed conversion or transfer date", () => {
                pipelineAcademiesPage
                    .checkFreeSchoolsCorrectProvisionalOpenDatePresent();
            });

            it('checks that each academy name does not have a link as URNs are not available', () => {
                pipelineAcademiesPage.checkSchoolNamesAreNotLinksOnFreeSchools();
            });
        });
    });

    describe(`On the pages with no pipeline academy data under them`, () => {

        it("Checks the Pipeline academies Pre decision page when an academy does not exist under it to ensure the correct message is displayed", () => {
            cy.visit(`/trusts/academies/pipeline/pre-decision?uid=5712`);
            pipelineAcademiesPage
                .checkPreDecisionNoAcademyPresent();
        });

        it("Checks the Pipeline academies Post decision page when an academy does not exist under it to ensure the correct message is displayed", () => {
            cy.visit(`/trusts/academies/pipeline/post-decision?uid=5712`);
            pipelineAcademiesPage
                .checkPostDecisionNoAcademyPresent();
        });

        it("Checks the Pipeline academies Free schools page when an academy does not exist under it to ensure the correct message is displayed", () => {
            cy.visit(`/trusts/academies/pipeline/free-schools?uid=5712`);
            pipelineAcademiesPage
                .checkFreeSchoolsNoAcademyPresent();
        });
    });

    describe("Testing a trust that has no pipeline data within it to ensure the issue of a 500 page appearing does not happen", () => {
        beforeEach(() => {
            commonPage
                .interceptAndVerifyNo500Errors();
        });

        it(`Should have no 500 error on the Pre decision page`, () => {
            cy.visit(`/trusts/academies/pipeline/pre-decision?uid=5712`);
        });

        it(`Should have no 500 error on the Post decision page`, () => {
            cy.visit(`/trusts/academies/pipeline/post-decision?uid=5712`);
        });

        it(`Should have no 500 error on the free schools page`, () => {
            cy.visit(`/trusts/academies/pipeline/free-schools?uid=5712`);
        });
    });

});
