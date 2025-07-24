import 'wick-a11y';

describe('Header Search Accessibility', () => {

    // Test header search on the same pages as UI tests - header search only appears on non-home pages
    [`/schools/overview/details?urn=123452`, `/trusts/overview/trust-details?uid=5527`].forEach((url) => {
        describe(`Header search accessibility on ${url}`, () => {
            beforeEach(() => {
                cy.visit(url);
            });

            it('should have accessible header search form', () => {
                // Wait for page to load
                cy.get('main, #main-content').should('be.visible');

                // Check header search form accessibility
                cy.get('body').then($body => {
                    if ($body.find('header form, .govuk-header form').length > 0) {
                        cy.checkAccessibility('header form, .govuk-header form');
                    }
                    if ($body.find('header input[type="search"], header input[type="text"]').length > 0) {
                        cy.checkAccessibility('header input[type="search"], header input[type="text"]');
                    }
                    if ($body.find('header button[type="submit"]').length > 0) {
                        cy.checkAccessibility('header button[type="submit"]');
                    }
                });
            });

            it('should have accessible header search autocomplete', () => {
                // Test autocomplete accessibility by triggering it
                cy.get('body').then($body => {
                    if ($body.find('header input[type="search"], header input[type="text"]').length > 0) {
                        // Type text to trigger autocomplete
                        cy.get('header input[type="search"], header input[type="text"]').first().type('West');

                        // Check autocomplete accessibility if it appears
                        cy.get('body').then($updatedBody => {
                            if ($updatedBody.find('[id*="autocomplete"], [class*="autocomplete"]').length > 0) {
                                cy.checkAccessibility('[id*="autocomplete"], [class*="autocomplete"]', {
                                    includedImpacts: ['critical', 'serious'],
                                    onlyWarnImpacts: ['moderate', 'minor']
                                });
                            }
                        });
                    }
                });
            });

            it('should have accessible header navigation elements', () => {
                // Check header navigation accessibility
                cy.get('body').then($body => {
                    if ($body.find('header nav, .govuk-header__navigation').length > 0) {
                        cy.checkAccessibility('header nav, .govuk-header__navigation');
                    }
                    if ($body.find('header .govuk-header__logo').length > 0) {
                        cy.checkAccessibility('header .govuk-header__logo');
                    }
                });
            });
        });
    });

    describe('Header Search Component Interactions', () => {
        it('should maintain accessibility during search interactions', () => {
            cy.visit('/trusts/overview/trust-details?uid=5527');

            // Wait for page to load
            cy.get('main, #main-content').should('be.visible');

            // Test search input with different scenarios
            cy.get('body').then($body => {
                if ($body.find('header input[type="search"], header input[type="text"]').length > 0) {
                    const searchInput = 'header input[type="search"], header input[type="text"]';

                    // Test with valid search term
                    cy.get(searchInput).first().clear();
                    cy.get(searchInput).first().type('west');
                    cy.checkAccessibility('header form, header');

                    // Test with no results scenario
                    cy.get(searchInput).first().clear();
                    cy.get(searchInput).first().type('NonExistentTerm');
                    cy.checkAccessibility('header form, header');

                    // Clear for clean state
                    cy.get(searchInput).first().clear();
                }
            });
        });
    });
}); 
