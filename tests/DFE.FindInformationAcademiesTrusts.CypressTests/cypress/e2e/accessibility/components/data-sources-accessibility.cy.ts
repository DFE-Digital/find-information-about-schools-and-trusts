import 'wick-a11y';
import { TestDataStore } from '../../../support/test-data-store';

describe('Data Sources Component Accessibility', () => {

    describe('Trust Pages Data Sources Accessibility', () => {
        TestDataStore.GetAllTrustSubpagesForUid(5712).forEach(({ pageName, subpages }) => {
            describe(`${pageName} pages`, () => {
                subpages.forEach(({ subpageName, url }) => {
                    it(`should have accessible data sources component on ${pageName} > ${subpageName}`, () => {
                        cy.visit(url);

                        // Wait for page to load
                        cy.get('main, #main-content').should('be.visible');

                        // Check data sources component accessibility
                        cy.get('body').then($body => {
                            if ($body.find('[data-testid="data-sources"]').length > 0) {
                                cy.checkAccessibility('[data-testid="data-sources"]');
                            }
                            if ($body.find('.data-sources').length > 0) {
                                cy.checkAccessibility('.data-sources');
                            }
                            if ($body.find('[id*="data-source"]').length > 0) {
                                cy.checkAccessibility('[id*="data-source"]');
                            }
                        });
                    });
                });
            });
        });
    });

    describe('School Pages Data Sources Accessibility', () => {
        // Test both academy and LA maintained school types
        [
            { type: 'Academy', urn: 137083, getSubpages: TestDataStore.GetAllAcademySubpagesForUrn },
            { type: 'LA Maintained School', urn: 107188, getSubpages: TestDataStore.GetAllSchoolSubpagesForUrn }
        ].forEach(({ type, urn, getSubpages }) => {
            describe(`${type} data sources accessibility`, () => {
                getSubpages(urn).forEach(({ pageName, subpages }) => {
                    describe(`${pageName} pages`, () => {
                        subpages.forEach(({ subpageName, url }) => {
                            it(`should have accessible data sources component on ${pageName} > ${subpageName}`, () => {
                                cy.visit(url);

                                // Wait for page to load
                                cy.get('main, #main-content').should('be.visible');

                                // Check data sources component accessibility
                                cy.get('body').then($body => {
                                    if ($body.find('[data-testid="data-sources"]').length > 0) {
                                        cy.checkAccessibility('[data-testid="data-sources"]');
                                    }
                                    if ($body.find('.data-sources').length > 0) {
                                        cy.checkAccessibility('.data-sources');
                                    }
                                    if ($body.find('[id*="data-source"]').length > 0) {
                                        cy.checkAccessibility('[id*="data-source"]');
                                    }
                                });
                            });
                        });
                    });
                });
            });
        });
    });

    describe('Data Sources Component Elements Accessibility', () => {
        it('should have accessible data sources headings and lists', () => {
            // Test on a representative trust page
            cy.visit('/trusts/overview/trust-details?uid=5712');

            // Wait for page to load
            cy.get('main, #main-content').should('be.visible');

            // Check specific data sources component elements
            cy.get('body').then($body => {
                if ($body.find('[data-testid="data-sources"] h2, [data-testid="data-sources"] h3').length > 0) {
                    cy.checkAccessibility('[data-testid="data-sources"] h2, [data-testid="data-sources"] h3');
                }
                if ($body.find('[data-testid="data-sources"] ul, [data-testid="data-sources"] ol').length > 0) {
                    cy.checkAccessibility('[data-testid="data-sources"] ul, [data-testid="data-sources"] ol');
                }
                if ($body.find('[data-testid="data-sources"] a').length > 0) {
                    cy.checkAccessibility('[data-testid="data-sources"] a');
                }
            });
        });
    });
}); 
