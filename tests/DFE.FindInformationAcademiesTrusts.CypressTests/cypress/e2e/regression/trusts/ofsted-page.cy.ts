import ofstedPage from "../../../pages/trusts/ofstedPage";
import navigation from "../../../pages/navigation";
import dataDownload from "../../../pages/trusts/dataDownload";
import commonPage from "../../../pages/commonPage";
import { TestDataStore, testTrustOfstedData, testOfstedWithDataUid } from "../../../support/test-data-store";

describe("Testing the Ofsted page and its subpages ", () => {

    describe(`Testing the single headline grades page `, () => {
        beforeEach(() => {
            cy.task('checkForFiles', 'cypress/downloads').then((files) => {
                if (files) {
                    cy.task('clearDownloads', 'cypress/downloads');
                }
            });
            cy.visit(`/trusts/ofsted/single-headline-grades?uid=5527`);
        });

        it("Checks the correct Ofsted single headline grades subpage header is present", () => {

            ofstedPage
                .checkOfstedSHGPageHeaderPresent();
        });

        it("Checks the browser title is correct", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Single headline grades - Ofsted - {trustName} - Find information about schools and trusts');
        });

        it("Checks the breadcrumb shows the correct page name", () => {
            navigation
                .checkPageNameBreadcrumbPresent("Ofsted");
        });

        it("Checks the correct Ofsted single headline grades table headers are present", () => {
            ofstedPage
                .checkOfstedSHGTableHeadersPresent();
        });

        it("Checks the Ofsted current ratings page sorting", () => {
            ofstedPage
                .checkOfstedSHGSorting();
        });

        it("Checks that a trusts current and previous ratings correct judgement types are present", () => {
            ofstedPage
                .checkSHGCurrentSHGJudgementsPresent()
                .checkSHGPreviousSHGJudgementsPresent()
                .checkSHGCurrentSHGBeforeOrAfterPresent()
                .checkSHGPreviousSHGBeforeOrAfterPresent()
                .checkSHGHasRecentShortInspectionPresent();
        });

        it("Checks that the single headline grades dates are within the correct parameters", () => {
            ofstedPage
                .checkSHGDateJoinedPresent()
                .checkSHGDateOfCurrentInspectionPresent()
                .checkSHGDateOfPreviousInspectionPresent();
        });

        it("Checks that the 'Where to find short inspection data' details section is present", () => {
            ofstedPage
                .checkWhereToFindShortInspectionDataDetailsPresent();
        });

        it("Checks that the 'Why single headline grade might not be available' details section is present", () => {
            ofstedPage
                .checkWhySingleHeadlineNotAvailableDetailsPresent();
        });

        it("Checks that clicking 'Where to find short inspection data' details expands the content", () => {
            ofstedPage
                .clickWhereToFindShortInspectionDataDetails()
                .checkWhereToFindShortInspectionDataDetailsIsOpen();
        });

        it("Checks that clicking 'Why single headline grade might not be available' details expands the content", () => {
            ofstedPage
                .clickWhySingleHeadlineNotAvailableDetails()
                .checkWhySingleHeadlineNotAvailableDetailsIsOpen();
        });

        it('should export academies data as an xlsx and verify it has downloaded and has content', () => {
            ofstedPage
                .clickDownloadButton();
            dataDownload
                .checkFileDownloaded()
                .checkFileHasContent()
                .deleteDownloadedFile();
        });

    });

    describe("Testing the Ofsted current ratings page ", () => {
        beforeEach(() => {
            cy.visit(`/trusts/ofsted/current-ratings?uid=${testOfstedWithDataUid}`);

            cy.task('checkForFiles', 'cypress/downloads').then((files) => {
                if (files) {
                    cy.task('clearDownloads', 'cypress/downloads');
                }
            });
        });

        it("Checks the correct Ofsted current ratings subpage header is present", () => {
            ofstedPage
                .checkOfstedCurrentRatingsPageHeaderPresent();
        });

        it("Checks the browser title is correct", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Current ratings - Ofsted - {trustName} - Find information about schools and trusts');
        });

        it("Checks the breadcrumb shows the correct page name", () => {
            navigation
                .checkPageNameBreadcrumbPresent("Ofsted");
        });

        it("Checks the correct Ofsted current ratings table headers are present", () => {
            ofstedPage
                .checkOfstedCurrentRatingsTableHeadersPresent();
        });

        it("Checks the Ofsted current ratings page sorting", () => {
            ofstedPage
                .checkOfstedCurrentRatingsSorting();
        });

        it("Checks that a trusts current ratings correct judgement types are present", () => {
            ofstedPage
                .checkCurrentRatingsQualityOfEducationJudgementsPresent()
                .checkCurrentRatingsBehaviourAndAttitudesJudgementsPresent()
                .checkCurrentRatingsPersonalDevelopmentJudgementsPresent()
                .checkCurrentRatingsLeadershipAndManagementJudgementsPresent()
                .checkCurrentRatingsEarlyYearsProvisionJudgementsPresent()
                .checkCurrentRatingsSixthFormProvisionJudgementsPresent()
                .checkCurrentRatingsBeforeOrAfterJoiningJudgementsPresent();
        });

        it("Checks that a different trusts current ratings correct judgement types are present", () => {
            cy.visit('/trusts/ofsted/current-ratings?uid=5712');
            ofstedPage
                .checkCurrentRatingsQualityOfEducationJudgementsPresent()
                .checkCurrentRatingsBehaviourAndAttitudesJudgementsPresent()
                .checkCurrentRatingsPersonalDevelopmentJudgementsPresent()
                .checkCurrentRatingsLeadershipAndManagementJudgementsPresent()
                .checkCurrentRatingsEarlyYearsProvisionJudgementsPresent()
                .checkCurrentRatingsSixthFormProvisionJudgementsPresent()
                .checkCurrentRatingsBeforeOrAfterJoiningJudgementsPresent();
        });

        it('should export academies data as an xlsx and verify it has downloaded and has content', () => {
            ofstedPage
                .clickDownloadButton();
            dataDownload
                .checkFileDownloaded()
                .checkFileHasContent()
                .deleteDownloadedFile();
        });
    });

    describe("Testing the Ofsted previous ratings page ", () => {
        beforeEach(() => {
            cy.visit('/trusts/ofsted/previous-ratings?uid=5143');

            cy.task('checkForFiles', 'cypress/downloads').then((files) => {
                if (files) {
                    cy.task('clearDownloads', 'cypress/downloads');
                }
            });
        });

        it("Checks the correct Ofsted Previous ratings subpage header is present", () => {
            ofstedPage
                .checkOfstedPreviousRatingsPageHeaderPresent();
        });

        it("Checks the browser title is correct", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Previous ratings - Ofsted - {trustName} - Find information about schools and trusts');
        });

        it("Checks the breadcrumb shows the correct page name", () => {
            navigation
                .checkPageNameBreadcrumbPresent("Ofsted");
        });

        it("Checks the correct Ofsted previous ratings headers are present", () => {
            ofstedPage
                .checkOfstedPreviousRatingsTableHeadersPresent();
        });

        it("Checks the Ofsted page previous ratings sorting", () => {
            ofstedPage
                .checkOfstedPreviousRatingsSorting();
        });

        it("Checks that a trusts previous ratings correct judgement types are present", () => {
            ofstedPage
                .checkPreviousRatingsQualityOfEducationJudgementsPresent()
                .checkPreviousRatingsBehaviourAndAttitudesJudgementsPresent()
                .checkPreviousRatingsPersonalDevelopmentJudgementsPresent()
                .checkPreviousRatingsLeadershipAndManagementJudgementsPresent()
                .checkPreviousRatingsEarlyYearsProvisionJudgementsPresent()
                .checkPreviousRatingsSixthFormProvisionJudgementsPresent()
                .checkPreviousRatingsBeforeOrAfterJoiningJudgementsPresent();
        });

        it("Checks that a different trusts previous ratings correct judgement types are present", () => {
            cy.visit('/trusts/ofsted/previous-ratings?uid=5712');
            ofstedPage
                .checkPreviousRatingsQualityOfEducationJudgementsPresent()
                .checkPreviousRatingsBehaviourAndAttitudesJudgementsPresent()
                .checkPreviousRatingsPersonalDevelopmentJudgementsPresent()
                .checkPreviousRatingsLeadershipAndManagementJudgementsPresent()
                .checkPreviousRatingsEarlyYearsProvisionJudgementsPresent()
                .checkPreviousRatingsSixthFormProvisionJudgementsPresent()
                .checkPreviousRatingsBeforeOrAfterJoiningJudgementsPresent();
        });

        it('should export academies data as an xlsx and verify it has downloaded and has content', () => {
            ofstedPage
                .clickDownloadButton();
            dataDownload
                .checkFileDownloaded()
                .checkFileHasContent()
                .deleteDownloadedFile();
        });
    });

    describe("Testing the Ofsted Safeguarding and concerns page", () => {
        beforeEach(() => {
            cy.visit('/trusts/ofsted/safeguarding-and-concerns?uid=5143');

            cy.task('checkForFiles', 'cypress/downloads').then((files) => {
                if (files) {
                    cy.task('clearDownloads', 'cypress/downloads');
                }
            });
        });

        it("Checks the correct Ofsted safeguarding and concerns subpage header is present", () => {
            ofstedPage
                .checkOfstedSafeguardingConcernsPageHeaderPresent();
        });

        it("Checks the browser title is correct", () => {
            commonPage
                .checkThatBrowserTitleForTrustPageMatches('Safeguarding and concerns - Ofsted - {trustName} - Find information about schools and trusts');
        });

        it("Checks the breadcrumb shows the correct page name", () => {
            navigation
                .checkPageNameBreadcrumbPresent("Ofsted");
        });

        it("Checks the correct Ofsted safeguarding and concerns headers are present", () => {
            ofstedPage
                .checkOfstedSafeguardingConcernsTableHeadersPresent();
        });

        it("Checks the Ofsted page safeguarding and concerns sorting", () => {
            ofstedPage
                .checkOfstedSafeguardingConcernsSorting();
        });

        it("Checks that a trusts safeguarding and concerns correct judgement types are present", () => {
            ofstedPage
                .checkSafeguardingConcernsEffectiveSafeguardingJudgementsPresent()
                .checkSafeguardingConcernsCategoryOfConcernJudgementsPresent()
                .checkSafeguardingConcernsBeforeOrAfterJoiningJudgementsPresent();
        });

        it("Checks that a different trusts safeguarding and concerns correct judgement types are present", () => {
            cy.visit('/trusts/ofsted/safeguarding-and-concerns?uid=5712');
            ofstedPage
                .checkSafeguardingConcernsEffectiveSafeguardingJudgementsPresent()
                .checkSafeguardingConcernsCategoryOfConcernJudgementsPresent()
                .checkSafeguardingConcernsBeforeOrAfterJoiningJudgementsPresent();
        });

        it('should export academies data as an xlsx and verify it has downloaded and has content', () => {
            ofstedPage
                .clickDownloadButton();
            dataDownload
                .checkFileDownloaded()
                .checkFileHasContent()
                .deleteDownloadedFile();
        });
    });

    describe("Testing a trust that has no ofsted data within it to ensure the issue of a 500 page appearing does not happen", () => {
        beforeEach(() => {
            commonPage
                .interceptAndVerifyNo500Errors();
        });

        TestDataStore.GetTrustSubpagesFor(17728, "Ofsted").forEach(({ subpageName, url }) => {
            it(`Should have no 500 error on ${subpageName}`, () => {
                cy.visit(url);
            });
        });
    });

    describe("Testing that no unknown entries are found for ofsteds various tables/pages", () => {
        testTrustOfstedData.forEach(({ typeOfTrust, uid }) => {

            [`trusts/ofsted/single-headline-grades?uid=${uid}`, `trusts/ofsted/current-ratings?uid=${uid}`, `trusts/ofsted/previous-ratings?uid=${uid}`, `/trusts/ofsted/safeguarding-and-concerns?uid=${uid}`].forEach((url) => {
                it(`Should have no unknown entries on ${url} for a ${typeOfTrust}`, () => {
                    cy.visit(url);
                    commonPage
                        .checkNoUnknownEntries();
                });
            });
        });
    });
});
