import { AutocompleteHelper } from '../../support/autocompleteHelper';

class WatchlistPage {

    elements = {
        pageHeading: () => cy.get('h1.govuk-heading-l'),
        addToWatchlistButton: () => cy.get('.govuk-button').contains('Add to watchlist'),
        removeFromWatchlistLink: () => cy.get('.govuk-link').contains('Remove'),
        selectSchool: () => cy.get('.govuk-radios__item').contains('School'),
        selectTrust: () => cy.get('.govuk-radios__item').contains('Trust'),
        continueButton: () => cy.get('[type="submit"]').contains('Continue'),
        searchInput: () => cy.get('.autocomplete__input.autocomplete__input--default'),
        establishmentDetails: () => cy.get('.govuk-summary-list'),
        saveAndCompleteButton: () => cy.get('.govuk-button').contains('Save and complete'),
        successMessage: () => cy.get('.govuk-notification-banner__content'),
        schoolSearch: () => cy.get('#school-search'),
        trustSearch: () => cy.get('#trust-search'),
        schoolAutocompleteResults: () => cy.get('#school-search__option--0'),
        trustAutocompleteResults: () => cy.get('#trust-search__option--0'),
        saveAndAddAnotherButton: () => cy.get('[data-cy="save-and-add-another-button"]').contains('Save and add another'),
        cancelAndGoBackToWatchlistLink: () => cy.get('.govuk-link').contains('Cancel and go back to watchlist'),
        removeSchoolButton: () => cy.get('.govuk-link').contains('Remove'),
        removeTrustButton: () => cy.get('.govuk-link').contains('Remove'),
        trustCount: () => cy.get('.moj-sub-navigation__item'),
        emptySchoolWatchlistMessage: () => cy.get('.govuk-body').contains('Your schools watchlist is currently empty'),
        emptyTrustWatchlistMessage: () => cy.get('.govuk-body').contains('Your trusts watchlist is currently empty'),
        schoolWatchlistTable: () => cy.get('.govuk-table[data-module="moj-sortable-table"]'),
        trustWatchlistTable: () => cy.get('.govuk-table[data-module="moj-sortable-table"]'),
        removeFromWatchlistButton: () => cy.get('[data-cy="remove-from-watchlist-button"]'),

        trustsTable: {
            table: () => cy.get('.govuk-table[data-module="moj-sortable-table"]'),
            noRecordsMessage: () => cy.get('.govuk-body').contains('No records found'),
            sortableHeaders: () => cy.get('.govuk-table[data-module="moj-sortable-table"] th[aria-sort]'),
            firstRecord: () => cy.get('.govuk-table__cell .govuk-link').first()
        },
        subNav: {
            schoolsTab: () => cy.get('.moj-sub-navigation__link').contains('Schools'),
            trustsTab: () => cy.get('.moj-sub-navigation__link').contains('Trusts'),
            activeTab: () => cy.get('.moj-sub-navigation__link[aria-current="page"]'),

        },
        schoolsTable: {
            table: () => cy.get('.govuk-table[data-module="moj-sortable-table"]'),
            sortableHeaders: () => cy.get('.govuk-table[data-module="moj-sortable-table"] th[aria-sort]'),
            body: () => cy.get('.govuk-table[data-module="moj-sortable-table"] .govuk-table__body'),
            noRecordsMessage: () => cy.get('.govuk-body').contains('No records found'),
            firstRecord: () => cy.get('.govuk-table__cell .govuk-link').first()
        },
    };

    public verifyHeading(headingText: string): this {
        this.elements.pageHeading().should('be.visible').and('contain', headingText);
        return this;
    }

    public watchlistPageAppears(): this {
        this.elements.pageHeading().should('be.visible').and('contain', 'My watchlist');
        return this;
    }

    public trustsTabAppearsByDefault(): this {
        this.elements.subNav.activeTab().should('contain', 'Trusts');
        return this;
    }

