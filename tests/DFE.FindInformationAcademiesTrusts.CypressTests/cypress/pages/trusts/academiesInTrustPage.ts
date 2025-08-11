import commonPage from "../commonPage";
import { TableUtility } from "../tableUtility";

class AcademiesInTrustPage {

    elements = {
        pageTabs: {
            academyCountLabel: () => cy.get('[data-testid="academies-nav"]'),
        },
        detailsPage: this.createDetailsPageElements(),
        pupilNumbersPage: this.createPupilNumbersPageElements(),
        freeSchoolMeals: this.createFreeSchoolMealsElements(),
    };

    private createDetailsPageElements() {
        const table = () => cy.get('[aria-describedby="details-caption"]');
        return {
            table,
            tableRows: () => table().find('tbody tr'),
            schoolName: () => table().find('[data-testid="school-name"]'),
            schoolNameHeader: () => table().find("th:contains('School name')"),
            urn: () => table().find('[data-testid="urn"]'),
            urnHeader: () => table().find("th:contains('URN')"),
            dateJoinedTrust: () => table().find('[data-testid="academy-date-joined"]'),
            dateJoinedTrustHeader: () => table().find("th:contains('Date joined trust')"),
            localAuthority: () => table().find('[data-testid="local-authority"]'),
            localAuthorityHeader: () => table().find("th:contains('Local authority')"),
            schoolType: () => table().find('[data-testid="type-of-establishment"]'),
            schoolTypeHeader: () => table().find("th:contains('Type')"),
            ruralOrUrban: () => table().find('[data-testid="urban-or-rural"]'),
            ruralOrUrbanHeader: () => table().find("th:contains('Rural or urban')"),
        };
    }

    private createPupilNumbersPageElements() {
        const table = () => cy.get('[aria-describedby="pupil-numbers-caption"]');
        return {
            table,
            tableRows: () => table().find('tbody tr'),
            schoolName: () => table().find('[data-testid="school-name"]'),
            schoolNameHeader: () => table().find("th:contains('School name')"),
            urn: () => table().find('[data-testid="urn"]'),
            urnHeader: () => table().find("th:contains('URN')"),
            phaseAndAge: () => table().find('[data-testid="phase-and-age-range"]'),
            phaseAndAgeHeader: () => table().find("th:contains('Phase and age range')"),
            pupilNumbers: () => table().find('[data-testid="pupil-numbers"]'),
            pupilNumbersHeader: () => table().find("th:contains('Pupil numbers')"),
            pupilCapacity: () => table().find('[data-testid="pupil-capacity"]'),
            pupilCapacityHeader: () => table().find("th:contains('Pupil capacity')"),
        };
    }

    private createFreeSchoolMealsElements() {
        const table = () => cy.get('[aria-describedby="free-school-meals-caption"]');
        return {
            table,
            tableRows: () => table().find('tbody tr'),
            schoolName: () => table().find('[data-testid="school-name"]'),
            schoolNameHeader: () => table().find("th:contains('School name')"),
            urn: () => table().find('[data-testid="urn"]'),
            urnHeader: () => table().find("th:contains('URN')"),
            pupilsEligible: () => table().find('[data-testid="pupils-eligible"]'),
            pupilsEligibleHeader: () => table().find("th:contains('Pupils eligible for free school meals')"),
            localAuthorityAverage: () => table().find('[data-testid="local-authority-average"]'),
            localAuthorityAverageHeader: () => table().find("th:contains('Local authority average')"),
            nationalAverage: () => table().find('[data-testid="national-average"]'),
            nationalAverageHeader: () => table().find("th:contains('National average')"),
        };
    }

    public getAcademyCountFromSidebar(): Cypress.Chainable<number> {
        return this.elements.pageTabs.academyCountLabel()
            .invoke('text')
            .then(text => parseInt((/\d+/.exec(text))[0]));
    }

    public getTableRowCountOnDetailsPage(): Cypress.Chainable<number> {
        return this.elements.detailsPage.tableRows().its('length');
    }

    public getTableRowCountOnPupilNumbersPage(): Cypress.Chainable<number> {
        return this.elements.pupilNumbersPage.tableRows().its('length');
    }

