class FinancialDocuments {

    elements = {
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        financialYear: () => cy.get('[data-testid="financial-docs-financial-year"]'),
        financialStatus: () => cy.get('[data-testid="financial-docs-financial-status-or-link"]'),
        aboutTheseDocuments: () => cy.get('[data-testid="about-these-documents"]'),
        internalUseOnlyMessage: () => cy.get('[data-testid="internal-use-only-warning"]'),
        permissionMessage: () => cy.get('[data-testid="you-must-have-permission-message"]'),
    };

    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().trim();
        expect(text).to.match(expected);
    };

    private readonly checkValueIsValidFinancialYear = (element: JQuery<HTMLElement>) =>
        this.checkElementMatches(element, /^(20\d{2} to 20\d{2})$/);

    public checkFinancialStatementsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Financial statements');
        return this;
    }

    public checkManagementLettersPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Management letters');
        return this;
    }

    public checkInternalScrutinyReportsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Internal scrutiny reports');
        return this;
    }

    public checkSelfAssessmentChecklistsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Self-assessment checklists');
        return this;
    }

    public checkFinancialStatementsCorrectStatusTypePresent(): this {
        this.elements.financialStatus().each((element: JQuery<HTMLElement>) =>
            this.checkElementMatches(element, /^(Not yet submitted|Not submitted|Not expected|View\s+\d{4} to \d{4}\s+financial statement \(opens in a new tab\))$/));

        return this;
    }

    public checkManagementLettersCorrectStatusTypePresent(): this {
        this.elements.financialStatus().each((element: JQuery<HTMLElement>) =>
            this.checkElementMatches(element, /^(Not yet submitted|Not submitted|Not expected|View\s+\d{4} to \d{4}\s+management letter \(opens in a new tab\))$/));

        return this;
    }

    public checkInternalScrutinyReportCorrectStatusTypePresent(): this {
        this.elements.financialStatus().each((element: JQuery<HTMLElement>) =>
            this.checkElementMatches(element, /^(Not yet submitted|Not submitted|Not expected|View\s+\d{4} to \d{4}\s+scrutiny report \(opens in a new tab\))$/));

        return this;
    }

    public checkSelfAssessmentCorrectStatusTypePresent(): this {
        this.elements.financialStatus().each((element: JQuery<HTMLElement>) =>
            this.checkElementMatches(element, /^(Not yet submitted|Not submitted|Not expected|View\s+\d{4} to \d{4}\s+self-assessment checklist \(opens in a new tab\))$/));

        return this;
    }

    public checkFinancialDocumentsCorrectYearRangePresent(): this {
        this.elements.financialYear().each(this.checkValueIsValidFinancialYear);
        return this;
    }

    public checkHasAboutTheseDocumentsComponent(): this {
        this.elements.aboutTheseDocuments().should('be.visible');
        return this;
    }

    public checkAboutTheseDocumentsComponentDetails(): this {
        // Expand the details element so we can see its contents on any screenshots if this fails
        this.elements.aboutTheseDocuments().expandDetailsElement();

        // Check for the presence of each line of text
        const expectedTexts = [
            "Requesting access or reporting a problem",
            "Financial documents are maintained by the Data Science team.",
            "Do not contact a trust about documents that are missing or in the wrong format.",
            "To report a problem with a document, or if you have a business need to be granted access, email:",
            "academiesfinancialmonitoring@education.gov.uk.",
            "Why a document might not be expected",
            "If a trust could not have submitted a document, for example because it had not formed yet, we say that it was ‘not expected’."
        ];

        expectedTexts.forEach(text => {
            this.elements.aboutTheseDocuments().contains(text).should('exist');
        });

        return this;
    }
    
    public checkAboutTheseDocumentsDoesNotHaveAboutDataLink(): this {
        this.elements.aboutTheseDocuments().expandDetailsElement();
        
        this.elements.aboutTheseDocuments().find('a[href="/aboutdata"]').should('not.exist');
        
        return this;
    }

    public checkForCorrectInternalUseMessage(internalUseMessage: string): this {
        this.elements.internalUseOnlyMessage().should('contain.text', internalUseMessage);
        return this;
    }

    public checkForNoInternalUseMessage(): this {
        this.elements.internalUseOnlyMessage().should('not.exist');
        return this;
    }

    public checkForCorrectPermissionMessage(): this {
        this.elements.permissionMessage().should('contain.text', 'You must have permission to view these\n      documents.');
        return this;
    }

}

const financialDocumentsPage = new FinancialDocuments();
export default financialDocumentsPage;
