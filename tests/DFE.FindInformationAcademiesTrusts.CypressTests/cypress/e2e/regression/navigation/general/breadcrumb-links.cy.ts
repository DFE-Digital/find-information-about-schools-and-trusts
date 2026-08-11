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
                testTrustData.forEach(({ uid, trustName, trn }) => {
                    it('Should check that a trusts name breadcrumb is displayed on the trusts page', () => {
                        cy.visit(`/trusts/overview/trust-details?uid=${uid}&referencenumber=${trn}`);
                        navigation
                            .checkTrustNameBreadcrumbPresent(`${trustName}`)
                            .clickHomeBreadcrumbButton()
                            .checkBrowserPageTitleContains('Home page');
                    });
                });
            });

            describe("Testing the breadcrumb links on the pipeline academies pages", () => {
                [`/trusts/academies/pipeline/pre-decision?uid=16002&referencenumber=tr04032`, `/trusts/academies/pipeline/post-decision?uid=5527&referencenumber=tr04032`, `/trusts/academies/pipeline/free-schools?uid=17584&referencenumber=tr04032`].forEach((url) => {
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
                getSubpages: (urn: number, trn: string) => TestDataStore.GetAllSchoolSubpagesForUrn(urn, trn)
            },
            {
                ...testBreadcrumbSchoolData.academyConverter,
                getSubpages: (urn: number, trn: string) => TestDataStore.GetAllAcademySubpagesForUrn(urn, trn)
            }
        ];

        schoolBreadcrumbTestData.forEach(({ urn, type, trn, getSubpages }) => {
            getSubpages(urn, trn).forEach(({ pageName, subpages }) => {
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
