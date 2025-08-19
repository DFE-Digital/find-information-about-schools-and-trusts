import commonPage from "../commonPage";

class SchoolOfstedPage {
    elements = {
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        downloadButton: () => cy.get('[data-testid="download-all-ofsted-data-button"]'),
        singleHeadlineGrades: {
            table: () => cy.get('[data-testid="ofsted-inspection-table"]'),
            joinedTrustText: () => cy.get('[data-testid="academy-joined-trust-text"]'),
            dateOfInspectionHeader: () => cy.get('[data-testid="date-of-inspection-header"]'),
            gradeHeader: () => cy.get('[data-testid="grade-header"]'),
            // Recent short inspection (conditional)
            recentShortInspectionRow: () => cy.get('[data-testid="recent-short-inspection-row"]'),
            recentShortInspectionDate: () => cy.get('[data-testid="recent-short-inspection-date"]'),
            recentShortInspectionGrade: () => cy.get('[data-testid="recent-short-inspection-grade"]'),
            // Current full inspection
            currentFullInspectionRow: () => cy.get('[data-testid="current-full-inspection-row"]'),
            currentFullInspectionDate: () => cy.get('[data-testid="current-full-inspection-date"]'),
            currentFullInspectionGrade: () => cy.get('[data-testid="current-full-inspection-grade"]'),
            // Previous full inspection
            previousFullInspectionRow: () => cy.get('[data-testid="previous-full-inspection-row"]'),
            previousFullInspectionDate: () => cy.get('[data-testid="previous-full-inspection-date"]'),
            previousFullInspectionGrade: () => cy.get('[data-testid="previous-full-inspection-grade"]'),
            // Details sections
            whySingleHeadlineNotAvailableDetails: () => cy.get('[data-testid="why-single-headline-not-available-details"]'),
            whyShortInspectionNotAvailableDetails: () => cy.get('[data-testid="why-short-inspection-not-available-details"]'),
            // Links
            inspectionReportsLink: () => cy.get('[data-testid="ofsted-inspection-reports-link"]'),
        },
        safeguardingAndConcerns: {
            // Summary list (not table)
            summaryList: () => cy.get('.govuk-summary-list'),
            // Details sections
            whatThisInformationMeansDetails: () => cy.get('[data-testid="what-this-information-means-details"]'),
            // Effective safeguarding
            effectiveSafeguardingRow: () => cy.get('[data-testid="effective-safeguarding-row"]'),
            effectiveSafeguardingLabel: () => cy.get('[data-testid="effective-safeguarding-label"]'),
            effectiveSafeguardingValue: () => cy.get('[data-testid="effective-safeguarding-value"]'),
            // Category of concern
            categoryOfConcernRow: () => cy.get('[data-testid="category-of-concern-row"]'),
            categoryOfConcernLabel: () => cy.get('[data-testid="category-of-concern-label"]'),
            categoryOfConcernValue: () => cy.get('[data-testid="category-of-concern-value"]'),
            // Before or after joining (academy-specific)
            beforeOrAfterJoiningRow: () => cy.get('[data-testid="before-or-after-joining-row"]'),
            beforeOrAfterJoiningLabel: () => cy.get('[data-testid="before-or-after-joining-label"]'),
            beforeOrAfterJoiningValue: () => cy.get('[data-testid="before-or-after-joining-value"]'),
        }
    };

    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().trim();
        expect(text).to.match(expected);
    };

    private readonly checkValueIsValidOfstedGrade = (element: JQuery<HTMLElement>) =>
        this.checkElementMatches(element, /^(School remains Good|Outstanding|Good|Requires improvement|Inadequate|Not yet inspected|Not inspected|Not available)$/);

    // Page header checks
    public checkOfstedSingleHeadlineGradesPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Single headline grades');
        return this;
    }

    // Table header checks
    public checkSingleHeadlineGradesTableHeadersPresent(): this {
        this.elements.singleHeadlineGrades.dateOfInspectionHeader().should('be.visible');
        this.elements.singleHeadlineGrades.gradeHeader().should('be.visible');
        return this;
    }

    // Academy joined trust text check
    public checkJoinedTrustTextPresent(): this {
        this.elements.singleHeadlineGrades.joinedTrustText().should('be.visible');
        return this;
    }

    // Recent short inspection checks (conditional)
    public checkRecentShortInspectionPresent(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionRow().should('be.visible');
        return this;
    }

    public checkRecentShortInspectionNotPresent(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionRow().should('not.exist');
        return this;
    }

    public checkRecentShortInspectionData(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionDate().should('be.visible');
        this.elements.singleHeadlineGrades.recentShortInspectionGrade().should('be.visible');
        return this;
    }

    public checkRecentShortInspectionGradeValid(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionGrade().each(this.checkValueIsValidOfstedGrade);
        return this;
    }

    // Current full inspection checks
    public checkCurrentFullInspectionPresent(): this {
        this.elements.singleHeadlineGrades.currentFullInspectionRow().should('be.visible');
        return this;
    }

    public checkCurrentFullInspectionData(): this {
        this.elements.singleHeadlineGrades.currentFullInspectionDate().should('be.visible');
        this.elements.singleHeadlineGrades.currentFullInspectionGrade().should('be.visible');
        return this;
    }

    public checkCurrentFullInspectionGradeValid(): this {
        this.elements.singleHeadlineGrades.currentFullInspectionGrade().each(this.checkValueIsValidOfstedGrade);
        return this;
    }

    // Previous full inspection checks
    public checkPreviousFullInspectionPresent(): this {
        this.elements.singleHeadlineGrades.previousFullInspectionRow().should('be.visible');
        return this;
    }

    public checkPreviousFullInspectionData(): this {
        this.elements.singleHeadlineGrades.previousFullInspectionDate().should('be.visible');
        this.elements.singleHeadlineGrades.previousFullInspectionGrade().should('be.visible');
        return this;
    }

    public checkPreviousFullInspectionGradeValid(): this {
        this.elements.singleHeadlineGrades.previousFullInspectionGrade().each(this.checkValueIsValidOfstedGrade);
        return this;
    }

    // Details sections checks
    public checkWhySingleHeadlineNotAvailableDetailsPresent(): this {
        this.elements.singleHeadlineGrades.whySingleHeadlineNotAvailableDetails().should('be.visible');
        return this;
    }

    public clickWhySingleHeadlineNotAvailableDetails(): this {
        this.elements.singleHeadlineGrades.whySingleHeadlineNotAvailableDetails().expandDetailsElement();
        return this;
    }

    public checkWhyShortInspectionNotAvailableDetailsPresent(): this {
        this.elements.singleHeadlineGrades.whyShortInspectionNotAvailableDetails().should('be.visible');
        return this;
    }

    public clickWhyShortInspectionNotAvailableDetails(): this {
        this.elements.singleHeadlineGrades.whyShortInspectionNotAvailableDetails().expandDetailsElement();
        return this;
    }

    public checkWhySingleHeadlineDetailsIsOpen(): this {
        this.elements.singleHeadlineGrades.whySingleHeadlineNotAvailableDetails().should('have.attr', 'open');
        return this;
    }

    public checkWhyShortInspectionDetailsIsOpen(): this {
        this.elements.singleHeadlineGrades.whyShortInspectionNotAvailableDetails().should('have.attr', 'open');
        return this;
    }

    // Inspection reports link check
    public checkInspectionReportsLinkPresent(): this {
        this.elements.singleHeadlineGrades.inspectionReportsLink().should('be.visible');
        return this;
    }

    public checkInspectionReportsLinkValid(): this {
        this.elements.singleHeadlineGrades.inspectionReportsLink()
        .should(($a) => {
            expect($a).to.have.attr('href').match(/^https:\/\/reports\.ofsted\.gov\.uk/);
            expect($a).to.have.attr('target', '_blank');
        });
        return this;
    }

    // Date validation methods
    public checkCurrentFullInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.currentFullInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    public checkPreviousFullInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.previousFullInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    public checkRecentShortInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    // Conditional checks for data-dependent elements
    public checkRecentShortInspectionGradeIfExists(): this {
        cy.get('body').then(($body) => {
            if ($body.find('[data-testid="recent-short-inspection-row"]').length > 0) {
                this.checkRecentShortInspectionGradeValid();
            }
        });
        return this;
    }

    public checkRecentShortInspectionDateIfExists(): this {
        cy.get('body').then(($body) => {
            if ($body.find('[data-testid="recent-short-inspection-row"]').length > 0) {
                this.checkRecentShortInspectionDateValid();
            }
        });
        return this;
    }

    // Download button - following same pattern as trust Ofsted page
    public clickDownloadButton(): this {
        this.elements.downloadButton().click();
        return this;
    }

    // ========= SAFEGUARDING AND CONCERNS METHODS =========

    /**
     * Checks that the safeguarding and concerns page header is present
     */
    public checkOfstedSafeguardingAndConcernsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Safeguarding and concerns');
        return this;
    }

    /**
     * Checks that the safeguarding and concerns summary list is visible
     */
    public checkSafeguardingAndConcernsTablePresent(): this {
        this.elements.safeguardingAndConcerns.summaryList().should('be.visible');
        return this;
    }

    // Effective safeguarding checks
    public checkEffectiveSafeguardingRowPresent(): this {
        this.elements.safeguardingAndConcerns.effectiveSafeguardingRow().should('be.visible');
        return this;
    }

    public checkEffectiveSafeguardingLabelPresent(): this {
        this.elements.safeguardingAndConcerns.effectiveSafeguardingLabel()
            .should('be.visible')
            .should('contain', 'Effective safeguarding');
        return this;
    }

    public checkEffectiveSafeguardingValuePresent(): this {
        this.elements.safeguardingAndConcerns.effectiveSafeguardingValue().should('be.visible');
        return this;
    }

    public checkEffectiveSafeguardingValueValid(): this {
        this.elements.safeguardingAndConcerns.effectiveSafeguardingValue()
            .each(($el) => {
                const text = $el.text().trim();
                expect(text).to.match(/^(Yes|No|Not recorded|Not yet inspected)$/);
            });
        return this;
    }

    // Category of concern checks
    public checkCategoryOfConcernRowPresent(): this {
        this.elements.safeguardingAndConcerns.categoryOfConcernRow().should('be.visible');
        return this;
    }

    public checkCategoryOfConcernLabelPresent(): this {
        this.elements.safeguardingAndConcerns.categoryOfConcernLabel()
            .should('be.visible')
            .should('contain', 'Category of concern');
        return this;
    }

    public checkCategoryOfConcernValuePresent(): this {
        this.elements.safeguardingAndConcerns.categoryOfConcernValue().should('be.visible');
        return this;
    }

    public checkCategoryOfConcernValueValid(): this {
        this.elements.safeguardingAndConcerns.categoryOfConcernValue()
            .each(($el) => {
                const text = $el.text().trim();
                expect(text).to.match(/^(None|Serious weaknesses|Special measures|Not available|Not yet inspected)$/);
            });
        return this;
    }

    // Before or after joining checks (Academy-specific)
    public checkBeforeOrAfterJoiningRowPresent(): this {
        this.elements.safeguardingAndConcerns.beforeOrAfterJoiningRow().should('be.visible');
        return this;
    }

    public checkBeforeOrAfterJoiningRowNotPresent(): this {
        this.elements.safeguardingAndConcerns.beforeOrAfterJoiningRow().should('not.exist');
        return this;
    }

    public checkBeforeOrAfterJoiningLabelPresent(): this {
        this.elements.safeguardingAndConcerns.beforeOrAfterJoiningLabel()
            .should('be.visible')
            .should('contain', 'Before or after joining');
        return this;
    }

    public checkBeforeOrAfterJoiningValuePresent(): this {
        this.elements.safeguardingAndConcerns.beforeOrAfterJoiningValue().should('be.visible');
        return this;
    }

    public checkBeforeOrAfterJoiningValueValid(): this {
        this.elements.safeguardingAndConcerns.beforeOrAfterJoiningValue()
            .each(($el) => {
                const text = $el.text().trim();
                expect(text).to.match(/^(Before joining|After joining|Not available|Not yet inspected)$/);
            });
        return this;
    }

    // Details sections checks
    public checkWhatThisInformationMeansDetailsPresent(): this {
        this.elements.safeguardingAndConcerns.whatThisInformationMeansDetails().should('be.visible');
        return this;
    }

    public clickWhatThisInformationMeansDetails(): this {
        this.elements.safeguardingAndConcerns.whatThisInformationMeansDetails().expandDetailsElement();
        return this;
    }

    public checkWhatThisInformationMeansDetailsIsOpen(): this {
        this.elements.safeguardingAndConcerns.whatThisInformationMeansDetails().should('have.attr', 'open');
        return this;
    }

    // ========= COMPREHENSIVE VALIDATION METHODS =========

    /**
     * Validates that all core safeguarding data elements are present
     */
    public checkAllSafeguardingDataPresent(): this {
        this.checkEffectiveSafeguardingRowPresent()
            .checkEffectiveSafeguardingLabelPresent()
            .checkEffectiveSafeguardingValuePresent()
            .checkCategoryOfConcernRowPresent()
            .checkCategoryOfConcernLabelPresent()
            .checkCategoryOfConcernValuePresent();
        return this;
    }

    /**
     * Validates that all safeguarding data values are in expected format
     */
    public checkAllSafeguardingDataValid(): this {
        this.checkEffectiveSafeguardingValueValid()
            .checkCategoryOfConcernValueValid();
        return this;
    }


}

const schoolOfstedPage = new SchoolOfstedPage();
export default schoolOfstedPage; 
