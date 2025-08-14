import commonPage from "../../../../pages/commonPage";
import pipelineAcademiesPage from "../../../../pages/trusts/pipelineAcademiesPage";
import dataDownload from "../../../../pages/trusts/dataDownload";
import { testPreAdvisoryData, testPostAdvisoryData, testFreeSchoolsData } from "../../../../support/test-data-store";

describe("Testing the Pipeline academies pages", () => {

    describe(`On the Pre advisory board page for a trust`, () => {
        testPreAdvisoryData.forEach(({ uid }) => {
            beforeEach(() => {
                cy.visit(`/trusts/academies/pipeline/pre-advisory-board?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Pre advisory board - Pipeline academies - Academies - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Pre advisory board Pipeline academies subpage header is present", () => {
                pipelineAcademiesPage
                    .checkPreAdvisoryPageHeaderPresent();
            });

            it("Checks the correct Pipeline academies Pre advisory board table headers are present", () => {
                pipelineAcademiesPage
                    .checkPreAdvisoryTableHeadersPresent();
            });

            it("Checks the Pipeline academies Pre advisory page sorting", () => {
                pipelineAcademiesPage
                    .checkPreAdvisoryTableSorting();
            });

            it("Checks the Pipeline academies Pre advisory project type", () => {
                pipelineAcademiesPage
                    .checkPreAdvisoryCorrectProjectTypePresent();
            });

            it("Checks the Pipeline academies Pre advisory Proposed conversion or transfer date", () => {
                pipelineAcademiesPage
                    .checkPreAdvisoryCorrectConversionTransferDatePresent();
            });

            // The download function is the same for every sub-page - the test is here because this is the landing page for pipeline academies
            it('should export pipeline academies data as an xlsx and verify it has downloaded and has content', () => {
                pipelineAcademiesPage
                    .clickDownloadButton();
                dataDownload
                    .checkFileDownloaded()
                    .checkFileHasContent()
                    .deleteDownloadedFile();
            });

            it('checks that each academy name is a link to the academy details page with the correct URN', () => {
                pipelineAcademiesPage.checkSchoolNamesAreCorrectLinksOnPreAdvisoryPage();
            });
        });
    });

    describe(`On the Post advisory board page for`, () => {
        testPostAdvisoryData.forEach(({ uid }) => {
            beforeEach(() => {
                cy.visit(`/trusts/academies/pipeline/post-advisory-board?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Post advisory board - Pipeline academies - Academies - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Post advisory board Pipeline academies subpage header is present", () => {
                pipelineAcademiesPage
                    .checkPostAdvisoryPageHeaderPresent();
            });

            it("Checks the correct Pipeline academies Post advisory board table headers are present", () => {
                pipelineAcademiesPage
                    .checkPostAdvisoryTableHeadersPresent();
            });

            it("Checks the Pipeline academies Post advisory page sorting", () => {
                pipelineAcademiesPage
                    .checkPostAdvisoryTableHeadersPresent();
            });

            it("Checks the Pipeline academies Post advisory page sorting", () => {
                pipelineAcademiesPage
                    .checkPostAdvisoryTableSorting();
            });

            it("Checks the Pipeline academies Post advisory project type", () => {
                pipelineAcademiesPage
                    .checkPostAdvisoryCorrectProjectTypePresent();
            });

            it("Checks the Pipeline academies Post advisory Proposed conversion or transfer date", () => {
                pipelineAcademiesPage
                    .checkPostAdvisoryCorrectConversionTransferDatePresent();
            });

        });

        it('checks that each academy name is a link to the academy details page with the correct URN', () => {
            pipelineAcademiesPage.checkSchoolNamesAreCorrectLinksOnPostAdvisoryPage();
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

        it("Checks the Pipeline academies Pre advisory page when an academy does not exist under it to ensure the correct message is displayed", () => {
            cy.visit(`/trusts/academies/pipeline/pre-advisory-board?uid=5712`);
            pipelineAcademiesPage
                .checkPreAdvisoryNoAcademyPresent();
        });

        it("Checks the Pipeline academies Post advisory page when an academy does not exist under it to ensure the correct message is displayed", () => {
            cy.visit(`/trusts/academies/pipeline/post-advisory-board?uid=5712`);
            pipelineAcademiesPage
                .checkPostAdvisoryNoAcademyPresent();
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

        it(`Should have no 500 error on the Pre advisory page`, () => {
            cy.visit(`/trusts/academies/pipeline/pre-advisory-board?uid=5712`);
        });

        it(`Should have no 500 error on the Post advisory page`, () => {
            cy.visit(`/trusts/academies/pipeline/post-advisory-board?uid=5712`);
        });

        it(`Should have no 500 error on the free schools page`, () => {
            cy.visit(`/trusts/academies/pipeline/free-schools?uid=5712`);
        });
    });

});
