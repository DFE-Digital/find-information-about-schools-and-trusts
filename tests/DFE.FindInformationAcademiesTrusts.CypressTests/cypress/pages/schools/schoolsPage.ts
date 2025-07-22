class SchoolsPage {

    elements = {
        pageName: () => cy.get('[data-testid="page-name"]'),
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        schoolType: () => cy.get('[data-testid="school-type"]'),
        trustLink: () => cy.get('[data-testid="header-trust-link"]'),
        nav: {
        },
        overview: {
            detailsTabHeader: () => cy.get('[data-testid="overview-details-subnav"]'),
            detailsTab: {
                addressHeader: () => cy.get('[data-testid="details-address-header"]'),
                dateJoinedTrustHeader: () => cy.get('[data-testid="details-date-joined-trust-header"]'),
                academyTrustHeader: () => cy.get('[data-testid="details-trust-header"]'),
                regionAndTerritoryHeader: () => cy.get('[data-testid="details-region-and-territory-header"]'),
                localAuthorityHeader: () => cy.get('[data-testid="details-local-authority-header"]'),
                phaseAndAgeRangeHeader: () => cy.get('[data-testid="details-phase-and-age-range-header"]'),
                hasNurseryClassesHeader: () => cy.get('[data-testid="details-has-nursery-classes-header"]'),
                informationForOtherServicesHeader: () => cy.get('[data-testid="details-information-from-other-services-header"]'),
                giasLink: () => cy.get('[data-testid="details-gias-link"]'),
                financialBenchmarkingLink: () => cy.get('[data-testid="details-financial-benchmarking-link"]'),
                findSchoolPerformanceDataLink: () => cy.get('[data-testid="details-find-school-performance-link"]'),
            },
            senTab: {
                senTabName: () => cy.get('[data-testid="overview-sen-subnav"]'),
                resourcedProvisionOnRollKey: () => cy.get('[data-testid="resourced-provision-on-roll-key"]'),
                resourcedProvisionCapacityKey: () => cy.get('[data-testid="resourced-provision-capacity-key"]'),
                senOnRollKey: () => cy.get('[data-testid="sen-on-roll-key"]'),
                senCapacityKey: () => cy.get('[data-testid="sen-capacity-key"]'),
                resourcedProvisionTypeKey: () => cy.get('[data-testid="resourced-provision-type-key"]'),
                senProvisionTypeKey: () => cy.get('[data-testid="sen-provision-type-key"]'),
                senProvisionType: () => cy.get('[data-testid="sen-provision-type"]'),
            },
            referenceNumbersTab: {
                urnHeader: () => cy.get('[data-testid="reference-numbers-urn-header"]'),
                urnValue: () => cy.get('[data-testid="reference-numbers-urn-value"]'),
                laestabHeader: () => cy.get('[data-testid="reference-numbers-laestab-header"]'),
                laestabValue: () => cy.get('[data-testid="reference-numbers-laestab-value"]'),
                ukprnHeader: () => cy.get('[data-testid="reference-numbers-ukprn-header"]'),
                ukprnValue: () => cy.get('[data-testid="reference-numbers-ukprn-value"]')
            },
        },
        schoolContacts: {
            internalUseWarning: () => cy.get('[data-testid="internal-use-only-warning"]'),
            inThisSchool: {
                headTeacherCard: () => cy.get('[data-testid="contact-card-head-teacher"]'),
                headTeacherTitle: () => cy.get('[data-testid="contact-card-title-head-teacher"]'),
                headTeacherName: () => cy.get('[data-testid="contact-card-head-teacher"] [data-testid="contact-name"]'),
                headTeacherEmail: () => cy.get('[data-testid="contact-card-head-teacher"] [data-testid="contact-email"]'),
            },
            inDfEContacts: {
                regionsGroupLaLeadCard: () => cy.get('[data-testid="contact-card-regions-group-la-lead"]'),
                regionsGroupLaLeadTitle: () => cy.get('[data-testid="contact-card-title-regions-group-la-lead"]'),
                regionsGroupLaLeadName: () => cy.get('[data-testid="contact-card-regions-group-la-lead"] [data-testid="contact-name"]'),
                regionsGroupLaLeadEmail: () => cy.get('[data-testid="contact-card-regions-group-la-lead"] [data-testid="contact-email"]'),
                regionsGroupLaLeadEditLink: () => cy.get('[data-testid="contact-card-regions-group-la-lead"] [class="govuk-summary-card__actions"] > a'),
                trustRelationshipManagerCard: () => cy.get('[data-testid="contact-card-trust-relationship-manager"]'),
                trustRelationshipManagerTitle: () => cy.get('[data-testid="contact-card-title-trust-relationship-manager"]'),
                trustRelationshipManagerName: () => cy.get('[data-testid="contact-card-trust-relationship-manager"] [data-testid="contact-name"]'),
                trustRelationshipManagerEmail: () => cy.get('[data-testid="contact-card-trust-relationship-manager"] [data-testid="contact-email"]'),
                sfsoLeadCard: () => cy.get('[data-testid="contact-card-sfso-lead"]'),
                sfsoLeadTitle: () => cy.get('[data-testid="contact-card-title-sfso-lead"]'),
                sfsoLeadName: () => cy.get('[data-testid="contact-card-sfso-lead"] [data-testid="contact-name"]'),
                sfsoLeadEmail: () => cy.get('[data-testid="contact-card-sfso-lead"] [data-testid="contact-email"]'),
            },
            editContacts: {
                nameInput: () => cy.get('[name="Name"]'),
                emailInput: () => cy.get('[name="Email"]'),
                saveButton: () => cy.contains('Save and continue'),
                cancelButton: () => cy.contains('Cancel')
            },
        },
        governance: {
            currentGovernors: {
                currentGovernorsSection: () => cy.get('[data-testid="current-governors"]'),
                currentGovernorsTable: () => cy.get('[data-testid="current-governors"] table'),
                currentGovernorsHeader: () => cy.get('[data-testid="current-governors"] [data-testid="subpage-header"]'),
                currentGovernorsRows: () => cy.get('[data-testid="current-members-row"]'),
                currentGovernorsNames: () => cy.get('[data-testid="current-governors-name"]'),
                currentGovernorsRoles: () => cy.get('[data-testid="current-governors-role"]'),
                currentGovernorsAppointedBy: () => cy.get('[data-testid="current-governors-appointed"]'),
                currentGovernorsFromDates: () => cy.get('[data-testid="current-governors-from"]'),
                currentGovernorsToDates: () => cy.get('[data-testid="current-governors-to"]'),
                currentGovernorsNavTab: () => cy.get('[data-testid="current-governors-subnav"]'),
                noCurrentGovernorsMessage: () => cy.get('[data-testid="current-governors"]').contains('No Current governors'),
            },
            historicGovernors: {
                historicGovernorsSection: () => cy.get('[data-testid="historic-governors"]'),
                historicGovernorsTable: () => cy.get('[data-testid="historic-governors"] table'),
                historicGovernorsHeader: () => cy.get('[data-testid="historic-governors"] [data-testid="subpage-header"]'),
                historicGovernorsRows: () => cy.get('[data-testid="historic-members-row"]'),
                historicGovernorsNames: () => cy.get('[data-testid="historic-governors-name"]'),
                historicGovernorsRoles: () => cy.get('[data-testid="historic-governors-role"]'),
                historicGovernorsAppointedBy: () => cy.get('[data-testid="historic-governors-appointed"]'),
                historicGovernorsFromDates: () => cy.get('[data-testid="historic-governors-from"]'),
                historicGovernorsToDates: () => cy.get('[data-testid="historic-governors-to"]'),
                historicGovernorsNavTab: () => cy.get('[data-testid="historic-governors-subnav"]'),
                noHistoricGovernorsMessage: () => cy.get('[data-testid="historic-governors"]').contains('No Historic governors'),
            }
        },
        federation: {
            federationName: () => cy.get('[data-testid="federation-details-name"]'),
            federationUid: () => cy.get('[data-testid="federation-details-uid"]'),
            federationOpenedOn: () => cy.get('[data-testid="federation-details-opened-on"]'),
            federationSchoolsHeader: () => cy.get('[data-testid="federation-schools-header"]'),
            federationSchoolLinks: () => cy.get('[data-testid="federation-school-link"]'),
            federationTab: () => cy.get('[data-testid="overview-federation-subnav"]')
        },
    };

    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().trim();
        expect(text).to.match(expected);
    };

    public checkCorrectSENTypePresent(): this {
        // Valid SEN (Special Educational Needs) types:
        // - Not Applicable
        // - SpLD - Specific Learning Difficulty
        // - VI - Visual Impairment
        // - OTH - Other Difficulty/Disability
        // - HI - Hearing Impairment
        // - SLCN - Speech, language and Communication
        // - ASD - Autistic Spectrum Disorder
        // - SEMH - Social, Emotional and Mental Health
        // - MSI - Multi-Sensory Impairment
        // - PD - Physical Disability
        // - MLD - Moderate Learning Difficulty
        // - SLD - Severe Learning Difficulty
        // - PMLD - Profound and Multiple Learning Difficulty

        const validSenRegex = /^(Not Applicable|SpLD - Specific Learning Difficulty|VI|- Visual Impairment|OTH - Other Difficulty\/Disability|HI - Hearing Impairment|SLCN - Speech, language and Communication|ASD - Autistic Spectrum Disorder|SEMH - Social, Emotional and Mental Health|MSI - Multi-Sensory Impairment|PD - Physical Disability|MLD - Moderate Learning Difficulty|SLD - Severe Learning Difficulty|PMLD - Profound and Multiple Learning Difficulty)$/;

        // Find all SEN type elements on the page
        this.elements.overview.senTab.senProvisionType().each((element) => {
            // Get the raw text content
            const rawTextContent = element.text();

            // Process the text content:
            // 1. Split into separate lines
            const textLines = rawTextContent.split('\n');

            // 2. Clean up each line by removing extra spaces
            const cleanedLines = textLines.map(line => line.trim());

            // 3. Remove any empty lines and get SEN types
            const foundSenTypes = cleanedLines.filter(line => line.length > 0);

            // Check each SEN type against our valid list
            foundSenTypes.forEach(senType => {
                expect(senType).to.match(validSenRegex,
                    `Expected "${senType}" to be a valid SEN type`);
            });
        });

        return this;
    }

    public checkValueIsValidSchoolType = (element: JQuery<HTMLElement>) =>
        this.checkElementMatches(element, /^(Community school|Academy converter|Local authority nursery school)$/);


    public checkCorrectSchoolTypePresent(): this {
        this.elements.schoolType().each(this.checkValueIsValidSchoolType);
        return this;
    }

    public checkOverviewPageNamePresent(): this {
        this.elements.pageName().should('contain', 'Overview');
        return this;
    }

    public checkAcademyLinkPresentAndCorrect(trustAcademyName: string): this {
        this.elements.trustLink().should('be.visible');
        this.elements.trustLink().should('contain.text', trustAcademyName);
        return this;
    }

    public checkAcademyLinkNotPresentForSchool(): this {
        this.elements.trustLink().should('not.exist');
        return this;
    }

    public clickAcademyTrustLink(): this {
        this.elements.trustLink().click();
        return this;
    }

    // #region Details Tab

    public checkSchoolDetailsHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'School details');
        return this;
    }

    public checkAcademyDetailsHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Academy details');
        return this;
    }

    public checkSchoolDetailsTabCorrect(): this {
        this.elements.overview.detailsTabHeader().should('contain', 'School details');
        return this;
    }

    public checkAcademyDetailsTabCorrect(): this {
        this.elements.overview.detailsTabHeader().should('contain', 'Academy details');
        return this;
    }

    public checkDetailsSchoolDataItemsPresent(): this {
        this.elements.overview.detailsTab.addressHeader().should('be.visible').and('contain.text', 'Address');
        this.elements.overview.detailsTab.regionAndTerritoryHeader().should('be.visible').and('contain.text', 'Region and territory');
        this.elements.overview.detailsTab.localAuthorityHeader().should('be.visible').and('contain.text', 'Local authority');
        this.elements.overview.detailsTab.phaseAndAgeRangeHeader().should('be.visible').and('contain.text', 'Phase and age range');
        this.elements.overview.detailsTab.hasNurseryClassesHeader().should('be.visible').and('contain.text', 'Has nursery classes');
        return this;
    }

    public checkDetailsAcademyDataItemsPresent(): this {
        this.elements.overview.detailsTab.addressHeader().should('be.visible').and('contain.text', 'Address');
        this.elements.overview.detailsTab.dateJoinedTrustHeader().should('be.visible').and('contain.text', 'Date joined the trust');
        this.elements.overview.detailsTab.academyTrustHeader().should('be.visible').and('contain.text', 'Trust');
        this.elements.overview.detailsTab.regionAndTerritoryHeader().should('be.visible').and('contain.text', 'Region and territory');
        this.elements.overview.detailsTab.localAuthorityHeader().should('be.visible').and('contain.text', 'Local authority');
        this.elements.overview.detailsTab.phaseAndAgeRangeHeader().should('be.visible').and('contain.text', 'Phase and age range');
        this.elements.overview.detailsTab.hasNurseryClassesHeader().should('be.visible').and('contain.text', 'Has nursery classes');
        return this;
    }

    public checkDetailsAcademyDataItemsNotPresent(): this {
        this.elements.overview.detailsTab.dateJoinedTrustHeader().should('not.exist');
        this.elements.overview.detailsTab.academyTrustHeader().should('not.exist');
        return this;
    }

    public checkDetailsOtherServicesItemsPresent(): this {
        this.elements.overview.detailsTab.giasLink().should('be.visible').and('contain.text', 'Get information about schools');
        this.elements.overview.detailsTab.financialBenchmarkingLink().should('be.visible').and('contain.text', 'Financial benchmarking');
        this.elements.overview.detailsTab.findSchoolPerformanceDataLink().should('be.visible').and('contain.text', 'Find school college and performance data');
        return this;
    }

    // #endregion

    // #region SEN Tab

    public checkSENTabNameCorrect(): this {
        this.elements.overview.senTab.senTabName().should('be.visible').and('contain.text', 'SEN');
        return this;
    }

    public checkSENSubpageHeaderCorrect(): this {
        this.elements.subpageHeader().should('contain', 'SEN (special educational needs)');
        return this;
    }

    public checkSENDataItemsPresent(): this {
        this.elements.overview.senTab.resourcedProvisionOnRollKey().should('be.visible').and('contain.text', 'Resourced provision number on roll');
        this.elements.overview.senTab.resourcedProvisionCapacityKey().should('be.visible').and('contain.text', 'Resourced provision capacity');
        this.elements.overview.senTab.senOnRollKey().should('be.visible').and('contain.text', 'Special Educational Needs (SEN) unit number on roll');
        this.elements.overview.senTab.senCapacityKey().should('be.visible').and('contain.text', 'Special Educational Needs (SEN) unit number capacity');
        this.elements.overview.senTab.resourcedProvisionTypeKey().should('be.visible').and('contain.text', 'Type of resourced provision');
        this.elements.overview.senTab.senProvisionTypeKey().should('be.visible').and('contain.text', 'Type of SEN provision');
        return this;
    }
    // #endregion

    // #region School contacts pages
    public checkHeadTeacherContactCardPresent(): this {
        this.elements.schoolContacts.inThisSchool.headTeacherCard().should('be.visible');
        this.elements.schoolContacts.inThisSchool.headTeacherTitle().should('contain', 'Head teacher');
        this.elements.schoolContacts.inThisSchool.headTeacherName().should('be.visible');
        this.elements.schoolContacts.inThisSchool.headTeacherEmail().should('be.visible');
        return this;
    }

    public checkHeadTeacherContactNamePresent(): this {
        this.elements.schoolContacts.inThisSchool.headTeacherName().should('not.contain.text', 'No contact name available');
        this.elements.schoolContacts.inThisSchool.headTeacherName().should('not.be.empty');
        return this;
    }

    public checkHeadTeacherContactEmailPresent(): this {
        this.elements.schoolContacts.inThisSchool.headTeacherEmail().should('not.contain.text', 'No contact email available');
        this.elements.schoolContacts.inThisSchool.headTeacherEmail().should('not.be.empty');
        this.elements.schoolContacts.inThisSchool.headTeacherEmail().should('have.attr', 'href').and('match', /^mailto:/);
        return this;
    }

    public checkInternalUseWarningPresent(): this {
        this.elements.schoolContacts.internalUseWarning().should('be.visible').and('contain.text', 'Head teacher email addresses are for internal use only.');
        return this;
    }

    public checkSubpageHeaderIsCorrect(): this {
        this.elements.subpageHeader().should('be.visible');
        this.elements.subpageHeader().should(($el: JQuery<HTMLElement>) => {
            // Get the text, trim whitespace, and replace non-breaking spaces
            let text = $el.text().replace(/\u00a0/g, ' ').replace(/&nbsp;/g, ' ').trim();
            // Collapse multiple spaces
            text = text.replace(/\s+/g, ' ');
            expect(text).to.match(/Contacts in this (school|academy)/);
        });
        return this;
    }

    public checkInDfeContactsSubpageHeaderIsCorrect(): this {
        this.elements.subpageHeader().should('contain', 'In DfE');
        return this;
    }

    public checkRegionsGroupLaLeadContactCardPresent(): this {
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadCard().should('be.visible');
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadName().should('be.visible');
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadEmail().should('be.visible');
        return this;
    }

    public checkRegionsGroupLaLeadContactTitlePresent(): this {
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadTitle().should('be.visible').and('contain', 'Regions group local authority lead');
        return this;
    }

    public checkRegionsGroupLaLeadContactNamePresent(): this {
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadName().should('not.contain.text', 'No contact name available');
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadName().should('not.be.empty');
        return this;
    }

    public checkRegionsGroupLaLeadContactEmailPresent(): this {
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadEmail().should('not.contain.text', 'No contact email available');
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadEmail().should('not.be.empty');
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadEmail().should('have.attr', 'href').and('match', /^mailto:/);
        return this;
    }

    public checkNoInternalUseWarningPresent(): this {
        this.elements.schoolContacts.internalUseWarning().should('not.exist');
        return this;
    }

    public checkSfsoLeadContactCardPresent(): this {
        this.elements.schoolContacts.inDfEContacts.sfsoLeadCard().should('be.visible');
        this.elements.schoolContacts.inDfEContacts.sfsoLeadName().should('be.visible');
        this.elements.schoolContacts.inDfEContacts.sfsoLeadEmail().should('be.visible');
        return this;
    }

    public checkSfsoLeadContactTitlePresent(): this {
        this.elements.schoolContacts.inDfEContacts.sfsoLeadTitle().should('be.visible').and('contain', 'SFSO (Schools Financial Support and Oversight) lead');
        return this;
    }

    public checkSfsoLeadContactNamePresent(): this {
        this.elements.schoolContacts.inDfEContacts.sfsoLeadName().should('not.contain.text', 'No contact name available');
        this.elements.schoolContacts.inDfEContacts.sfsoLeadName().should('not.be.empty');
        return this;
    }

    public checkSfsoLeadContactEmailPresent(): this {
        this.elements.schoolContacts.inDfEContacts.sfsoLeadEmail().should('not.contain.text', 'No contact email available');
        this.elements.schoolContacts.inDfEContacts.sfsoLeadEmail().should('not.be.empty');
        this.elements.schoolContacts.inDfEContacts.sfsoLeadEmail().should('have.attr', 'href').and('match', /^mailto:/);
        return this;
    }

    public checkTrustRelationshipManagerContactCardPresent(): this {
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerCard().should('be.visible');
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerName().should('be.visible');
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerEmail().should('be.visible');
        return this;
    }

    public checkTrustRelationshipManagerContactTitlePresent(): this {
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerTitle().should('be.visible').and('contain', 'Trust relationship manager');
        return this;
    }

    public checkTrustRelationshipManagerContactNamePresent(): this {
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerName().should('not.contain.text', 'No contact name available');
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerName().should('not.be.empty');
        return this;
    }

    public checkTrustRelationshipManagerContactEmailPresent(): this {
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerEmail().should('not.contain.text', 'No contact email available');
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerEmail().should('not.be.empty');
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerEmail().should('have.attr', 'href').and('match', /^mailto:/);
        return this;
    }

    // #endregion

    // #region school federation details
    public checkFederationDetailsHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Federation details');
        return this;
    }

    public checkFederationDetailsPresent(): this {
        this.elements.federation.federationName().should('be.visible');
        this.elements.federation.federationUid().should('be.visible');
        this.elements.federation.federationOpenedOn().should('be.visible');
        return this;
    }

    public checkFederationSchoolsListPresent(): this {
        this.elements.federation.federationSchoolsHeader().should('be.visible');
        this.elements.federation.federationSchoolLinks().should('have.length.at.least', 1);
        this.elements.federation.federationSchoolLinks().each(($link) => {
            expect($link).to.have.attr('href').and.match(/\/schools\/overview\/federation\?urn=\d+/);
        });
        return this;
    }

    public checkFederationDetailsNotAvailable(): this {
        this.elements.federation.federationName().should('contain', 'Not available');
        this.elements.federation.federationUid().should('contain', 'Not available');
        this.elements.federation.federationOpenedOn().should('contain', 'Not available');
        return this;
    }

    public checkFederationTabNotPresent(): this {
        this.elements.federation.federationTab().should('not.exist');
        return this;
    }
    // #endregion

    // #region School contact edit functionality methods
    public checkRegionsGroupLaLeadEditLinkPresent(): this {
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadEditLink().should('be.visible').and('contain', 'Change');
        return this;
    }

    public checkRegionsGroupLaLeadEditLinkNotPresent(): this {
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadCard().should('not.contain', 'Change');
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadCard().should('not.contain', 'Edit');
        return this;
    }

    public editRegionsGroupLaLead(name: string, email: string): this {
        const { inDfEContacts, editContacts } = this.elements.schoolContacts;
        inDfEContacts.regionsGroupLaLeadEditLink().click();
        editContacts.nameInput().clear();
        if (name) {
            editContacts.nameInput().type(name);
        }
        editContacts.emailInput().clear();
        if (email) {
            editContacts.emailInput().type(email);
        }
        editContacts.saveButton().click();
        return this;
    }

    public editRegionsGroupLaLeadWithoutSaving(name: string, email: string): this {
        const { inDfEContacts, editContacts } = this.elements.schoolContacts;
        inDfEContacts.regionsGroupLaLeadEditLink().click();
        editContacts.nameInput().clear();
        if (name) {
            editContacts.nameInput().type(name);
        }
        editContacts.emailInput().clear();
        if (email) {
            editContacts.emailInput().type(email);
        }
        return this;
    }

    public clickContactUpdateCancelButton(): this {
        this.elements.schoolContacts.editContacts.cancelButton().click();
        return this;
    }

    public checkRegionsGroupLaLeadIsSuccessfullyUpdated(name: string, email: string): this {
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadName().should('contain.text', name);
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadEmail().should('contain.text', email);
        return this;
    }

    public checkRegionsGroupLaLeadIsNotUpdated(dontDisplayName: string, dontDisplayEmail: string): this {
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadName().should('not.contain.text', dontDisplayName);
        this.elements.schoolContacts.inDfEContacts.regionsGroupLaLeadEmail().should('not.contain.text', dontDisplayEmail);
        return this;
    }
    // #endregion

    public checkNoChangeLinksPresent(): this {
        // Verify no "Change" or "Edit" links are present in the academy contacts section
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerCard().should('not.contain', 'Change');
        this.elements.schoolContacts.inDfEContacts.trustRelationshipManagerCard().should('not.contain', 'Edit');
        this.elements.schoolContacts.inDfEContacts.sfsoLeadCard().should('not.contain', 'Change');
        this.elements.schoolContacts.inDfEContacts.sfsoLeadCard().should('not.contain', 'Edit');
        return this;
    }

    // #region school reference numbers

    public checkSchoolReferenceNumbersHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Reference numbers');
        return this;
    }

    public checkReferenceNumbersCardItemsPresent(): this {
        this.elements.overview.referenceNumbersTab.urnHeader().should('be.visible').and('contain.text', 'URN (Unique Reference Number)');
        this.elements.overview.referenceNumbersTab.laestabHeader().should('be.visible').and('contain.text', 'LAESTAB (local authority establishment number)');
        this.elements.overview.referenceNumbersTab.ukprnHeader().should('be.visible').and('contain.text', 'UKPRN (UK provider reference number)');
        return this;
    }

    public checkUrnHeaderPresent(): this {
        this.elements.overview.referenceNumbersTab.urnHeader().should('be.visible').and('contain.text', 'URN (Unique Reference Number)');
        return this;
    }

    public checkLaestabHeaderPresent(): this {
        this.elements.overview.referenceNumbersTab.laestabHeader().should('be.visible').and('contain.text', 'LAESTAB (local authority establishment number)');
        return this;
    }

    public checkUkprnHeaderPresent(): this {
        this.elements.overview.referenceNumbersTab.ukprnHeader().should('be.visible').and('contain.text', 'UKPRN (UK provider reference number)');
        return this;
    }

    public checkUrnValuePresent(): this {
        this.elements.overview.referenceNumbersTab.urnValue().should('be.visible');
        this.elements.overview.referenceNumbersTab.urnValue().should('not.contain.text', 'Not available');
        this.elements.overview.referenceNumbersTab.urnValue().should('not.be.empty');
        return this;
    }

    public checkLaestabValuePresent(): this {
        this.elements.overview.referenceNumbersTab.laestabValue().should('be.visible');
        this.elements.overview.referenceNumbersTab.laestabValue().should('not.contain.text', 'Not available');
        this.elements.overview.referenceNumbersTab.laestabValue().should('not.be.empty');
        return this;
    }

    public checkUkprnValuePresent(): this {
        this.elements.overview.referenceNumbersTab.ukprnValue().should('be.visible');
        this.elements.overview.referenceNumbersTab.ukprnValue().should('not.contain.text', 'Not available');
        this.elements.overview.referenceNumbersTab.ukprnValue().should('not.be.empty');
        return this;
    }

    public checkLaestabCorrectFormat(): this {
        // Business rule: LAESTAB should be displayed as XXX/XXXX format
        this.elements.overview.referenceNumbersTab.laestabValue().should(($el) => {
            const text = $el.text().trim();
            // Check if it matches XXX/XXXX format (3 digits slash 4 digits)
            expect(text).to.match(/^\d{3}\/\d{4}$/, 'LAESTAB should be in XXX/XXXX format');
        });
        return this;
    }

    public checkUrnIsNumeric(): this {
        // Business rule: URN should be a numeric value
        this.elements.overview.referenceNumbersTab.urnValue().should(($el) => {
            const text = $el.text().trim();
            expect(text).to.match(/^\d+$/, 'URN should be numeric');
        });
        return this;
    }

    public checkUkprnIsNumeric(): this {
        // Business rule: UKPRN should be a numeric value
        this.elements.overview.referenceNumbersTab.ukprnValue().should(($el) => {
            const text = $el.text().trim();
            expect(text).to.match(/^\d+$/, 'UKPRN should be numeric');
        });
        return this;
    }

    public checkAllReferenceNumbersDataPresent(): this {
        return this
            .checkUrnValuePresent()
            .checkLaestabValuePresent()
            .checkUkprnValuePresent()
            .checkLaestabCorrectFormat()
            .checkUrnIsNumeric()
            .checkUkprnIsNumeric();
    }

    public checkReferenceNumbersPageCompleteWithBusinessRules(): this {
        return this
            .checkSchoolReferenceNumbersHeaderPresent()
            .checkReferenceNumbersCardItemsPresent()
            .checkAllReferenceNumbersDataPresent();
    }
    // #endregion

    // #region School Governance pages

    // Current governors methods
    public checkCurrentGovernorsHeaderPresent(): this {
        this.elements.governance.currentGovernors.currentGovernorsHeader().should('be.visible');
        this.elements.governance.currentGovernors.currentGovernorsHeader().should('contain', 'Current governors');
        return this;
    }

    public checkCurrentGovernorsSectionPresent(): this {
        this.elements.governance.currentGovernors.currentGovernorsSection().should('be.visible');
        return this;
    }

    public checkCurrentGovernorsTablePresent(): this {
        this.elements.governance.currentGovernors.currentGovernorsTable().should('be.visible');
        return this;
    }

    public checkCurrentGovernorsDataPresent(): this {
        this.elements.governance.currentGovernors.currentGovernorsRows().should('have.length.at.least', 1);
        this.elements.governance.currentGovernors.currentGovernorsNames().should('be.visible');
        this.elements.governance.currentGovernors.currentGovernorsRoles().should('be.visible');
        this.elements.governance.currentGovernors.currentGovernorsAppointedBy().should('be.visible');
        this.elements.governance.currentGovernors.currentGovernorsFromDates().should('be.visible');
        this.elements.governance.currentGovernors.currentGovernorsToDates().should('be.visible');
        return this;
    }

    public checkCurrentGovernorsNavTabIsPresent(): this {
        this.elements.governance.currentGovernors.currentGovernorsNavTab().should('be.visible');
        return this;
    }

    public checkCurrentGovernorsTabHasCount(): this {
        this.elements.governance.currentGovernors.currentGovernorsNavTab().should('contain.text', '(');
        this.elements.governance.currentGovernors.currentGovernorsNavTab().should('contain.text', ')');
        return this;
    }

    public checkCurrentGovernorsTabCountMatches(): this {
        // Business rule: Tab should show the actual count of current governors
        this.elements.governance.currentGovernors.currentGovernorsRows().then(($rows) => {
            const actualCount = $rows.length;
            this.elements.governance.currentGovernors.currentGovernorsNavTab()
                .should('contain.text', `(${actualCount})`);
        });
        return this;
    }

    public checkNoCurrentGovernorsMessagePresent(): this {
        this.elements.governance.currentGovernors.noCurrentGovernorsMessage().should('be.visible');
        return this;
    }

    // Historic governors methods
    public checkHistoricGovernorsHeaderPresent(): this {
        this.elements.governance.historicGovernors.historicGovernorsHeader().should('be.visible');
        this.elements.governance.historicGovernors.historicGovernorsHeader().should('contain', 'Historic governors');
        return this;
    }

    public checkHistoricGovernorsSectionPresent(): this {
        this.elements.governance.historicGovernors.historicGovernorsSection().should('be.visible');
        return this;
    }

    public checkHistoricGovernorsTablePresent(): this {
        this.elements.governance.historicGovernors.historicGovernorsTable().should('be.visible');
        return this;
    }

    public checkHistoricGovernorsDataPresent(): this {
        this.elements.governance.historicGovernors.historicGovernorsRows().should('have.length.at.least', 1);
        this.elements.governance.historicGovernors.historicGovernorsNames().should('be.visible');
        this.elements.governance.historicGovernors.historicGovernorsRoles().should('be.visible');
        this.elements.governance.historicGovernors.historicGovernorsAppointedBy().should('be.visible');
        this.elements.governance.historicGovernors.historicGovernorsFromDates().should('be.visible');
        this.elements.governance.historicGovernors.historicGovernorsToDates().should('be.visible');
        return this;
    }

    public checkHistoricGovernorsNavTabIsPresent(): this {
        this.elements.governance.historicGovernors.historicGovernorsNavTab().should('be.visible');
        return this;
    }

    public checkHistoricGovernorsTabHasCount(): this {
        this.elements.governance.historicGovernors.historicGovernorsNavTab().should('contain.text', '(');
        this.elements.governance.historicGovernors.historicGovernorsNavTab().should('contain.text', ')');
        return this;
    }

    public checkHistoricGovernorsTabCountMatches(): this {
        // Business rule: Tab should show the actual count of historic governors
        this.elements.governance.historicGovernors.historicGovernorsRows().then(($rows) => {
            const actualCount = $rows.length;
            this.elements.governance.historicGovernors.historicGovernorsNavTab()
                .should('contain.text', `(${actualCount})`);
        });
        return this;
    }

    public checkNoHistoricGovernorsMessagePresent(): this {
        this.elements.governance.historicGovernors.noHistoricGovernorsMessage().should('be.visible');
        return this;
    }

    // Governance page general methods
    public checkGovernancePageNamePresent(): this {
        this.elements.pageName().should('contain', 'Governance');
        return this;
    }

    public checkAllGovernanceSubNavItemsPresent(): this {
        this.elements.governance.currentGovernors.currentGovernorsNavTab().should('be.visible');
        this.elements.governance.historicGovernors.historicGovernorsNavTab().should('be.visible');
        return this;
    }

    public checkGovernanceTabCountsDisplayed(): this {
        // Business rule: Both tabs should display counts in brackets
        return this
            .checkCurrentGovernorsTabHasCount()
            .checkHistoricGovernorsTabHasCount();
    }

    public checkGovernanceTabCountsAccurate(): this {
        // Business rule: Tab counts should match actual data
        return this
            .checkCurrentGovernorsTabCountMatches()
            .checkHistoricGovernorsTabCountMatches();
    }

    public verifyGovernorsTableStructure(tableType: 'current' | 'historic'): this {
        const tableSelector = tableType === 'current'
            ? this.elements.governance.currentGovernors.currentGovernorsTable
            : this.elements.governance.historicGovernors.historicGovernorsTable;

        // Check table headers are present and correctly labeled
        tableSelector().find('thead th').should('have.length', 5);
        tableSelector().find('thead th').eq(0).should('contain.text', 'Name');
        tableSelector().find('thead th').eq(1).should('contain.text', 'Role');
        tableSelector().find('thead th').eq(2).should('contain.text', 'Appointed by');
        tableSelector().find('thead th').eq(3).should('contain.text', 'From');
        tableSelector().find('thead th').eq(4).should('contain.text', 'To');

        return this;
    }

    public verifyGovernorsDataIntegrity(tableType: 'current' | 'historic'): this {
        const rowsSelector = tableType === 'current'
            ? this.elements.governance.currentGovernors.currentGovernorsRows
            : this.elements.governance.historicGovernors.historicGovernorsRows;

        // Check that each row has data in all required fields
        rowsSelector().each(($row) => {
            cy.wrap($row).find('[data-testid*="governors-name"]').should('not.be.empty');
            cy.wrap($row).find('[data-testid*="governors-role"]').should('not.be.empty');
            // Note: Appointed by, From, and To dates may be empty/N/A in some cases
        });

        return this;
    }

    // #endregion
}

const schoolsPage = new SchoolsPage();
export default schoolsPage;