    public schoolTabContainsContent(): this {
        this.elements.schoolsTable.table().should('be.visible');
        return this;
    }

    public navigateToTrustsTab(): this {
        this.elements.subNav.trustsTab().click();
        this.elements.subNav.activeTab().should('contain', 'Trusts');
        return this;
    }

    public trustTabAppearsWithContent(): this {
        this.elements.subNav.activeTab().should('contain', 'Trusts');
        this.elements.trustsTable.table().should('be.visible');
        return this;
    }

    public navigateToSchoolTab(): this {
        this.elements.subNav.schoolsTab().click();
        this.elements.subNav.activeTab().should('contain', 'Schools');
        return this;
    }

    public clickAddToWatchlistButton(): this {
        this.elements.addToWatchlistButton().should('be.visible').click();
        return this;
    }

    public removeSchoolFromWatchlist(): this {
        this.elements.removeSchoolButton().click();
        cy.get('h1').should('contain', 'Remove school from watchlist')
        cy.get('a').contains('Cancel and go back to watchlist')
        this.elements.removeFromWatchlistButton().should('contain', 'Remove from watchlist').click()

        return this;
    }

    public removeTrustFromWatchlist(): this {
        this.elements.removeTrustButton().click()
        cy.get('h1').should('contain', 'Remove trust from watchlist')
        cy.get('a').contains('Cancel and go back to watchlist')
        cy.get('[data-cy="remove-from-watchlist-button"]').should('contain', 'Remove from watchlist').click()

        return this;
    }

    public sortColumnsInSchoolTab(): this {
        this.elements.schoolsTable.sortableHeaders().first().click();
        this.elements.schoolsTable.sortableHeaders().first().should('have.attr', 'aria-sort', 'ascending');

        return this;
    }

    public sortColumnsInTrustTab(): this {
        this.elements.trustsTable.sortableHeaders().first().click();
        this.elements.trustsTable.sortableHeaders().first().should('have.attr', 'aria-sort', 'ascending');

        return this;
    }

    public clickFirstSchoolInWatchlist(): this {
        this.elements.schoolsTable.firstRecord().click({});
        return this;
    }

    public schoolPageAppears(): this {
        cy.url().should('include', '/schools/overview');
        return this;
    }

    public trustPageAppears(): this {
        cy.wait(5000)
        cy.url().should('include', '/trusts/overview');
        return this;
    }

    public clickFirstTrustInWatchlist(): this {
        this.elements.trustsTable.firstRecord().click({});
        return this;
    }

    public trustTabDisplaysNoRecordsMessage(): this {
        this.elements.trustsTable.table().should('not.exist');
        this.elements.trustsTable.noRecordsMessage().should('be.visible');
        return this;
    }

    public schoolTabDisplaysNoRecordsMessage(): this {
        this.elements.schoolsTable.table().should('not.exist');
        this.elements.schoolsTable.noRecordsMessage().should('be.visible');
        return this;
    }

    public selectSchoolEstablishmentType(): this {
        cy.get('label').should('contain', 'Select the type of establishment to add');
        this.elements.selectSchool().click();
        return this;
    }

    public selectTrustEstablishmentType(): this {
        cy.get('label').should('contain', 'Select the type of establishment to add');
        this.elements.selectTrust().click();
        return this;
    }

    public clickContinue(): this {
        this.elements.continueButton().click();
        return this;
    }

    public searchForSchool(name: string): this {
        AutocompleteHelper.typeWithAutocomplete(this.elements.schoolSearch, name);
        this.selectSchoolOptionFromDropdown(name)
        return this;
    }

    public searchInvalidSchool(name: string): this {
        AutocompleteHelper.typeWithAutocomplete(this.elements.schoolSearch, name);
        return this;
    }

    public clickContinueButton(): this {
        cy.get('.autocomplete__option').should('contain', 'No results found');
        this.elements.continueButton().click({ force: true });
        return this;
    }

