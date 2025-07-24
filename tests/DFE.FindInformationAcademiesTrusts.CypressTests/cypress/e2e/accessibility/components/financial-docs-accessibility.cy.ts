import 'wick-a11y';
import { testFinanceData } from '../../../support/test-data-store';

describe('Trust Financial Documents Pages Accessibility', () => {

    testFinanceData.forEach(({ uid }) => {
        describe(`Financial Documents Accessibility for Trust UID ${uid}`, () => {

            describe('Financial Statements Page Accessibility', () => {
                beforeEach(() => {
                    cy.visit(`/trusts/financial-documents/financial-statements?uid=${uid}`);
                });

                it('should have accessible financial statements page', () => {
                    // Wait for page to load
                    cy.get('main, #main-content').should('be.visible');

                    // Financial documents contain sensitive business data - disable HTML reports
                    cy.checkAccessibility(null, {
                        generateReport: false,
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                });

                it('should have accessible financial statements components', () => {
                    // Check accessibility of financial documents components - disable reports for sensitive data
                    cy.get('body').then($body => {
                        if ($body.find('[data-testid="subpage-header"]').length > 0) {
                            cy.checkAccessibility('[data-testid="subpage-header"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid="financial-docs-financial-year"]').length > 0) {
                            cy.checkAccessibility('[data-testid="financial-docs-financial-year"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid="financial-docs-financial-status-or-link"]').length > 0) {
                            cy.checkAccessibility('[data-testid="financial-docs-financial-status-or-link"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid="about-these-documents"]').length > 0) {
                            cy.checkAccessibility('[data-testid="about-these-documents"]', { generateReport: false });
                        }
                    });
                });
            });

            describe('Management Letters Page Accessibility', () => {
                beforeEach(() => {
                    cy.visit(`/trusts/financial-documents/management-letters?uid=${uid}`);
                });

                it('should have accessible management letters page', () => {
                    // Wait for page to load
                    cy.get('main, #main-content').should('be.visible');

                    // Financial documents contain sensitive business data - disable HTML reports
                    cy.checkAccessibility(null, {
                        generateReport: false,
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                });

                it('should have accessible management letters components', () => {
                    // Check accessibility of warning messages specifically - disable reports for sensitive data
                    cy.get('body').then($body => {
                        if ($body.find('[data-testid="internal-use-only-warning"]').length > 0) {
                            cy.checkAccessibility('[data-testid="internal-use-only-warning"]', {
                                includedImpacts: ['critical', 'serious'],
                                onlyWarnImpacts: ['moderate', 'minor'],
                                generateReport: false
                            });
                        }
                        if ($body.find('[data-testid="you-must-have-permission-message"]').length > 0) {
                            cy.checkAccessibility('[data-testid="you-must-have-permission-message"]', { generateReport: false });
                        }
                    });
                });
            });

            describe('Internal Scrutiny Reports Page Accessibility', () => {
                beforeEach(() => {
                    cy.visit(`/trusts/financial-documents/internal-scrutiny-reports?uid=${uid}`);
                });

                it('should have accessible internal scrutiny reports page', () => {
                    // Wait for page to load
                    cy.get('main, #main-content').should('be.visible');

                    // Financial documents contain sensitive business data - disable HTML reports
                    cy.checkAccessibility(null, {
                        generateReport: false,
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                });
            });

            describe('Self-Assessment Checklists Page Accessibility', () => {
                beforeEach(() => {
                    cy.visit(`/trusts/financial-documents/self-assessment-checklists?uid=${uid}`);
                });

                it('should have accessible self-assessment checklists page', () => {
                    // Wait for page to load
                    cy.get('main, #main-content').should('be.visible');

                    // Financial documents contain sensitive business data - disable HTML reports
                    cy.checkAccessibility(null, {
                        generateReport: false,
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                });
            });
        });

        describe(`Common Financial Documents Components Accessibility for Trust UID ${uid}`, () => {
            // Test common components across all financial document pages
            [`/trusts/financial-documents/financial-statements?uid=${uid}`,
            `/trusts/financial-documents/management-letters?uid=${uid}`,
            `/trusts/financial-documents/internal-scrutiny-reports?uid=${uid}`,
            `/trusts/financial-documents/self-assessment-checklists?uid=${uid}`].forEach((url) => {

                it(`should have accessible common components on ${url}`, () => {
                    cy.visit(url);

                    // Wait for page to load
                    cy.get('main, #main-content').should('be.visible');

                    // Check accessibility of common components - disable reports for sensitive financial data
                    cy.get('body').then($body => {
                        if ($body.find('[data-testid="about-these-documents"]').length > 0) {
                            cy.checkAccessibility('[data-testid="about-these-documents"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid="you-must-have-permission-message"]').length > 0) {
                            cy.checkAccessibility('[data-testid="you-must-have-permission-message"]', { generateReport: false });
                        }
                        if ($body.find('[aria-label="Breadcrumb"]').length > 0) {
                            cy.checkAccessibility('[aria-label="Breadcrumb"]', { generateReport: false });
                        }
                        if ($body.find('.govuk-breadcrumbs').length > 0) {
                            cy.checkAccessibility('.govuk-breadcrumbs', { generateReport: false });
                        }
                    });
                });

                it(`should have accessible data display elements on ${url}`, () => {
                    cy.visit(url);

                    // Check financial data components accessibility - disable reports for sensitive financial data
                    cy.get('body').then($body => {
                        if ($body.find('[data-testid="financial-docs-financial-year"]').length > 0) {
                            cy.checkAccessibility('[data-testid="financial-docs-financial-year"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid="financial-docs-financial-status-or-link"]').length > 0) {
                            cy.checkAccessibility('[data-testid="financial-docs-financial-status-or-link"]', { generateReport: false });
                        }
                        if ($body.find('table, .govuk-table').length > 0) {
                            cy.checkAccessibility('table, .govuk-table', {
                                includedImpacts: ['critical', 'serious'],
                                onlyWarnImpacts: ['moderate', 'minor'],
                                generateReport: false
                            });
                        }
                        if ($body.find('a[href$=".pdf"], a[href*="download"]').length > 0) {
                            cy.checkAccessibility('a[href$=".pdf"], a[href*="download"]', { generateReport: false });
                        }
                    });
                });
            });
        });
    });

    describe('Financial Documents Navigation Accessibility', () => {
        it('should have accessible financial documents sub-navigation', () => {
            cy.visit('/trusts/financial-documents/financial-statements?uid=5527');

            // Wait for page to load
            cy.get('main, #main-content').should('be.visible');

            // Check financial documents sub-navigation accessibility - disable reports for sensitive financial data
            cy.get('body').then($body => {
                if ($body.find('.moj-sub-navigation').length > 0) {
                    cy.checkAccessibility('.moj-sub-navigation', { generateReport: false });
                }
                if ($body.find('[data-testid*="financial-documents"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="financial-documents"]', { generateReport: false });
                }
                if ($body.find('nav[aria-label*="sub"]').length > 0) {
                    cy.checkAccessibility('nav[aria-label*="sub"]', { generateReport: false });
                }
            });
        });

        it('should have accessible financial documents service navigation', () => {
            cy.visit('/trusts/financial-documents/financial-statements?uid=5527');

            // Check service navigation accessibility - disable reports for sensitive financial data
            cy.get('body').then($body => {
                if ($body.find('[data-testid="service-navigation"]').length > 0) {
                    cy.checkAccessibility('[data-testid="service-navigation"]', { generateReport: false });
                }
                if ($body.find('.govuk-header__navigation').length > 0) {
                    cy.checkAccessibility('.govuk-header__navigation', { generateReport: false });
                }
            });
        });
    });
});