    public getTableRowCountOnFreeSchoolMealsPage(): Cypress.Chainable<number> {
        return this.elements.freeSchoolMeals.tableRows().its('length');
    }

    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().trim();
        expect(text).to.match(expected);
    };

    public checkValueIsValidPhaseType = (element: JQuery<HTMLElement>) =>
        this.checkElementMatches(element, /^(16 plus|All-through|Middle deemed primary|Middle deemed secondary|Not applicable|Nursery|Primary|Secondary)(\s+\d+-\d+)?$/);

    public checkCorrectPhaseTypePresent(): this {
        const { pupilNumbersPage } = this.elements;
        pupilNumbersPage.phaseAndAge().each(this.checkValueIsValidPhaseType);
        return this;
    }

    public checkDetailsHeadersPresent(): this {
        const { detailsPage } = this.elements;
        detailsPage.table().should('contain', 'School name')
            .and('contain', 'URN')
            .and('contain', 'Local authority')
            .and('contain', 'Type')
            .and('contain', 'Rural or urban');
        return this;
    }
    
    public checkSchoolNamesAreCorrectLinksOnDetailsPage(): this {
        TableUtility.checkSchoolNamesAreCorrectLinksOnPage(this.elements.detailsPage, "school-name", "urn");
        return this;
    }

    public checkSchoolNamesAreCorrectLinksOnPupilNumbersPage(): this {
        TableUtility.checkSchoolNamesAreCorrectLinksOnPage(this.elements.pupilNumbersPage, "school-name", "urn");
        return this;
    }

    public checkSchoolNamesAreCorrectLinksOnFreeSchoolMealsPage(): this {
        TableUtility.checkSchoolNamesAreCorrectLinksOnPage(this.elements.freeSchoolMeals, "school-name", "urn");
        return this;
    }
    
    public checkGiasHeaderNotPresent(): this {
        const { detailsPage } = this.elements;
        detailsPage.table().should('not.contain', 'Get information about schools');
    }

    public checkPupilNumbersHeadersPresent(): this {
        const { pupilNumbersPage } = this.elements;
        pupilNumbersPage.table().should('contain', 'School name')
            .and('contain', 'URN')
            .and('contain', 'Phase and age range')
            .and('contain', 'Pupil numbers')
            .and('contain', 'Pupil capacity')
            .and('contain', '% full');
        return this;
    }

    public checkFreeSchoolMealsHeadersPresent(): this {
        const { freeSchoolMeals } = this.elements;
        freeSchoolMeals.table().should('contain', 'School name')
            .and('contain', 'URN')
            .and('contain', 'Pupils eligible for free school meals')
            .and('contain', 'Local authority average')
            .and('contain', 'National average');
        return this;
    }

    public checkSchoolTypesOnDetailsTable() {
        this.elements.detailsPage.schoolType().each(element => {
            expect(element.text().trim()).to.be.oneOf(["Academy sponsor led", "Academy converter", "University technical college", "Free schools"]);
        });
    }

    public checkTrustDetailsSorting() {
        const { detailsPage } = this.elements;
        TableUtility.checkStringSorting(detailsPage.schoolName, detailsPage.schoolNameHeader);
        TableUtility.checkStringSorting(detailsPage.urn, detailsPage.urnHeader);
        TableUtility.checkStringSorting(detailsPage.dateJoinedTrust, detailsPage.dateJoinedTrustHeader);
        TableUtility.checkStringSorting(detailsPage.localAuthority, detailsPage.localAuthorityHeader);
        TableUtility.checkStringSorting(detailsPage.schoolType, detailsPage.schoolTypeHeader);
        TableUtility.checkStringSorting(detailsPage.ruralOrUrban, detailsPage.ruralOrUrbanHeader);
    }

    public checkPupilNumbersSorting() {
        const { pupilNumbersPage } = this.elements;
        TableUtility.checkStringSorting(pupilNumbersPage.schoolName, pupilNumbersPage.schoolNameHeader);
        TableUtility.checkStringSorting(pupilNumbersPage.urn, pupilNumbersPage.urnHeader);
        TableUtility.checkStringSorting(pupilNumbersPage.phaseAndAge, pupilNumbersPage.phaseAndAgeHeader);
        TableUtility.checkNumericSorting(pupilNumbersPage.pupilNumbers, pupilNumbersPage.pupilNumbersHeader);
        TableUtility.checkNumericSorting(pupilNumbersPage.pupilCapacity, pupilNumbersPage.pupilCapacityHeader);
    }

    public checkFreeSchoolMealsSorting() {
        const { freeSchoolMeals } = this.elements;
        TableUtility.checkStringSorting(freeSchoolMeals.schoolName, freeSchoolMeals.schoolNameHeader);
        TableUtility.checkStringSorting(freeSchoolMeals.urn, freeSchoolMeals.urnHeader);
        TableUtility.checkNumericSorting(freeSchoolMeals.pupilsEligible, freeSchoolMeals.pupilsEligibleHeader);
        TableUtility.checkNumericSorting(freeSchoolMeals.localAuthorityAverage, freeSchoolMeals.localAuthorityAverageHeader);
        TableUtility.checkNumericSorting(freeSchoolMeals.nationalAverage, freeSchoolMeals.nationalAverageHeader);
    }

    public checkEnglandWalesIdentifierNotPresent() {
        this.elements.detailsPage.table().should('not.contain', 'England and Wales');
        return this;
    }

    public checkTrustJoinedDatePresentAndValid(): this {
        this.elements.detailsPage.dateJoinedTrust().each(commonPage.checkValueIsValidDate);
        return this;
    }

}

const academiesInTrustPage = new AcademiesInTrustPage();
export default academiesInTrustPage;
