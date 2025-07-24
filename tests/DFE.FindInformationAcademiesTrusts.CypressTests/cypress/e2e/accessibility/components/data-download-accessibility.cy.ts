import 'wick-a11y';

describe('Trust Data Download Accessibility', () => {

    describe('Academies Data Download Accessibility', () => {
        it('should have accessible academies download page', () => {
            cy.visit('/trusts/academies/in-trust/details?uid=5712');

            // Wait for page to load
            cy.get('main, #main-content').should('be.visible');

            // Check overall page accessibility
            cy.checkAccessibility();
        });

        it('should have accessible download buttons and controls', () => {
            // Check download functionality accessibility
            cy.get('body').then($body => {
                if ($body.find('button, .govuk-button').length > 0) {
                    cy.checkAccessibility('button, .govuk-button', {
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                }
                if ($body.find('a[href*="download"]').length > 0) {
                    cy.checkAccessibility('a[href*="download"]');
                }
                if ($body.find('[data-testid*="download"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="download"]');
                }
            });
        });

        it('should have accessible data tables for export', () => {
            // Check accessibility of tables being exported
            cy.get('body').then($body => {
                if ($body.find('table, .govuk-table').length > 0) {
                    cy.checkAccessibility('table, .govuk-table');
                }
                if ($body.find('.govuk-summary-list').length > 0) {
                    cy.checkAccessibility('.govuk-summary-list');
                }
            });
        });
    });

    describe('Ofsted Data Download Accessibility', () => {
        it('should have accessible ofsted download controls', () => {
            cy.visit('/trusts/ofsted/single-headline-grades?uid=5527');

            // Wait for page to load
            cy.get('main, #main-content').should('be.visible');

            // Check download functionality accessibility
            cy.get('body').then($body => {
                if ($body.find('button, .govuk-button').length > 0) {
                    cy.checkAccessibility('button, .govuk-button');
                }
                if ($body.find('a[href*="download"]').length > 0) {
                    cy.checkAccessibility('a[href*="download"]');
                }
            });
        });
    });

    describe('Pipeline Academies Data Download Accessibility', () => {
        it('should have accessible pipeline academies download controls', () => {
            cy.visit('/trusts/academies/pipeline/pre-advisory-board?uid=5527');

            // Wait for page to load
            cy.get('main, #main-content').should('be.visible');

            // Check download functionality accessibility - pipeline academies have download like UI tests
            cy.get('body').then($body => {
                if ($body.find('button, .govuk-button').length > 0) {
                    cy.checkAccessibility('button, .govuk-button');
                }
                if ($body.find('a[href*="download"]').length > 0) {
                    cy.checkAccessibility('a[href*="download"]');
                }
            });
        });
    });

    describe('Download Form Elements Accessibility', () => {
        it('should have accessible export form controls', () => {
            cy.visit('/trusts/academies/in-trust/details?uid=5712');

            // Check form accessibility for any export options
            cy.get('body').then($body => {
                if ($body.find('form').length > 0) {
                    cy.checkAccessibility('form');
                }
                if ($body.find('select, input').length > 0) {
                    cy.checkAccessibility('select, input');
                }
                if ($body.find('.govuk-form-group').length > 0) {
                    cy.checkAccessibility('.govuk-form-group');
                }
                if ($body.find('fieldset, legend').length > 0) {
                    cy.checkAccessibility('fieldset, legend');
                }
            });
        });

        it('should have accessible download status messages', () => {
            cy.visit('/trusts/academies/in-trust/details?uid=5712');

            // Check accessibility of any download status or error messages
            cy.get('body').then($body => {
                if ($body.find('.govuk-notification-banner').length > 0) {
                    cy.checkAccessibility('.govuk-notification-banner');
                }
                if ($body.find('.govuk-error-summary').length > 0) {
                    cy.checkAccessibility('.govuk-error-summary');
                }
                if ($body.find('[data-testid*="success"], [data-testid*="error"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="success"], [data-testid*="error"]');
                }
            });
        });
    });
}); 
