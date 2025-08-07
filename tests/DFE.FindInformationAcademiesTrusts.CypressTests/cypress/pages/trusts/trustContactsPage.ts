class ContactsPage {

    elements = {
        trustRelationshipManager: {
            section: () => cy.get('[data-testid="trust-relationship-manager"]'),
            title: () => this.elements.trustRelationshipManager.section().find('.govuk-summary-card__title'),
            name: () => this.elements.trustRelationshipManager.section().find('[data-testid="contact-name"]'),
            email: () => this.elements.trustRelationshipManager.section().find('[data-testid="contact-email"]'),
            editLink: () => this.elements.trustRelationshipManager.section().find('[class="govuk-summary-card__actions"] > a')
        },

        schoolsFinancialSupportOversight: {
            section: () => cy.get('[data-testid="sfso-lead"]'),
            title: () => this.elements.schoolsFinancialSupportOversight.section().find('.govuk-summary-card__title'),
            name: () => this.elements.schoolsFinancialSupportOversight.section().find('[data-testid="contact-name"]'),
            email: () => this.elements.schoolsFinancialSupportOversight.section().find('[data-testid="contact-email"]'),
            editLink: () => this.elements.schoolsFinancialSupportOversight.section().find('[class="govuk-summary-card__actions"] > a')
        },

        editContacts: {
            nameInput: () => cy.get('[name="Name"]'),
            emailInput: () => cy.get('[name="Email"]'),
            saveButton: () => cy.contains('Save and continue'),
            cancelButton: () => cy.contains('Cancel')
        },

        accountingOfficer: {
            section: () => cy.get('[data-testid="accounting-officer"]'),
            title: () => cy.get('[data-testid="accounting-officer"] > .govuk-summary-card__title-wrapper > .govuk-summary-card__title'),
            name: () => this.elements.accountingOfficer.section().find('[data-testid="contact-name"]'),
            email: () => this.elements.accountingOfficer.section().find('[data-testid="contact-email"]'),
        },

        chairOfTrustees: {
            section: () => cy.get('[data-testid="chair-of-trustees"]'),
            title: () => cy.get('[data-testid="chair-of-trustees"] > .govuk-summary-card__title-wrapper > .govuk-summary-card__title'),
            name: () => this.elements.chairOfTrustees.section().find('[data-testid="contact-name"]'),
            email: () => this.elements.chairOfTrustees.section().find('[data-testid="contact-email"]'),
        },

        chiefFinancialOfficer: {
            section: () => cy.get('[data-testid="chief-financial-officer"]'),
            title: () => cy.get('[data-testid="chief-financial-officer"] > .govuk-summary-card__title-wrapper > .govuk-summary-card__title'),
            name: () => this.elements.chiefFinancialOfficer.section().find('[data-testid="contact-name"]'),
            email: () => this.elements.chiefFinancialOfficer.section().find('[data-testid="contact-email"]'),
        },

        datasource: {
            section: () => cy.get('[data-testid="data-source-and-updates"]'),
            trmLatestUpdatedBy: () => cy.get('[data-testid="data-source-fiatdb-trust-relationship-manager"]'),
            sfsoLatestUpdatedBy: () => cy.get('[data-testid="data-source-fiatdb-sfso-lead"]')
        },

        subNav: {
            contactsInDfeSubnavButton: () => cy.get('[data-testid="contacts-in-dfe-subnav"]'),
            contactsInTheTrustSubnavButton: () => cy.get('[data-testid="contacts-in-this-trust-subnav"]'),
        },
        subHeaders: {
            subHeader: () => cy.get('[data-testid="subpage-header"]'),
        },
    };


    public clickContactUpdateCancelButton(): this {
        this.elements.editContacts.cancelButton().click();
        return this;
    }

    public editTrustRelationshipManager(name: string, email: string): this {
        const { trustRelationshipManager, editContacts } = this.elements;
        trustRelationshipManager.editLink().click();
        editContacts.nameInput().clear().type(name);
        editContacts.emailInput().clear().type(email);
        editContacts.saveButton().click();
        return this;
    }

    public editTrustRelationshipManagerWithoutSaving(name: string, email: string): this {
        const { trustRelationshipManager, editContacts } = this.elements;
        trustRelationshipManager.editLink().click();
        editContacts.nameInput().clear().type(name);
        editContacts.emailInput().clear().type(email);
        return this;
    }

    public checkTrustRelationshipManagerIsSuccessfullyUpdated(name: string, email: string): this {
        this.elements.trustRelationshipManager.name().should('contain.text', name);
        this.elements.trustRelationshipManager.email().should('contain.text', email);
        return this;
    }

    public checkTrustRelationshipManagerIsNotUpdated(dontDisplayName: string, dontDisplayEmail: string): this {
        this.elements.trustRelationshipManager.name().should('not.contain.text', dontDisplayName);
        this.elements.trustRelationshipManager.email().should('not.contain.text', dontDisplayEmail);
        return this;
    }

    public checkSfsoLeadIsNotUpdated(dontDisplayName: string, dontDisplayEmail: string): this {
        this.elements.schoolsFinancialSupportOversight.name().should('not.contain.text', dontDisplayName);
        this.elements.schoolsFinancialSupportOversight.email().should('not.contain.text', dontDisplayEmail);
        return this;
    }

    public editSfsoLead(name: string, email: string): this {
        const { schoolsFinancialSupportOversight, editContacts } = this.elements;
        schoolsFinancialSupportOversight.editLink().click();
        editContacts.nameInput().clear().type(name);
        editContacts.emailInput().clear().type(email);
        editContacts.saveButton().click();
        return this;
    }

    public editSfsoLeadWithoutSaving(name: string, email: string): this {
        const { schoolsFinancialSupportOversight, editContacts } = this.elements;
        schoolsFinancialSupportOversight.editLink().click();
        editContacts.nameInput().clear().type(name);
        editContacts.emailInput().clear().type(email);
        return this;
    }

    public checkSfsoLeadIsSuccessfullyUpdated(name: string, email: string): this {
        this.elements.schoolsFinancialSupportOversight.name().should('contain.text', name);
        this.elements.schoolsFinancialSupportOversight.email().should('contain.text', email);
        return this;
    }

    public checkTrustRelationshipManagerIsPresent(): this {
        this.elements.trustRelationshipManager.title().should('contain', 'Trust relationship manager');
        this.elements.trustRelationshipManager.name().should('be.visible');
        this.elements.trustRelationshipManager.email().should('be.visible');
        return this;
    }

    public checkSfsoLeadIsPresent(): this {
        this.elements.schoolsFinancialSupportOversight.title().should('contain', 'SFSO (Schools financial support and oversight) lead');
        this.elements.schoolsFinancialSupportOversight.name().should('be.visible');
        this.elements.schoolsFinancialSupportOversight.email().should('be.visible');
        return this;
    }

    public checkAccountingOfficerPresent(): this {
        this.elements.accountingOfficer.title().should('contain', 'Accounting officer');
        this.elements.accountingOfficer.name().should('be.visible');
        this.elements.accountingOfficer.email().should('be.visible');
        return this;
    }

    public checkChairOfTrusteesPresent(): this {
        this.elements.chairOfTrustees.title().should('contain', 'Chair of trustees');
        this.elements.chairOfTrustees.name().should('be.visible');
        this.elements.chairOfTrustees.email().should('be.visible');
        return this;
    }

    public checkChiefFinancialOfficerPresent(): this {
        this.elements.chiefFinancialOfficer.title().should('contain', 'Chief financial officer');
        this.elements.chiefFinancialOfficer.name().should('be.visible');
        this.elements.chiefFinancialOfficer.email().should('be.visible');
        return this;
    }

    public checkTrustRelationshipManagerDatasourceLastUpdatedByUser(expectedUser: string): this {
        const { datasource } = this.elements;
        datasource.section().expandDetailsElement();
        datasource.section().should('have.attr', 'open');
        datasource.trmLatestUpdatedBy().should('include.text', 'Trust relationship manager updated by ' + expectedUser + ' on ');
        return this;
    }

    public checkSfsoLeadDatasourceLastUpdatedByUser(expectedUser: string): this {
        const { datasource } = this.elements;
        datasource.section().expandDetailsElement();
        datasource.section().should('have.attr', 'open');
        datasource.sfsoLatestUpdatedBy().should('include.text', 'SFSO (Schools financial support and oversight) lead updated by ' + expectedUser + ' on ');
        return this;
    }

    public clickContactsInDfeSubnavButton(): this {
        this.elements.subNav.contactsInDfeSubnavButton().click();
        return this;
    }

    public clickContactsInTheTrustSubnavButton(): this {
        this.elements.subNav.contactsInTheTrustSubnavButton().click();
        return this;
    }

    public checkAllSubNavItemsPresent(): this {
        this.elements.subNav.contactsInDfeSubnavButton().should('be.visible');
        this.elements.subNav.contactsInTheTrustSubnavButton().should('be.visible');
        return this;
    }

    public checkSubNavNotPresent(): this {
        this.elements.subNav.contactsInDfeSubnavButton().should('not.exist');
        this.elements.subNav.contactsInTheTrustSubnavButton().should('not.exist');
        return this;
    }

    public checkContactsInDfeSubnavButtonIsHighlighted(): this {
        this.elements.subNav.contactsInDfeSubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    public checkContactsInTheTrustSubnavButtonIsHighlighted(): this {
        this.elements.subNav.contactsInTheTrustSubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    public checkContactsInDfeSubHeaderPresent(): this {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Contacts in DfE');
        return this;
    }

    public checkContactsInTheTrustSubHeaderPresent(): this {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Contacts in this trust');
        return this;
    }
}

const contactsPage = new ContactsPage();
export default contactsPage;
