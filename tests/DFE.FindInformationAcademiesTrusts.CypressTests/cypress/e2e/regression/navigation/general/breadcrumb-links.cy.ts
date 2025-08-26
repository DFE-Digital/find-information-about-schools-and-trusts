import navigation from "../../../../pages/navigation";
import { testTrustData, testBreadcrumbSchoolData, TestDataStore } from "../../../../support/test-data-store";

describe('Testing breadcrumb functionality across FAST', () => {
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

        ['/', '/error', '/aboutdata'].forEach((url) => {
            it(`Should have no breadcrumb on ${url}`, () => {
                cy.visit(url);

                navigation
                    .checkCurrentURLIsCorrect(url)
                    .checkAccessibilityStatementLinkPresent() // ensure page content has loaded - all pages have an a11y statement link
                    .checkBreadcrumbNotPresent();
            });
        });
    });

    describe("Testing the breadcrumb-links for Trust pages", () => {

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
                    });
                });
            });
        });
    });

    describe("Testing breadcrumb links for School pages", () => {
        const schoolBreadcrumbTestData = [
            {
                ...testBreadcrumbSchoolData.communitySchool,
                getSubpages: (urn: number) => TestDataStore.GetAllSchoolSubpagesForUrn(urn)
            },
            {
                ...testBreadcrumbSchoolData.academyConverter,
                getSubpages: (urn: number) => TestDataStore.GetAllAcademySubpagesForUrn(urn)
            }
        ];

        schoolBreadcrumbTestData.forEach(({ urn, type, getSubpages }) => {
            getSubpages(urn).forEach(({ pageName, subpages }) => {
                describe(`Testing breadcrumb links on ${pageName} pages for ${type}`, () => {
                    subpages.forEach(({ subpageName, url }) => {
                        it(`Checks the breadcrumb shows the correct page name for ${type} on ${pageName} > ${subpageName}`, () => {
                            cy.visit(url);
                            navigation
                                .checkPageNameBreadcrumbPresent(pageName);
                        });
                    });
                });
            });
        });
    });
});
