
import commonPage from "../commonPage";

class SchoolOfstedPage {
    elements = {
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        downloadButton: () => cy.get('[data-testid="download-all-ofsted-data-button"]'),
        singleHeadlineGrades: {
            table: () => cy.get('[data-testid="ofsted-inspection-table"]'),
            joinedTrustText: () => cy.get('[data-testid="academy-joined-trust-text"]'),
            dateOfInspectionHeader: () => cy.get('[data-testid="date-of-inspection-header"]'),
            inspectionTypeHeader: () => cy.get('[data-testid="inspection-type-header"]'),
            // Recent short inspection (conditional)
            recentShortInspectionRow: () => cy.get('[data-testid="recent-short-inspection-row"]'),
            recentShortInspectionDate: () => cy.get('[data-testid="recent-short-inspection-date"]'),
            recentShortInspectionGrade: () => cy.get('[data-testid="recent-short-inspection-grade"]'),
            // Current inspection
            currentInspectionRow: () => cy.get('[data-testid="current-inspection-row"]'),
            currentInspectionDate: () => cy.get('[data-testid="current-inspection-date"]'),
            currentInspectionGrade: () => cy.get('[data-testid="current-inspection-grade"]'),
            // Previous inspection
            previousInspectionRow: () => cy.get('[data-testid="previous-inspection-row"]'),
            previousInspectionDate: () => cy.get('[data-testid="previous-inspection-date"]'),
            previousInspectionGrade: () => cy.get('[data-testid="previous-inspection-grade"]'),
            // Details sections
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
            beforeOrAfterJoiningRow: () => cy.get('[data-testid="before-or-after-joining"]'),
            beforeOrAfterJoiningLabel: () => cy.get('[data-testid="before-or-after-joining-label"]'),
            beforeOrAfterJoiningValue: () => cy.get('[data-testid="before-or-after-joining-value"]'),
        }
    };

    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().replace(/\s+/g, ' ').trim();
        expect(text).to.match(expected);
    };

    private readonly checkValueIsValidOfstedGrade = (element: JQuery<HTMLElement>) => {
        const text = element.text().replace(/\s+/g, ' ').trim();
        // Inspection type column can show headline grades, or "Report card" / "Older inspection" for newer/older formats
        expect(text).to.match(/^(Not yet inspected|No |Not available|Report card|Older inspection)?$/i);
    };

    // Page header checks
    public checkOfstedOverviewPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Overview');
        return this;
    }

    // Table header checks
    public checkOfstedOverviewTableHeadersPresent(): this {
        this.elements.singleHeadlineGrades.dateOfInspectionHeader().should('be.visible');
        this.elements.singleHeadlineGrades.inspectionTypeHeader().should('be.visible');
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

    // Current inspection checks
    public checkCurrentInspectionPresent(): this {
        this.elements.singleHeadlineGrades.currentInspectionRow().should('be.visible');
        return this;
    }

    public checkCurrentInspectionData(): this {
        this.elements.singleHeadlineGrades.currentInspectionDate().should('be.visible');
        this.elements.singleHeadlineGrades.currentInspectionGrade().should('be.visible');
        return this;
    }

    public checkCurrentInspectionGradeValid(): this {
        this.elements.singleHeadlineGrades.currentInspectionGrade().each(this.checkValueIsValidOfstedGrade);
        return this;
    }

    // Previous inspection checks
    public checkPreviousInspectionPresent(): this {
        this.elements.singleHeadlineGrades.previousInspectionRow().should('be.visible');
        return this;
    }

    public checkPreviousInspectionData(): this {
        this.elements.singleHeadlineGrades.previousInspectionDate().should('be.visible');
        this.elements.singleHeadlineGrades.previousInspectionGrade().should('be.visible');
        return this;
    }

    public checkPreviousInspectionGradeValid(): this {
        this.elements.singleHeadlineGrades.previousInspectionGrade().each(this.checkValueIsValidOfstedGrade);
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

    public checkWhyShortInspectionDetailsIsOpen(): this {
        this.elements.singleHeadlineGrades.whyShortInspectionNotAvailableDetails().should('have.attr', 'open');
        return this;
    }

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
    public checkCurrentInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.currentInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    public checkPreviousInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.previousInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    public checkRecentShortInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    // ========= SAFEGUARDING AND CONCERNS METHODS =========

    /**
     * Checks that the safeguarding and concerns page header is present
     */

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
                expect(text).to.match(/^(Yes|No|Not recorded|Not yet inspected|Met|None)$/);
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
                const text = $el.text().replace(/\s+/g, ' ').trim();
                expect(text).to.match(/^(Before joining|After joining|Not available|Not yet inspected inspection has not yet taken place)$/i);
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