    public searchInvalidTrust(name: string): this {
        AutocompleteHelper.typeWithAutocomplete(this.elements.trustSearch, name);
        return this;
    }


    public verifyValidationMessageAppears(message: string): this {
        cy.get('.govuk-error-message').should('contain', message);
        return this;
    }

    public searchForTrust(name: string): this {
        AutocompleteHelper.typeWithAutocomplete(this.elements.trustSearch, name);
        this.selectTrustOptionFromDropdown(name);
        return this;
    }

    public confirmTrustDetails(details: string): this {
        cy.url().should('include','/confirm-trust');
        cy.get('h1').should('contain', 'Confirm trust details');
        this.elements.establishmentDetails().should('be.visible').and('not.be.empty');
        const expectedRows = ['Trust name', 'TRN', 'Region', 'Companies house number'];
        expectedRows.forEach((rowLabel) => {
            cy.get('.govuk-summary-list__key')
                .contains(rowLabel)
                .should('be.visible')
                .next('.govuk-summary-list__value')
                .should('not.be.empty');
        });

        return this;
    }

    public selectSchoolOptionFromDropdown(searchText: string): this {
        this.waitForAutocompleteResults();
        this.selectFirstAutocompleteResult();
        return this;
    }

    public selectTrustOptionFromDropdown(searchText: string): this {
        this.waitForTrustAutocompleteResults();
        this.selectFirstTrustAutocompleteResult();
        return this;
    }

    public confirmSchoolDetails(details: string): this {
        cy.url().should('include', '/confirm');
        cy.get('h1').should('contain', 'Confirm school details');
        this.elements.establishmentDetails().should('be.visible').and('not.be.empty');
        const expectedRows = [' Name', 'URN', 'Trust', 'Local authority'];
        expectedRows.forEach((rowLabel) => {
            cy.get('.govuk-summary-list__key')
                .contains(rowLabel)
                .should('be.visible')
                .next('.govuk-summary-list__value')
                .should('not.be.empty');
        });

        return this;
    }

    public clickSaveAndComplete(): this {
        this.elements.saveAndCompleteButton().click();
        return this;
    }

    public verifyButtonsOnConfirmationPage(): this {
        this.elements.saveAndCompleteButton().should('be.visible');
        this.elements.saveAndAddAnotherButton().should('be.visible');
        this.elements.cancelAndGoBackToWatchlistLink().should('be.visible');
        return this;
    }

    public successMessageAppears(message: string): this {
        this.elements.successMessage().should('contain', message);
        return this;
    }

    public watchlistNotEmpty(): this {
        this.elements.emptySchoolWatchlistMessage().should('not.contain', 'Watchlist is not empty');
        return this;
    }

    public schoolAppearsInWatchlistTable(schoolName: string): this {
        this.elements.schoolWatchlistTable().should('contain', schoolName);
        return this;
    }

    public waitForAutocompleteResults(): this {
        this.elements.schoolAutocompleteResults().should('be.visible');
        return this;
    }

    public waitForTrustAutocompleteResults(): this {
        this.elements.trustAutocompleteResults().should('be.visible');
        return this;
    }

    public trustCountIncreases(): this {
        this.elements.trustCount().invoke('text').then((text) => {
            const trimmedText = text.trim().replace(/\D/g, '');
            const count = Number.parseInt(trimmedText, 10);
            expect(count).to.be.greaterThan(0);
        });
        return this;
    }

    public selectFirstAutocompleteResult(): this {
        this.elements.schoolAutocompleteResults().first().click();
        return this;
    }

    public selectFirstTrustAutocompleteResult(): this {
        this.elements.trustAutocompleteResults().first().click();
        return this;
    }

    public successBannerAppears(message: string): this {
        this.elements.successMessage().should('contain', message);
        return this;
    }

}

const watchlistPage = new WatchlistPage();
export default watchlistPage;
