class HomePage {

    elements = {
        mainSearchBox: () => cy.get('#home-search'),
        mainSearchButton: () => cy.get('[data-testid="search"]'),

        whatYouCanFindList: {
            mainBox: () => cy.get('.govuk-details'),
            list: () => cy.get('.govuk-list'),
            button: () => cy.get('.govuk-details__summary'),
            academies: () => this.elements.whatYouCanFindList.list().contains('academies'),
            freeSchools: () => this.elements.whatYouCanFindList.list().contains('free schools'),
            laMaintainedSchools: () => this.elements.whatYouCanFindList.list().contains('local authority maintained schools'),
            specialSchools: () => this.elements.whatYouCanFindList.list().contains('special schools'),
            trusts: () => this.elements.whatYouCanFindList.list().contains('trusts'),
        },

        usefulToolsCards: {
            manageProjectsAndCases: () => cy.get('.dfe-card').contains('Manage projects and cases')
        }
    };

    public enterMainSearchText(searchText: string): this {
        this.elements.mainSearchBox().type(searchText);
        return this;
    }

    public clickMainSearchButton(): this {
        this.elements.mainSearchButton().click();
        return this;
    }

    public checkMainSearchButtonPresent(): this {
        this.elements.mainSearchButton().should('be.visible').should('be.enabled');
        return this;
    }

    public clickWhatYouCanFindList(): this {
        this.elements.whatYouCanFindList.button().click();
        return this;
    }

    public checkWhatYouCanFindPresent(): this {
        this.elements.whatYouCanFindList.mainBox().should('be.visible');
        return this;
    }

    public checkWhatYouCanFindListCollapsed(): this {
        this.elements.whatYouCanFindList.mainBox().should('not.have.attr', 'open');
        return this;
    }

    public checkWhatYouCanFindListOpen(): this {
        this.elements.whatYouCanFindList.mainBox().should('have.attr', 'open');
        return this;
    }

    public checkWhatYouCanFindListItemsPresent(): this {
        this.elements.whatYouCanFindList.academies().should('be.visible');
        this.elements.whatYouCanFindList.freeSchools().should('be.visible');
        this.elements.whatYouCanFindList.laMaintainedSchools().should('be.visible');
        this.elements.whatYouCanFindList.specialSchools().should('be.visible');
        this.elements.whatYouCanFindList.trusts().should('be.visible');
        return this;
    }

    public checkIfManageProjectsAndCasesPresent(): this {
        this.elements.usefulToolsCards.manageProjectsAndCases().should('be.visible');;
        return this;
    }

    public checkIfManageProjectsAndCasesNotPresent(): this {
        this.elements.usefulToolsCards.manageProjectsAndCases().should('not.exist');
        return this;
    }

    public clickManageProjectsAndCases(): this {
        this.elements.usefulToolsCards.manageProjectsAndCases().click();
        return this;
    }
}

const homePage = new HomePage();
export default homePage;
