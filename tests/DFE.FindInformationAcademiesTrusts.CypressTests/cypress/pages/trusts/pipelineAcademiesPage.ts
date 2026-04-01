import { TableUtility } from "../tableUtility";

class PipelineAcademies {

    elements = {
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        emptyStateMessage: () => cy.get('[data-testid="empty-state-message"]'),
        downloadButton: () => cy.get('[data-testid="download-all-pipeline-data-button"]'),
        preDecision: {
            section: () => cy.get('[data-testid="pre-decision-table"]'),
            table: () => this.elements.preDecision.section().find('[aria-describedby="pre-decision-caption"]'),
            tableRows: () => this.elements.preDecision.table().find('tbody tr'),
            schoolName: () => this.elements.preDecision.section().find('[data-testid="pre-decision-school-name"]'),
            schoolNameHeader: () => this.elements.preDecision.section().find('[data-testid="pre-decision-school-name-header"]'),
            urn: () => this.elements.preDecision.section().find('[data-testid="pre-decision-URN"]'),
            urnHeader: () => this.elements.preDecision.section().find('[data-testid="pre-decision-URN-header"]'),
            ageRange: () => this.elements.preDecision.section().find('[data-testid="pre-decision-age-range"]'),
            ageRangeHeader: () => this.elements.preDecision.section().find('[data-testid="pre-decision-age-range-header"]'),
            localAuthority: () => this.elements.preDecision.section().find('[data-testid="pre-decision-local-authority"]'),
            localAuthorityHeader: () => this.elements.preDecision.section().find('[data-testid="pre-decision-local-authority-header"]'),
            projectType: () => this.elements.preDecision.section().find('[data-testid="pre-decision-project-type"]'),
            projectTypeHeader: () => this.elements.preDecision.section().find('[data-testid="pre-decision-project-type-header"]'),
            proposedConversionTransferDate: () => this.elements.preDecision.section().find('[data-testid="pre-decision-date"]'),
            proposedConversionTransferDateHeader: () => this.elements.preDecision.section().find('[data-testid="pre-decision-proposed-conversion-transfer-date-header"]'),
        },
        postDecision: {
            section: () => cy.get('[data-testid="post-decision-table"]'),
            table: () => this.elements.postDecision.section().find('[aria-describedby="post-decision-caption"]'),
            tableRows: () => this.elements.postDecision.table().find('tbody tr'),
            schoolName: () => this.elements.postDecision.section().find('[data-testid="post-decision-school-name"]'),
            schoolNameHeader: () => this.elements.postDecision.section().find('[data-testid="post-decision-school-name-header"]'),
            urn: () => this.elements.postDecision.section().find('[data-testid="post-decision-URN"]'),
            urnHeader: () => this.elements.postDecision.section().find('[data-testid="post-decision-URN-header"]'),
            ageRange: () => this.elements.postDecision.section().find('[data-testid="post-decision-age-range"]'),
            ageRangeHeader: () => this.elements.postDecision.section().find('[data-testid="post-decision-age-range-header"]'),
            localAuthority: () => this.elements.postDecision.section().find('[data-testid="post-decision-local-authority"]'),
            localAuthorityHeader: () => this.elements.postDecision.section().find('[data-testid="post-decision-local-authority-header"]'),
            projectType: () => this.elements.postDecision.section().find('[data-testid="post-decision-project-type"]'),
            projectTypeHeader: () => this.elements.postDecision.section().find('[data-testid="post-decision-project-type-header"]'),
            proposedConversionTransferDate: () => this.elements.postDecision.section().find('[data-testid="post-decision-conversion-transfer-date"]'),
            proposedConversionTransferDateHeader: () => this.elements.postDecision.section().find('[data-testid="post-decision-proposed-conversion-transfer-date-header"]'),

        },
        freeSchools: {
            section: () => cy.get('[data-testid="free-schools-table"]'),
            table: () => this.elements.freeSchools.section().find('[aria-describedby="free-schools-caption"]'),
            tableRows: () => this.elements.freeSchools.table().find('tbody tr'),
            schoolName: () => this.elements.freeSchools.section().find('[data-testid="free-schools-school-name"]'),
            schoolNameHeader: () => this.elements.freeSchools.section().find('[data-testid="free-schools-school-name-header"]'),
            urn: () => this.elements.freeSchools.section().find('[data-testid="free-schools-URN"]'),
            urnHeader: () => this.elements.freeSchools.section().find('[data-testid="free-schools-URN-header"]'),
            ageRange: () => this.elements.freeSchools.section().find('[data-testid="free-schools-age-range"]'),
            ageRangeHeader: () => this.elements.freeSchools.section().find('[data-testid="free-schools-age-range-header"]'),
            localAuthority: () => this.elements.freeSchools.section().find('[data-testid="free-schools-local-authority"]'),
            localAuthorityHeader: () => this.elements.freeSchools.section().find('[data-testid="free-schools-local-authority-header"]'),
            projectType: () => this.elements.freeSchools.section().find('[data-testid="free-schools-project-type"]'),
            projectTypeHeader: () => this.elements.freeSchools.section().find('[data-testid="free-schools-project-type-header"]'),
            provisionalOpeningDate: () => this.elements.freeSchools.section().find('[data-testid="free-schools-provisional-opening-date"]'),
            provisionalOpeningDateHeader: () => this.elements.freeSchools.section().find('[data-testid="free-schools-provisional-opening-date-header"]'),
        },

    };

    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().trim();
        expect(text).to.match(expected);
    };

    private readonly checkValueIsValidConversionTransfer = (element: JQuery<HTMLElement>) =>
        this.checkElementMatches(element, /^(Conversion|Transfer)$/);

    private readonly checkValueIsValidOpeningDate = (element: JQuery<HTMLElement>) => {
        const text = element.text().trim();

        // Resolves to a date ({2 digits} {month} {4 digits}) or "Unconfirmed" string
        // Tech debt - We are allowing Sep and Sept due to different cultures set on remote vs local builds
        expect(text).to.match(/^\d{1,2} (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec) \d{4}$|^Unconfirmed$/);
    };

    public checkPreDecisionPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Pre decision');
        return this;
    }

    public checkPreDecisionTableHeadersPresent(): this {
        this.elements.preDecision.schoolNameHeader().should('be.visible');
        this.elements.preDecision.urnHeader().should('be.visible');
        this.elements.preDecision.ageRangeHeader().should('be.visible');
        this.elements.preDecision.localAuthorityHeader().should('be.visible');
        this.elements.preDecision.projectTypeHeader().should('be.visible');
        this.elements.preDecision.proposedConversionTransferDateHeader().should('be.visible');
        return this;
    }

    public checkPreDecisionTableSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.preDecision.schoolName,
            this.elements.preDecision.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preDecision.urn,
            this.elements.preDecision.urnHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preDecision.ageRange,
            this.elements.preDecision.ageRangeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preDecision.localAuthority,
            this.elements.preDecision.localAuthorityHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preDecision.projectType,
            this.elements.preDecision.projectTypeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preDecision.proposedConversionTransferDate,
            this.elements.preDecision.proposedConversionTransferDateHeader
        );
        return this;
    }

    public checkPostDecisionPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Post decision');
        return this;
    }

    public checkPostDecisionTableHeadersPresent(): this {
        this.elements.postDecision.schoolNameHeader().should('be.visible');
        this.elements.postDecision.urnHeader().should('be.visible');
        this.elements.postDecision.ageRangeHeader().should('be.visible');
        this.elements.postDecision.localAuthorityHeader().should('be.visible');
        this.elements.postDecision.projectTypeHeader().should('be.visible');
        this.elements.postDecision.proposedConversionTransferDateHeader().should('be.visible');
        return this;
    }

    public checkPostDecisionTableSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.postDecision.schoolName,
            this.elements.postDecision.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postDecision.urn,
            this.elements.postDecision.urnHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postDecision.ageRange,
            this.elements.postDecision.ageRangeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postDecision.localAuthority,
            this.elements.postDecision.localAuthorityHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postDecision.projectType,
            this.elements.postDecision.projectTypeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postDecision.proposedConversionTransferDate,
            this.elements.postDecision.proposedConversionTransferDateHeader
        );
        return this;
    }

    public checkFreeSchoolsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Free schools');
        return this;
    }

    public checkFreeSchoolsTableHeadersPresent(): this {
        this.elements.freeSchools.schoolNameHeader().should('be.visible');
        this.elements.freeSchools.urnHeader().should('be.visible');
        this.elements.freeSchools.ageRangeHeader().should('be.visible');
        this.elements.freeSchools.localAuthorityHeader().should('be.visible');
        this.elements.freeSchools.projectTypeHeader().should('be.visible');
        this.elements.freeSchools.provisionalOpeningDateHeader().should('be.visible');
        return this;
    }

    public checkFreeSchoolsTableSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.freeSchools.schoolName,
            this.elements.freeSchools.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.freeSchools.urn,
            this.elements.freeSchools.urnHeader
        );
        TableUtility.checkStringSorting(
            this.elements.freeSchools.ageRange,
            this.elements.freeSchools.ageRangeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.freeSchools.localAuthority,
            this.elements.freeSchools.localAuthorityHeader
        );
        TableUtility.checkStringSorting(
            this.elements.freeSchools.projectType,
            this.elements.freeSchools.projectTypeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.freeSchools.provisionalOpeningDate,
            this.elements.freeSchools.provisionalOpeningDateHeader
        );
        return this;
    }

    public checkPreDecisionNoAcademyPresent(): this {
        this.elements.emptyStateMessage().should('contain', 'There are no pre decision academies in the pipeline for this trust');
        return this;
    }

    public checkPostDecisionNoAcademyPresent(): this {
        this.elements.emptyStateMessage().should('contain', 'There are no post decision academies in the pipeline for this trust');
        return this;
    }

    public checkFreeSchoolsNoAcademyPresent(): this {
        this.elements.emptyStateMessage().should('contain', 'There are no free schools in the pipeline for this trust.');
        return this;
    }

    public checkPreDecisionCorrectProjectTypePresent(): this {
        this.elements.preDecision.projectType().each(this.checkValueIsValidConversionTransfer);
        return this;
    }

    public checkPreDecisionCorrectConversionTransferDatePresent(): this {
        this.elements.preDecision.proposedConversionTransferDate().each(this.checkValueIsValidOpeningDate);
        return this;
    }

    public checkPostDecisionCorrectProjectTypePresent(): this {
        this.elements.postDecision.projectType().each(this.checkValueIsValidConversionTransfer);
        return this;
    }

    public checkPostDecisionCorrectConversionTransferDatePresent(): this {
        this.elements.postDecision.proposedConversionTransferDate().each(this.checkValueIsValidOpeningDate);
        return this;
    }

    public checkFreeSchoolsCorrectProjectTypePresent(): this {
        this.elements.freeSchools.projectType().each((element: JQuery<HTMLElement>) =>
            this.checkElementMatches(element, /^(Central|Presumption)$/));

        return this;
    }

    public checkFreeSchoolsCorrectProvisionalOpenDatePresent(): this {
        this.elements.freeSchools.provisionalOpeningDate().each(this.checkValueIsValidOpeningDate);
        return this;
    }

    public checkSchoolNamesAreCorrectLinksOnPreDecisionPage(): this {
        TableUtility.checkSchoolNamesAreCorrectLinksOnPage(this.elements.preDecision, "pre-decision-school-name", { path: "/schools/overview/details", urnTestId: "pre-decision-URN" });
        return this;
    }

    public checkSchoolNamesAreCorrectLinksOnPostDecisionPage(): this {
        TableUtility.checkSchoolNamesAreCorrectLinksOnPage(this.elements.postDecision, "post-decision-school-name", { path: "/schools/overview/details", urnTestId: "post-decision-URN" });
        return this;
    }

    public checkSchoolNamesAreNotLinksOnFreeSchools(): this {
        this.elements.freeSchools.schoolName().each(element => {
            expect(element.children('a').length).to.equal(0);
            expect(element.text().trim()).to.not.be.empty;
        });
        return this;
    }
}

const pipelineAcademiesPage = new PipelineAcademies();
export default pipelineAcademiesPage;

