import navigation from "../../../../pages/navigation";
import { testTrustData } from "../../../../support/test-data-store";

describe('Testing Navigation', () => {

    describe("Testing the breadcrumb-links", () => {

        describe("Testing the general page breadcrumb links and their relevant functionality", () => {

            ['/search', '/accessibility', '/cookies', '/privacy', '/notfound'].forEach((url) => {
                it(`Should have Home breadcrumb only on ${url}`, () => {
                    cy.visit(url, { failOnStatusCode: false });

                    navigation
                        .checkCurrentURLIsCorrect(url)
                        .checkHomeBreadcrumbPresent()
                        .clickHomeBreadcrumbButton()
                        .checkBrowserPageTitleContains('Home page');
                });
            });

            ['/', '/error'].forEach((url) => {
                it(`Should have no breadcrumb on ${url}`, () => {
                    cy.visit(url);

                    navigation
                        .checkCurrentURLIsCorrect(url)
                        .checkAccessibilityStatementLinkPresent() // ensure page content has loaded - all pages have an a11y statement link
                        .checkBreadcrumbNotPresent();
                });
            });
        });

        describe("Testing the breadcrumb links on the trust academy details page", () => {

            describe("Testing the breadcrumb links on the trust page", () => {
                testTrustData.forEach(({ uid, trustName }) => {
                    it('Should check that a trusts name breadcrumb is displayed on the trusts page', () => {
                        cy.visit(`/trusts/overview/trust-details?uid=${uid}`);
                        navigation
                            .checkTrustNameBreadcrumbPresent(`${trustName}`)
                            .clickHomeBreadcrumbButton()
                            .checkBrowserPageTitleContains('Home page');
                    });
                });
            });

            describe("Testing the breadcrumb links on the pipeline academies pages", () => {
                [`/trusts/academies/pipeline/pre-advisory-board?uid=16002`, `/trusts/academies/pipeline/post-advisory-board?uid=17584`, `/trusts/academies/pipeline/free-schools?uid=17584`].forEach((url) => {
                    it("Checks the breadcrumb shows the correct page name", () => {
                        cy.visit(url);
                        navigation
                            .checkPageNameBreadcrumbPresent("Academies");
                        //revist adding this later need to rethink how the foreach handles differing uid pulls for the differnt pages .checkTrustNameBreadcrumbPresent('Ashton West End Primary Academy');
                    });
                });
            });
        });
    });

    describe("Testing the breadcrumb links on the schools overview pages for a LA Maintained School", () => {
        const testBreadcrumbSchool = {
            urn: 107188,
            type: "Community school"
        };

        [`/schools/overview/details?urn=${testBreadcrumbSchool.urn}`, `/schools/overview/federation?urn=${testBreadcrumbSchool.urn}`, `/schools/overview/sen?urn=${testBreadcrumbSchool.urn}`].forEach((url) => {
            it(`Checks the breadcrumb shows the correct page name for ${testBreadcrumbSchool.type} on ${url}`, () => {
                cy.visit(url);
                navigation
                    .checkPageNameBreadcrumbPresent("Overview");
            });
        });
    });

    describe("Testing the breadcrumb links on the schools overview pages for an Academy", () => {
        const testBreadcrumbAcademy = {
            urn: 137083,
            type: "Academy converter"
        };

        [`/schools/overview/details?urn=${testBreadcrumbAcademy.urn}`, `/schools/overview/sen?urn=${testBreadcrumbAcademy.urn}`].forEach((url) => {
            it(`Checks the breadcrumb shows the correct page name for ${testBreadcrumbAcademy.type} on ${url}`, () => {
                cy.visit(url);
                navigation
                    .checkPageNameBreadcrumbPresent("Overview");
            });
        });
    });
});

