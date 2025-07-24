import 'wick-a11y';
import { testPaginationData } from '../../../support/test-data-store';

describe('Pagination Accessibility', () => {

    describe('Search Results Pagination Accessibility', () => {
        it('should have accessible pagination on search results', () => {
            // Navigate to search that has multiple pages
            cy.visit('/search?searchterm=academy');

            // Wait for page to load
            cy.get('#main-content').should('be.visible');

            // Check pagination accessibility (if present)
            cy.get('body').then($body => {
                if ($body.find('.govuk-pagination').length > 0) {
                    cy.checkAccessibility('.govuk-pagination', {
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                }
                if ($body.find('[data-testid*="pagination"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="pagination"]');
                }
                if ($body.find('nav[aria-label*="pagination"]').length > 0) {
                    cy.checkAccessibility('nav[aria-label*="pagination"]');
                }
            });
        });

        it('should have accessible pagination navigation controls', () => {
            cy.visit('/search?searchterm=school');

            // Wait for page to load
            cy.get('#main-content').should('be.visible');

            // Check pagination navigation controls accessibility
            cy.get('body').then($body => {
                if ($body.find('[data-testid*="next"], [data-testid*="previous"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="next"], [data-testid*="previous"]');
                }
                if ($body.find('a[rel="next"], a[rel="prev"]').length > 0) {
                    cy.checkAccessibility('a[rel="next"], a[rel="prev"]');
                }
                if ($body.find('.govuk-pagination__link').length > 0) {
                    cy.checkAccessibility('.govuk-pagination__link');
                }
            });
        });
    });

    describe('Trust Data Tables Pagination Accessibility', () => {
        it('should have accessible pagination on trust academies tables', () => {
            // Test pagination accessibility on academies data table
            cy.visit(`/trusts/academies/in-trust/details?uid=${testPaginationData.academiesInTrustUid}`);

            // Wait for page to load
            cy.get('#main-content').should('be.visible');

            // Check table pagination accessibility
            cy.get('body').then($body => {
                if ($body.find('.govuk-pagination').length > 0) {
                    cy.checkAccessibility('.govuk-pagination');
                }
                if ($body.find('[data-testid*="table-pagination"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="table-pagination"]');
                }
            });
        });

        it('should have accessible pagination on ofsted tables', () => {
            // Test pagination accessibility on Ofsted ratings data table
            cy.visit(`/trusts/ofsted/current-ratings?uid=${testPaginationData.ofstedRatingsUid}`);

            // Wait for page to load
            cy.get('#main-content').should('be.visible');

            // Check Ofsted table pagination accessibility
            cy.get('body').then($body => {
                if ($body.find('.govuk-pagination').length > 0) {
                    cy.checkAccessibility('.govuk-pagination');
                }
            });
        });
    });

    describe('Pagination Components Accessibility', () => {
        it('should have accessible pagination ellipsis and numbering', () => {
            cy.visit('/search?searchterm=trust');

            // Wait for page to load
            cy.get('#main-content').should('be.visible');

            // Check pagination components accessibility
            cy.get('body').then($body => {
                if ($body.find('.govuk-pagination__item').length > 0) {
                    cy.checkAccessibility('.govuk-pagination__item');
                }
                if ($body.find('[aria-label*="Page"]').length > 0) {
                    cy.checkAccessibility('[aria-label*="Page"]');
                }
                if ($body.find('.govuk-pagination__ellipses').length > 0) {
                    cy.checkAccessibility('.govuk-pagination__ellipses');
                }
            });
        });

        it('should have accessible current page indication', () => {
            cy.visit('/search?searchterm=school');

            // Wait for page to load
            cy.get('#main-content').should('be.visible');

            // Check current page accessibility
            cy.get('body').then($body => {
                if ($body.find('[aria-current="page"]').length > 0) {
                    cy.checkAccessibility('[aria-current="page"]');
                }
                if ($body.find('.govuk-pagination__link--current').length > 0) {
                    cy.checkAccessibility('.govuk-pagination__link--current');
                }
            });
        });
    });
}); 
