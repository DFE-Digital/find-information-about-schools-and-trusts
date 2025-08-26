import 'wick-a11y';
import navigation from "../../../pages/navigation";
import { testTrustData, testBreadcrumbSchoolData } from "../../../support/test-data-store";

describe('Navigation Accessibility', () => {
    it('should have accessible main navigation', () => {
        cy.visit('/');

        // Check main site navigation with conditional check
        cy.get('body').then($body => {
            if ($body.find('nav, [role="navigation"]').length > 0) {
                cy.checkAccessibility('nav, [role="navigation"]');
            }
        });
    });

    it('should have accessible navigation links', () => {
        cy.visit('/');

        // Check navigation link accessibility
        cy.get('body').then($body => {
            if ($body.find('nav a, .govuk-header__navigation a').length > 0) {
                cy.checkAccessibility('nav a, .govuk-header__navigation a');
            }
        });
    });

    describe('should have accessible breadcrumb navigation following UI test patterns', () => {

        describe("General page breadcrumb accessibility", () => {
            ['/search', '/accessibility', '/cookies', '/privacy', '/notfound'].forEach((url) => {
                it(`Should have accessible Home breadcrumb on ${url}`, () => {
                    cy.visit(url, { failOnStatusCode: false });

                    navigation
                        .checkCurrentURLIsCorrect(url)
                        .checkHomeBreadcrumbPresent();

                    // Check breadcrumb accessibility
                    cy.get('body').then($body => {
                        if ($body.find('[aria-label="Breadcrumb"]').length > 0) {
                            cy.checkAccessibility('[aria-label="Breadcrumb"]', {
                                includedImpacts: ['critical', 'serious'],
                                onlyWarnImpacts: ['moderate', 'minor']
                            });
                        }
                    });
                });
            });

            ['/', '/error', '/aboutdata'].forEach((url) => {
                it(`Should have accessible page structure without breadcrumb on ${url}`, () => {
                    cy.visit(url);

                    navigation
                        .checkCurrentURLIsCorrect(url)
                        .checkAccessibilityStatementLinkPresent()
                        .checkBreadcrumbNotPresent();

                    // Check main page accessibility since no breadcrumbs
                    cy.get('main, #main-content').should('be.visible');
                    cy.checkAccessibility('main, #main-content');
                });
            });
        });

        describe("Trust page breadcrumb accessibility", () => {
            testTrustData.forEach(({ uid, trustName }) => {
                it(`Should have accessible trust breadcrumb for ${trustName}`, () => {
                    cy.visit(`/trusts/overview/trust-details?uid=${uid}`);

                    navigation
                        .checkTrustNameBreadcrumbPresent(`${trustName}`)
                        .checkHomeBreadcrumbPresent();

                    // Check breadcrumb accessibility
                    cy.get('body').then($body => {
                        if ($body.find('[aria-label="Breadcrumb"]').length > 0) {
                            cy.checkAccessibility('[aria-label="Breadcrumb"]');
                        }
                    });
                });
            });
        });

        describe("Pipeline academies breadcrumb accessibility", () => {
            [`/trusts/academies/pipeline/pre-advisory-board?uid=16002`, `/trusts/academies/pipeline/post-advisory-board?uid=17584`, `/trusts/academies/pipeline/free-schools?uid=17584`].forEach((url) => {
                it(`Should have accessible "Academies" page breadcrumb on ${url}`, () => {
                    cy.visit(url);

                    navigation.checkPageNameBreadcrumbPresent("Academies");

                    // Check breadcrumb accessibility
                    cy.get('body').then($body => {
                        if ($body.find('[aria-label="Breadcrumb"]').length > 0) {
                            cy.checkAccessibility('[aria-label="Breadcrumb"]');
                        }
                    });
                });
            });
        });

        describe("School overview breadcrumb accessibility for LA Maintained School", () => {
            const testBreadcrumbSchool = testBreadcrumbSchoolData.communitySchool;

            [`/schools/overview/details?urn=${testBreadcrumbSchool.urn}`, `/schools/overview/federation?urn=${testBreadcrumbSchool.urn}`, `/schools/overview/sen?urn=${testBreadcrumbSchool.urn}`].forEach((url) => {
                it(`Should have accessible "Overview" breadcrumb for ${testBreadcrumbSchool.type} on ${url}`, () => {
                    cy.visit(url);

                    navigation.checkPageNameBreadcrumbPresent("Overview");

                    // Check breadcrumb accessibility
                    cy.get('body').then($body => {
                        if ($body.find('[aria-label="Breadcrumb"]').length > 0) {
                            cy.checkAccessibility('[aria-label="Breadcrumb"]');
                        }
                    });
                });
            });
        });

        describe("School overview breadcrumb accessibility for Academy", () => {
            const testBreadcrumbAcademy = testBreadcrumbSchoolData.academyConverter;

            [`/schools/overview/details?urn=${testBreadcrumbAcademy.urn}`, `/schools/overview/sen?urn=${testBreadcrumbAcademy.urn}`].forEach((url) => {
                it(`Should have accessible "Overview" breadcrumb for ${testBreadcrumbAcademy.type} on ${url}`, () => {
                    cy.visit(url);

                    navigation.checkPageNameBreadcrumbPresent("Overview");

                    // Check breadcrumb accessibility
                    cy.get('body').then($body => {
                        if ($body.find('[aria-label="Breadcrumb"]').length > 0) {
                            cy.checkAccessibility('[aria-label="Breadcrumb"]');
                        }
                    });
                });
            });
        });
    });

    it('should have accessible skip links', () => {
        cy.visit('/');

        // Check skip to main content links
        cy.get('body').then($body => {
            if ($body.find('.govuk-skip-link, a[href="#main-content"]').length > 0) {
                cy.checkAccessibility('.govuk-skip-link, a[href="#main-content"]');
            }
        });
    });

    it('should have accessible footer navigation', () => {
        cy.visit('/');

        // Use navigation POM to check footer links
        navigation
            .checkAboutDataLinkPresent()
            .checkPrivacyLinkPresent()
            .checkCookiesLinkPresent()
            .checkAccessibilityStatementLinkPresent();

        // Check footer navigation
        cy.get('body').then($body => {
            if ($body.find('footer nav, .govuk-footer__navigation').length > 0) {
                cy.checkAccessibility('footer nav, .govuk-footer__navigation');
            }
        });
    });

    it('should have accessible breadcrumbs using navigation POM', () => {
        const trustData = testTrustData[0]; // Ashton West End Primary Academy
        cy.visit(`/trusts/overview/trust-details?uid=${trustData.uid}`);

        // Use navigation POM to verify breadcrumbs
        navigation
            .checkHomeBreadcrumbPresent()
            .checkTrustNameBreadcrumbPresent(trustData.trustName);

        // Check breadcrumb accessibility
        cy.get('body').then($body => {
            if ($body.find('[aria-label="Breadcrumb"]').length > 0) {
                cy.checkAccessibility('[aria-label="Breadcrumb"]');
            }
        });
    });

    it('should have accessible service navigation', () => {
        const trustData = testTrustData[1]; // Aspire North East Multi Academy Trust
        cy.visit(`/trusts/overview/trust-details?uid=${trustData.uid}`);

        // Use navigation POM to check service navigation
        navigation.checkAllServiceNavItemsPresent();

        // Check service navigation accessibility
        cy.get('body').then($body => {
            if ($body.find('[data-testid="service-navigation"]').length > 0) {
                cy.checkAccessibility('[data-testid="service-navigation"]');
            }
        });
    });
}); 
