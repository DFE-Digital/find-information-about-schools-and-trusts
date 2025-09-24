class SchoolPupilsPage {

    elements = {
        pageName: () => cy.get('[data-testid="page-name"]'),
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),

        // Navigation elements
        nav: {
            populationTab: () => cy.get('[data-testid="pupils-population-subnav"]'),
            attendanceTab: () => cy.get('[data-testid="pupils-attendance-subnav"]'),
        },

        // Population page elements
        population: {
            censusTable: () => cy.get('[data-testid="population-census-table"]'),
            tableHeaders: () => cy.get('[data-testid="population-census-table"] thead th'),
            yearHeaders: () => cy.get('[data-testid*="-year-header"]'),

            // Data rows
            numberOfPupilsOnRoleRow: () => cy.get('[data-testid="number-of-pupils-on-role-row"]'),
            eligiblePupilsWithEhcPlanRow: () => cy.get('[data-testid="eligible-pupils-with-ehc-plan-row"]'),
            eligiblePupilsWithSenSupportRow: () => cy.get('[data-testid="eligible-pupils-with-sen-support-row"]'),
            englishAsAdditionalLanguageRow: () => cy.get('[data-testid="english-as-an-additional-language-row"]'),
            eligibleForFreeSchoolMealsRow: () => cy.get('[data-testid="eligible-for-free-school-meals-row"]'),

            // Data cells for specific years and metrics
            getDataCell: (year: string, metric: string) => cy.get(`[data-testid="${year}-${metric}"]`),
            getAllDataCells: (metric: string) => cy.get(`[data-testid*="-${metric}"]`),
        },

        // Attendance page elements
        attendance: {
            censusTable: () => cy.get('[data-testid="attendance-census-table"]'),
            tableHeaders: () => cy.get('[data-testid="attendance-census-table"] thead th'),
            yearHeaders: () => cy.get('[data-testid*="-year-header"]'),

            // Data rows - note: the attendance page reuses some row data-testids from population
            numberOfPupilsOnRoleRow: () => cy.get('[data-testid="number-of-pupils-on-role-row"]'),
            eligiblePupilsWithEhcPlanRow: () => cy.get('[data-testid="eligible-pupils-with-ehc-plan-row"]'),

            // Data cells for specific years and metrics
            getDataCell: (year: string, metric: string) => cy.get(`[data-testid="${year}-${metric}"]`),
            getAllDataCells: (metric: string) => cy.get(`[data-testid*="-${metric}"]`),
        },

        // Census information elements
        censusInfo: {
            censusText: () => cy.get('p').contains('spring'),
            attendanceCensusText: () => cy.get('p').contains('autumn'),
        }
    };

    // Helper method for consistent element text matching
    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().trim();
        expect(text).to.match(expected);
    };

    // General page methods
    public checkPupilsPageNamePresent(): this {
        this.elements.pageName().should('contain', 'Pupils');
        return this;
    }

    public checkPupilsNavTabsPresent(): this {
        this.elements.nav.populationTab().should('be.visible').and('contain.text', 'Population');
        this.elements.nav.attendanceTab().should('be.visible').and('contain.text', 'Attendance');
        return this;
    }

    public clickPopulationTab(): this {
        this.elements.nav.populationTab().click();
        return this;
    }

    public clickAttendanceTab(): this {
        this.elements.nav.attendanceTab().click();
        return this;
    }

    // Population page methods
    public checkPopulationSubpageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Pupil population');
        return this;
    }

    public checkPopulationTablePresent(): this {
        this.elements.population.censusTable().should('be.visible');
        return this;
    }

    public checkPopulationTableStructure(): this {
        // Check that the table has the correct headers
        this.elements.population.tableHeaders().should('have.length.at.least', 2);

        // Check that year headers are present and visible
        this.elements.population.yearHeaders().should('have.length.at.least', 1);
        this.elements.population.yearHeaders().each(($header) => {
            cy.wrap($header).should('be.visible');
            // Year should be a 4-digit number (e.g., 2023, 2024)
            cy.wrap($header).should(($el) => {
                const text = $el.text().trim();
                expect(text).to.match(/^\d{4}$/, 'Year header should be a 4-digit year');
            });
        });

        return this;
    }

    public checkPopulationDataRowsPresent(): this {
        // Check that all expected data rows are present
        this.elements.population.numberOfPupilsOnRoleRow().should('be.visible');
        this.elements.population.eligiblePupilsWithEhcPlanRow().should('be.visible');
        this.elements.population.eligiblePupilsWithSenSupportRow().should('be.visible');
        this.elements.population.englishAsAdditionalLanguageRow().should('be.visible');
        this.elements.population.eligibleForFreeSchoolMealsRow().should('be.visible');
        return this;
    }

    public checkPopulationDataRowLabels(): this {
        // Verify the correct row labels are present
        this.elements.population.numberOfPupilsOnRoleRow().should('contain', 'Number of pupils on role (NOR)');
        this.elements.population.eligiblePupilsWithEhcPlanRow().should('contain', 'Eligible pupils with EHC plan');
        this.elements.population.eligiblePupilsWithSenSupportRow().should('contain', 'Eligible pupils with SEN support');
        this.elements.population.englishAsAdditionalLanguageRow().should('contain', 'English as an additional language');
        this.elements.population.eligibleForFreeSchoolMealsRow().should('contain', 'Eligible for free school meals');
        return this;
    }

    public checkPopulationDataCellsPresent(): this {
        // Check that data cells are present for each metric
        this.elements.population.getAllDataCells('number-of-pupils-on-role').should('have.length.at.least', 1);
        this.elements.population.getAllDataCells('eligible-pupils-with-ehc-plan').should('have.length.at.least', 1);
        this.elements.population.getAllDataCells('eligible-pupils-with-sen-support').should('have.length.at.least', 1);
        this.elements.population.getAllDataCells('english-as-an-additional-language').should('have.length.at.least', 1);
        this.elements.population.getAllDataCells('eligible-for-free-school-meals').should('have.length.at.least', 1);
        return this;
    }

    public verifyPopulationDataIntegrity(): this {
        // Simple check: data cells should not be empty and should be visible
        this.elements.population.getAllDataCells('number-of-pupils-on-role').each(($cell) => {
            cy.wrap($cell).should('be.visible').and('not.be.empty');
        });

        this.elements.population.getAllDataCells('eligible-pupils-with-ehc-plan').each(($cell) => {
            cy.wrap($cell).should('be.visible').and('not.be.empty');
        });

        return this;
    }

    // Attendance page methods
    public checkAttendanceSubpageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Pupil attendance');
        return this;
    }

    public checkAttendanceTablePresent(): this {
        this.elements.attendance.censusTable().should('be.visible');
        return this;
    }

    public checkAttendanceTableStructure(): this {
        // Check that the table has the correct headers
        this.elements.attendance.tableHeaders().should('have.length.at.least', 2);

        // Check that year headers are present and visible
        this.elements.attendance.yearHeaders().should('have.length.at.least', 1);
        this.elements.attendance.yearHeaders().each(($header) => {
            cy.wrap($header).should('be.visible');
            // Year should be a 4-digit number (e.g., 2023, 2024)
            cy.wrap($header).should(($el) => {
                const text = $el.text().trim();
                expect(text).to.match(/^\d{4}$/, 'Year header should be a 4-digit year');
            });
        });

        return this;
    }

    public checkAttendanceDataRowsPresent(): this {
        // Check that expected data rows are present
        this.elements.attendance.numberOfPupilsOnRoleRow().should('be.visible');
        this.elements.attendance.eligiblePupilsWithEhcPlanRow().should('be.visible');
        return this;
    }

    public checkAttendanceDataRowLabels(): this {
        // Verify the correct row labels are present for attendance
        this.elements.attendance.numberOfPupilsOnRoleRow().should('contain', 'Percentage of overall absence');
        this.elements.attendance.eligiblePupilsWithEhcPlanRow().should('contain', 'Percentage of enrolments who are persistent absentees');
        return this;
    }

    public checkAttendanceDataCellsPresent(): this {
        // Check that data cells are present for each metric
        this.elements.attendance.getAllDataCells('number-of-pupils-on-role').should('have.length.at.least', 1);
        this.elements.attendance.getAllDataCells('eligible-pupils-with-ehc-plan').should('have.length.at.least', 1);
        return this;
    }

    public verifyAttendanceDataIntegrity(): this {
        // Simple check: data cells should not be empty and should be visible
        this.elements.attendance.getAllDataCells('number-of-pupils-on-role').each(($cell) => {
            cy.wrap($cell).should('be.visible').and('not.be.empty');
        });

        this.elements.attendance.getAllDataCells('eligible-pupils-with-ehc-plan').each(($cell) => {
            cy.wrap($cell).should('be.visible').and('not.be.empty');
        });

        return this;
    }

    // Census information methods
    public checkPopulationCensusTextPresent(): this {
        this.elements.censusInfo.censusText().should('be.visible');
        this.elements.censusInfo.censusText().should('contain.text', 'spring');
        return this;
    }

    public checkAttendanceCensusTextPresent(): this {
        this.elements.censusInfo.attendanceCensusText().should('be.visible');
        this.elements.censusInfo.attendanceCensusText().should('contain.text', 'autumn');
        return this;
    }


    public checkYearHeadersPresent(): this {
        // Simple check: year headers should be visible and contain 4-digit years
        this.elements.population.yearHeaders().should('have.length.at.least', 1);
        this.elements.population.yearHeaders().each(($header) => {
            cy.wrap($header).should('be.visible');
        });
        return this;
    }

}

const schoolPupilsPage = new SchoolPupilsPage();
export default schoolPupilsPage;
