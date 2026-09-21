import 'wick-a11y';
import homePage from '../../../pages/homePage';
import watchlistPage from '../../../pages/watchlist/watchlistPage';

describe('Watchlist Accessibility', () => {

    beforeEach(() => {
        cy.visit('/');
        homePage.clickWatchlistLink();
        cy.get('#main-content').should('be.visible');
    });

    describe('Watchlist Page Accessibility- Trusts', () => {
        it('should have accessible watchlist container', () => {
            cy.get('body').then($body => {
                if ($body.find('[data-testid*="watchlist"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="watchlist"]', {
                        includedImpacts: ['critical', 'serious'],
                        onlyWarnImpacts: ['moderate', 'minor']
                    });
                }
                if ($body.find('.watchlist-container').length > 0) {
                    cy.checkAccessibility('.watchlist-container');
                }
                if ($body.find('section[aria-label*="watchlist"]').length > 0) {
                    cy.checkAccessibility('section[aria-label*="watchlist"]');
                }
            });
        });

        it('should have accessible watchlist items', () => {
            cy.get('body').then($body => {
                if ($body.find('[data-testid*="watchlist-item"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="watchlist-item"]');
                }
                if ($body.find('.watchlist-item').length > 0) {
                    cy.checkAccessibility('.watchlist-item');
                }
                if ($body.find('li[role="listitem"]').length > 0) {
                    cy.checkAccessibility('li[role="listitem"]');
                }
            });
        });
    });

    describe('Watchlist Actions Accessibility', () => {
        it('should have accessible remove from watchlist buttons', () => {
             cy.get('body').then($body => {
                if ($body.find('[data-testid*="remove-watchlist"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="remove-watchlist"]');
                }
                if ($body.find('button[aria-label*="Remove"]').length > 0) {
                    cy.checkAccessibility('button[aria-label*="Remove"]');
                }
                if ($body.find('.watchlist-remove-btn').length > 0) {
                    cy.checkAccessibility('.watchlist-remove-btn');
                }
            });
        });
    });

    describe('Watchlist State Messages Accessibility', () => {
        it('should have accessible watchlist alerts and notifications', () => {
            cy.get('body').then($body => {
                if ($body.find('[role="alert"]').length > 0) {
                    cy.checkAccessibility('[role="alert"]');
                }
                if ($body.find('.govuk-notification-banner').length > 0) {
                    cy.checkAccessibility('.govuk-notification-banner');
                }
                if ($body.find('[aria-live]').length > 0) {
                    cy.checkAccessibility('[aria-live]');
                }
            });
        });
    });

    describe('Watchlist Heading and Labels Accessibility', () => {
        it('should have accessible watchlist headings', () => {
            cy.get('body').then($body => {
                if ($body.find('h1').length > 0) {
                    cy.checkAccessibility('h1');
                }
                if ($body.find('[data-testid*="watchlist-heading"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="watchlist-heading"]');
                }
            });
        });

        it('should have accessible watchlist form labels', () => {
            cy.get('body').then($body => {
                if ($body.find('label').length > 0) {
                    cy.checkAccessibility('label');
                }
                if ($body.find('[aria-label*="watchlist"]').length > 0) {
                    cy.checkAccessibility('[aria-label*="watchlist"]');
                }
            });
        });
    });

    describe('Watchlist Buttons Accessibility', () => {
        it('should have accessible buttons', () => {
            cy.get('body').then($body => {
                if ($body.find('button').length > 0) {
                    cy.checkAccessibility('button');
                }
                if ($body.find('[data-testid*="button"]').length > 0) {
                    cy.checkAccessibility('[data-testid*="button"]');
                }
                if ($body.find('button[aria-label]').length > 0) {
                    cy.checkAccessibility('button[aria-label]');
                }
            });
        });
    });

    describe('Watchlist Page Accessibility - Schools', () => {
          it('should have accessible links', () => {
             watchlistPage.navigateToSchoolTab();

              cy.get('body').then($body => {
                  if ($body.find('a').length > 0) {
                      cy.checkAccessibility('a');
                  }
                  if ($body.find('[data-testid*="link"]').length > 0) {
                      cy.checkAccessibility('[data-testid*="link"]');
                  }
                  if ($body.find('a[aria-label]').length > 0) {
                      cy.checkAccessibility('a[aria-label]');
                  }
              });
          });
    });
});
