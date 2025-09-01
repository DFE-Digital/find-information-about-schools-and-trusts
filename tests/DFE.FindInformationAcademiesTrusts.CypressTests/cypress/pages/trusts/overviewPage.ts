class OverviewPage {

    elements = {
        overviewHeader: () => cy.get('[data-testid="page-name"]'),
        trustSummaryCard: () => cy.get('[data-testid="trust-summary"]'),
        overviewOfstedSummaryCardContentBox: () => cy.get('[data-testid="ofsted-ratings"]'),
        tableRowSortValues: () => cy.get('tbody.govuk-table__body tr td[data-sort-value]'),
        firstRowRatingText: () => cy.get('tbody.govuk-table__body tr:first-child td:first-child'),
        detailsHeader: () => cy.get('[data-testid="page-name"]'),
        trustDetails: () => cy.get('[data-testid="trust-details-summary"]'),
        referenceNumberCard: () => cy.get('[data-testid="reference-numbers-summary-card"]'),
        trustDetailsSubHeader: () => cy.get('[data-testid="reference-numbers-summary-card"]'),
        informationForOtherServicesHeader: () => cy.get('[data-testid="details-information-from-other-services-header"]'),
        giasLink: () => cy.get('[data-testid="details-gias-link"]'),
        financialBenchmarkingLink: () => cy.get('[data-testid="details-financial-benchmarking-link"]'),
        findSchoolPerformanceDataLink: () => cy.get('[data-testid="details-find-school-performance-link"]'),
        subNav: {
            trustDetailsSubnavButton: () => cy.get('[data-testid="overview-trust-details-subnav"]'),
            trustSummarySubnavButton: () => cy.get('[data-testid="overview-trust-summary-subnav"]'),
            referenceNumbersSubnavButton: () => cy.get('[data-testid="overview-reference-numbers-subnav"]'),
        },
        subHeaders: {
            subHeader: () => cy.get('[data-testid="subpage-header"]'),
        },
    };

    public checkOverviewHeaderPresent(): this {
        this.elements.overviewHeader().should('be.visible');
        this.elements.overviewHeader().should('contain', 'Overview');
        return this;
    }

    public checkTrustSummaryCardItemsPresent(): this {
        this.elements.trustSummaryCard().should('contain', 'Total academies');
        this.elements.trustSummaryCard().should('contain', 'Academies in each local authority');
        this.elements.trustSummaryCard().should('contain', 'Pupil numbers');
        this.elements.trustSummaryCard().should('contain', 'Pupil capacity');
        return this;
    }

    public checkTrustSummaryCardPresent(): this {
        this.elements.trustSummaryCard().should('be.visible');
        this.elements.trustSummaryCard().should('contain', 'Trust summary');
        return this;
    }

    public checkTrustDetailsPresent(): this {
        this.elements.trustDetails().should('be.visible');
        this.elements.trustDetails().should('contain', 'Trust details');
        return this;
    }

    public checkTrustDetailsItemsPresent(): this {
        this.elements.trustDetails().should('contain', 'Address');
        this.elements.trustDetails().should('contain', 'Opened on');
        this.elements.trustDetails().should('contain', 'Region and territory');
        return this;
    }

    public checkInformationFromOtherServicesPresent(): this {
        this.elements.giasLink().should('be.visible').and('contain.text', 'Get information about schools');
        this.elements.financialBenchmarkingLink().should('be.visible').and('contain.text', 'Financial benchmarking');
        this.elements.findSchoolPerformanceDataLink().should('be.visible').and('contain.text', 'Find school college and performance data');
        return this;
    }

    public checkReferenceNumbersCardPresent(): this {
        this.elements.referenceNumberCard().should('be.visible');
        this.elements.referenceNumberCard().should('contain', 'Reference numbers');
        return this;
    }

    public checkReferenceNumbersCardItemsPresent(): this {
        this.elements.referenceNumberCard().should('contain', 'UID');
        this.elements.referenceNumberCard().should('contain', 'Group ID');
        this.elements.referenceNumberCard().should('contain', 'UKPRN');
        this.elements.referenceNumberCard().should('contain', 'Companies House number');
        return this;
    }

    public checkAllSubNavItemsPresent(): this {
        this.elements.subNav.trustDetailsSubnavButton().should('be.visible');
        this.elements.subNav.trustSummarySubnavButton().should('be.visible');
        this.elements.subNav.referenceNumbersSubnavButton().should('be.visible');
        return this;
    }

    public checkSubNavNotPresent(): this {
        this.elements.subNav.trustDetailsSubnavButton().should('not.exist');
        this.elements.subNav.trustSummarySubnavButton().should('not.exist');
        this.elements.subNav.referenceNumbersSubnavButton().should('not.exist');
        return this;
    }

    public clickTrustDetailsSubnavButton(): this {
        this.elements.subNav.trustDetailsSubnavButton().click();
        return this;
    }

    public clickTrustSummarySubnavButton(): this {
        this.elements.subNav.trustSummarySubnavButton().click();
        return this;
    }

    public clickReferenceNumbersSubnavButton(): this {
        this.elements.subNav.referenceNumbersSubnavButton().click();
        return this;
    }

    public checkTrustDetailsSubnavButtonIsHighlighted(): this {
        this.elements.subNav.trustDetailsSubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    public checkTrustSummarySubnavButtonIsHighlighted(): this {
        this.elements.subNav.trustSummarySubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    public checkReferenceNumbersSubnavButtonIsHighlighted(): this {
        this.elements.subNav.referenceNumbersSubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    public checkTrustDetailsSubHeaderPresent(): this {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Trust details');
        return this;
    }

    public checkTrustSummarySubHeaderPresent(): this {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Trust summary');
        return this;
    }

    public checkReferenceNumbersSubHeaderPresent(): this {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Reference numbers');
        return this;
    }

}

const overviewPage = new OverviewPage();
export default overviewPage;
