import { TableUtility } from "../tableUtility";

class PipelineAcademies {

    elements = {
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        emptyStateMessage: () => cy.get('[data-testid="empty-state-message"]'),
        downloadButton: () => cy.get('[data-testid="download-all-pipeline-data-button"]'),
        preAdvisory: {
            section: () => cy.get('[data-testid="pre-advisory-board-table"]'),
            table: () => this.elements.preAdvisory.section().find('[aria-describedby="pre-advisory-caption"]'),
            tableRows: () => this.elements.preAdvisory.table().find('tbody tr'),
            schoolName: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-school-name"]'),
            schoolNameHeader: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-school-name-header"]'),
            urn: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-URN"]'),
            urnHeader: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-URN-header"]'),
            ageRange: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-age-range"]'),
            ageRangeHeader: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-age-range-header"]'),
            localAuthority: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-local-authority"]'),
            localAuthorityHeader: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-local-authority-header"]'),
            projectType: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-project-type"]'),
            projectTypeHeader: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-project-type-header"]'),
            proposedConversionTransferDate: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-date"]'),
            proposedConversionTransferDateHeader: () => this.elements.preAdvisory.section().find('[data-testid="pre-advisory-board-proposed-conversion-transfer-date-header"]'),
        },
        postAdvisory: {
            section: () => cy.get('[data-testid="post-advisory-board-table"]'),
            table: () => this.elements.postAdvisory.section().find('[aria-describedby="post-advisory-caption"]'),
            tableRows: () => this.elements.postAdvisory.table().find('tbody tr'),
            schoolName: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-school-name"]'),
            schoolNameHeader: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-school-name-header"]'),
            urn: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-URN"]'),
            urnHeader: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-URN-header"]'),
            ageRange: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-age-range"]'),
            ageRangeHeader: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-age-range-header"]'),
            localAuthority: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-local-authority"]'),
            localAuthorityHeader: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-local-authority-header"]'),
            projectType: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-project-type"]'),
            projectTypeHeader: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-project-type-header"]'),
            proposedConversionTransferDate: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-conversion-transfer-date"]'),
            proposedConversionTransferDateHeader: () => this.elements.postAdvisory.section().find('[data-testid="post-advisory-board-proposed-conversion-transfer-date-header"]'),

        },
        freeSchools: {
            section: () => cy.get('[data-testid="free-schools-table"]'),
            table: () => this.elements.freeSchools.section().find('[aria-describedby="free-schools-caption"]'),
            tableRows: () => this.elements.freeSchools.table().find('tbody tr'),
            schoolName: () => this.elements.freeSchools.section().find('[data-testid="free-schools-board-school-name"]'),
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

    public checkPreAdvisoryPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Pre advisory board');
        return this;
    }

    public clickDownloadButton(): this {
        this.elements.downloadButton().click();
        return this;
    }

    public checkPreAdvisoryTableHeadersPresent(): this {
        this.elements.preAdvisory.schoolNameHeader().should('be.visible');
        this.elements.preAdvisory.urnHeader().should('be.visible');
        this.elements.preAdvisory.ageRangeHeader().should('be.visible');
        this.elements.preAdvisory.localAuthorityHeader().should('be.visible');
        this.elements.preAdvisory.projectTypeHeader().should('be.visible');
        this.elements.preAdvisory.proposedConversionTransferDateHeader().should('be.visible');
        return this;
    }

    public checkPreAdvisoryTableSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.preAdvisory.schoolName,
            this.elements.preAdvisory.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preAdvisory.urn,
            this.elements.preAdvisory.urnHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preAdvisory.ageRange,
            this.elements.preAdvisory.ageRangeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preAdvisory.localAuthority,
            this.elements.preAdvisory.localAuthorityHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preAdvisory.projectType,
            this.elements.preAdvisory.projectTypeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.preAdvisory.proposedConversionTransferDate,
            this.elements.preAdvisory.proposedConversionTransferDateHeader
        );
        return this;
    }

    public checkPostAdvisoryPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Post advisory board');
        return this;
    }

    public checkPostAdvisoryTableHeadersPresent(): this {
        this.elements.postAdvisory.schoolNameHeader().should('be.visible');
        this.elements.postAdvisory.urnHeader().should('be.visible');
        this.elements.postAdvisory.ageRangeHeader().should('be.visible');
        this.elements.postAdvisory.localAuthorityHeader().should('be.visible');
        this.elements.postAdvisory.projectTypeHeader().should('be.visible');
        this.elements.postAdvisory.proposedConversionTransferDateHeader().should('be.visible');
        return this;
    }

    public checkPostAdvisoryTableSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.postAdvisory.schoolName,
            this.elements.postAdvisory.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postAdvisory.urn,
            this.elements.postAdvisory.urnHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postAdvisory.ageRange,
            this.elements.postAdvisory.ageRangeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postAdvisory.localAuthority,
            this.elements.postAdvisory.localAuthorityHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postAdvisory.projectType,
            this.elements.postAdvisory.projectTypeHeader
        );
        TableUtility.checkStringSorting(
            this.elements.postAdvisory.proposedConversionTransferDate,
            this.elements.postAdvisory.proposedConversionTransferDateHeader
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

    public checkPreAdvisoryNoAcademyPresent(): this {
        this.elements.emptyStateMessage().should('contain', 'There are no pre advisory board academies in the pipeline for this trust');
        return this;
    }

    public checkPostAdvisoryNoAcademyPresent(): this {
        this.elements.emptyStateMessage().should('contain', 'There are no post advisory board academies in the pipeline for this trust');
        return this;
    }

    public checkFreeSchoolsNoAcademyPresent(): this {
        this.elements.emptyStateMessage().should('contain', 'There are no free schools in the pipeline for this trust.');
        return this;
    }

    public checkPreAdvisoryCorrectProjectTypePresent(): this {
        this.elements.preAdvisory.projectType().each(this.checkValueIsValidConversionTransfer);
        return this;
    }

    public checkPreAdvisoryCorrectConversionTransferDatePresent(): this {
        this.elements.preAdvisory.proposedConversionTransferDate().each(this.checkValueIsValidOpeningDate);
        return this;
    }

    public checkPostAdvisoryCorrectProjectTypePresent(): this {
        this.elements.postAdvisory.projectType().each(this.checkValueIsValidConversionTransfer);
        return this;
    }

    public checkPostAdvisoryCorrectConversionTransferDatePresent(): this {
        this.elements.postAdvisory.proposedConversionTransferDate().each(this.checkValueIsValidOpeningDate);
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

    public checkSchoolNamesAreCorrectLinksOnPreAdvisoryPage(): this {
        TableUtility.checkSchoolNamesAreCorrectLinksOnPage(this.elements.preAdvisory, "pre-advisory-board-school-name", "pre-advisory-board-URN");
        return this;
    }

    public checkSchoolNamesAreCorrectLinksOnPostAdvisoryPage(): this {
        TableUtility.checkSchoolNamesAreCorrectLinksOnPage(this.elements.postAdvisory, "post-advisory-board-school-name", "post-advisory-board-URN");
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

