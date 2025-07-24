import 'wick-a11y';
import { testTrustData } from '../../../support/test-data-store';

describe('Trust Contacts Pages Accessibility', () => {

    testTrustData.forEach(({ typeOfTrust, uid }) => {
        describe(`Contacts Pages Accessibility for ${typeOfTrust}`, () => {

            describe('In DfE Contacts Page Accessibility', () => {
                beforeEach(() => {
                    cy.visit(`/trusts/contacts/in-dfe?uid=${uid}`);
                });

                it('should have accessible in DfE contacts page', () => {
                    // Wait for page to load
                    cy.get('main, #main-content').should('be.visible');

                    // Contacts pages contain sensitive personal data - disable HTML reports
                    cy.checkAccessibility(null, {
                        generateReport: false,
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                });

                it('should have accessible contact forms and inputs', () => {
                    // Check contact form accessibility - disable reports for sensitive personal data
                    cy.get('body').then($body => {
                        if ($body.find('form').length > 0) {
                            cy.checkAccessibility('form', {
                                generateReport: false,
                                includedImpacts: ['critical', 'serious'],
                                onlyWarnImpacts: ['moderate', 'minor']
                            });
                        }
                        if ($body.find('input, textarea, select').length > 0) {
                            cy.checkAccessibility('input, textarea, select', { generateReport: false });
                        }
                        if ($body.find('button, .govuk-button').length > 0) {
                            cy.checkAccessibility('button, .govuk-button', { generateReport: false });
                        }
                    });
                });

                it('should have accessible contact information display', () => {
                    // Check contact data presentation - disable reports for sensitive personal data
                    cy.get('body').then($body => {
                        if ($body.find('.govuk-summary-list').length > 0) {
                            cy.checkAccessibility('.govuk-summary-list', { generateReport: false });
                        }
                        if ($body.find('[data-testid*="contact"]').length > 0) {
                            cy.checkAccessibility('[data-testid*="contact"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid*="trust-relationship-manager"]').length > 0) {
                            cy.checkAccessibility('[data-testid*="trust-relationship-manager"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid*="sfso-lead"]').length > 0) {
                            cy.checkAccessibility('[data-testid*="sfso-lead"]', { generateReport: false });
                        }
                    });
                });
            });

            describe('In Trust Contacts Page Accessibility', () => {
                beforeEach(() => {
                    cy.visit(`/trusts/contacts/in-the-trust?uid=${uid}`);
                });

                it('should have accessible in trust contacts page', () => {
                    // Wait for page to load
                    cy.get('main, #main-content').should('be.visible');

                    // Contacts pages contain sensitive personal data - disable HTML reports
                    cy.checkAccessibility(null, {
                        generateReport: false,
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                });

                it('should have accessible trust contact information', () => {
                    // Check external contact information display - disable reports for sensitive personal data
                    cy.get('body').then($body => {
                        if ($body.find('.govuk-summary-list').length > 0) {
                            cy.checkAccessibility('.govuk-summary-list', { generateReport: false });
                        }
                        if ($body.find('[data-testid*="accounting-officer"]').length > 0) {
                            cy.checkAccessibility('[data-testid*="accounting-officer"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid*="chair-trustees"]').length > 0) {
                            cy.checkAccessibility('[data-testid*="chair-trustees"]', { generateReport: false });
                        }
                        if ($body.find('[data-testid*="chief-financial-officer"]').length > 0) {
                            cy.checkAccessibility('[data-testid*="chief-financial-officer"]', { generateReport: false });
                        }
                    });
                });
            });
        });

        describe(`Contact Edit Pages Accessibility for ${typeOfTrust}`, () => {

            it('should have accessible Trust relationship manager edit page', () => {
                cy.visit(`/trusts/contacts/edittrustrelationshipmanager?uid=${uid}`);

                // Wait for page to load
                cy.get('main, #main-content').should('be.visible');

                // Contacts edit pages contain sensitive personal data - disable HTML reports
                cy.checkAccessibility(null, {
                    generateReport: false,
                    includedImpacts: ['critical', 'serious'],
                    onlyWarnImpacts: ['moderate', 'minor']
                });

                // Check form accessibility specifically - disable reports for sensitive personal data
                cy.get('body').then($body => {
                    if ($body.find('form').length > 0) {
                        cy.checkAccessibility('form', { generateReport: false });
                    }
                    if ($body.find('.govuk-form-group').length > 0) {
                        cy.checkAccessibility('.govuk-form-group', { generateReport: false });
                    }
                    if ($body.find('label').length > 0) {
                        cy.checkAccessibility('label', { generateReport: false });
                    }
                    if ($body.find('.govuk-input').length > 0) {
                        cy.checkAccessibility('.govuk-input', { generateReport: false });
                    }
                });
            });

            it('should have accessible SFSO lead edit page', () => {
                cy.visit(`/trusts/contacts/editsfsolead?uid=${uid}`);

                // Wait for page to load
                cy.get('main, #main-content').should('be.visible');

                // Contacts edit pages contain sensitive personal data - disable HTML reports
                cy.checkAccessibility(null, {
                    generateReport: false,
                    includedImpacts: ['critical', 'serious'],
                    onlyWarnImpacts: ['moderate', 'minor']
                });

                // Check form accessibility specifically - disable reports for sensitive personal data
                cy.get('body').then($body => {
                    if ($body.find('form').length > 0) {
                        cy.checkAccessibility('form', { generateReport: false });
                    }
                    if ($body.find('.govuk-form-group').length > 0) {
                        cy.checkAccessibility('.govuk-form-group', { generateReport: false });
                    }
                    if ($body.find('.govuk-label').length > 0) {
                        cy.checkAccessibility('.govuk-label', { generateReport: false });
                    }
                    if ($body.find('input[type="text"], input[type="email"]').length > 0) {
                        cy.checkAccessibility('input[type="text"], input[type="email"]', { generateReport: false });
                    }
                });
            });
        });
    });

    describe('Common Contacts Components Accessibility', () => {
        it('should have accessible navigation and breadcrumbs', () => {
            cy.visit('/trusts/contacts/in-dfe?uid=5527');

            // Wait for page to load
            cy.get('main, #main-content').should('be.visible');

            // Check navigation accessibility - disable reports for sensitive personal data
            cy.get('body').then($body => {
                if ($body.find('[aria-label="Breadcrumb"]').length > 0) {
                    cy.checkAccessibility('[aria-label="Breadcrumb"]', { generateReport: false });
                }
                if ($body.find('.govuk-breadcrumbs').length > 0) {
                    cy.checkAccessibility('.govuk-breadcrumbs', { generateReport: false });
                }
                if ($body.find('.moj-sub-navigation').length > 0) {
                    cy.checkAccessibility('.moj-sub-navigation', { generateReport: false });
                }
                if ($body.find('[data-testid="service-navigation"]').length > 0) {
                    cy.checkAccessibility('[data-testid="service-navigation"]', { generateReport: false });
                }
            });
        });

        it('should have accessible success and error messages', () => {
            cy.visit('/trusts/contacts/in-dfe?uid=5527');

            // Check message accessibility (if they appear) - disable reports for sensitive personal data
            cy.get('body').then($body => {
                if ($body.find('.govuk-notification-banner').length > 0) {
                    cy.checkAccessibility('.govuk-notification-banner', { generateReport: false });
                }
                if ($body.find('.govuk-error-summary').length > 0) {
                    cy.checkAccessibility('.govuk-error-summary', { generateReport: false });
                }
                if ($body.find('[data-testid*="success"], [data-testid*="error"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="success"], [data-testid*="error"]', { generateReport: false });
                }
                if ($body.find('.govuk-error-message').length > 0) {
                    cy.checkAccessibility('.govuk-error-message', { generateReport: false });
                }
            });
        });

        it('should have accessible page headers and structure', () => {
            cy.visit('/trusts/contacts/in-the-trust?uid=5527');

            // Check page structure accessibility - disable reports for sensitive personal data
            cy.get('body').then($body => {
                if ($body.find('h1').length > 0) {
                    cy.checkAccessibility('h1', { generateReport: false });
                }
                if ($body.find('h2, h3').length > 0) {
                    cy.checkAccessibility('h2, h3', { generateReport: false });
                }
                if ($body.find('[data-testid*="page-title"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="page-title"]', { generateReport: false });
                }
                if ($body.find('[data-testid="subpage-header"]').length > 0) {
                    cy.checkAccessibility('[data-testid="subpage-header"]', { generateReport: false });
                }
            });
        });
    });
}); 
