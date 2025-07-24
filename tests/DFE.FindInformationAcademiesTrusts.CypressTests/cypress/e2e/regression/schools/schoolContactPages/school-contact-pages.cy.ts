import schoolsPage from '../../../../pages/schools/schoolsPage';
import commonPage from '../../../../pages/commonPage';
import { testSchoolData } from '../../../../support/test-data-store';

// Test data constants
const TEST_SCHOOL_URN = 123452; // The Meadows Primary School
const TEST_ACADEMY_URN = 137083; // Abbey Grange Church of England Academy

// Helper to build the URL for the school contacts page
const getInTheSchoolContactsUrl = (urn: number) => `/schools/contacts/in-the-school?urn=${urn}`;

// Helper to build the URL for the "in DfE" contacts page

const incorrectEmailFormatMessage = 'Enter a DfE email address in the correct format, e.g. joe.bloggs@education.gov.uk';
const getInDfeContactsUrl = (urn: number) => `/schools/contacts/in-dfe?urn=${urn}`;

describe('Testing the components of the School "in this school/academy" contacts page', () => {
    (testSchoolData as { schoolName: string; urn: number; schoolOrAcademy: string; }[]).forEach(({ schoolName, urn, schoolOrAcademy }) => {
        describe(`On the "in this ${schoolOrAcademy}" contacts page for ${schoolName}`, () => {
            beforeEach(() => {
                cy.visit(getInTheSchoolContactsUrl(urn));
            });

            it('Checks the subpage header is correct', () => {
                schoolsPage
                    .checkSubpageHeaderIsCorrect();
            });

            it('Checks the head teacher contact card is present', () => {
                schoolsPage
                    .checkHeadTeacherContactCardPresent();
            });

            it('Checks the head teacher name is displayed', () => {
                schoolsPage
                    .checkHeadTeacherContactNamePresent();
            });

            it('Checks the head teacher email is displayed', () => {
                schoolsPage
                    .checkHeadTeacherContactEmailPresent();
            });

            it('Checks the internal use warning is present', () => {
                schoolsPage
                    .checkInternalUseWarningPresent();
            });

        });
    });
});

describe('Testing the components of the School "in DfE" contacts page', () => {
    beforeEach(() => {
        cy.visit(getInDfeContactsUrl(TEST_SCHOOL_URN));
    });

    it('Checks the subpage header is correct', () => {
        schoolsPage
            .checkInDfeContactsSubpageHeaderIsCorrect();
    });

    it('Checks the regions group LA lead contact card is present', () => {
        schoolsPage
            .checkRegionsGroupLaLeadContactCardPresent();
    });

    it('Checks the regions group LA lead contact title is displayed', () => {
        schoolsPage
            .checkRegionsGroupLaLeadContactTitlePresent();
    });

    it('Checks the regions group LA lead contact name is displayed', () => {
        schoolsPage
            .checkRegionsGroupLaLeadContactNamePresent();
    });

    it('Checks the regions group LA lead contact email is displayed', () => {
        schoolsPage
            .checkRegionsGroupLaLeadContactEmailPresent();
    });

    it('Checks no internal use warning is present (DfE contacts are not internal use only)', () => {
        schoolsPage
            .checkNoInternalUseWarningPresent();
    });

    it('Checks that edit link is present for Regions group LA lead contact (LA maintained schools can edit their contacts)', () => {
        schoolsPage
            .checkRegionsGroupLaLeadEditLinkPresent();
    });
});

describe('Testing the components of the Academy "in DfE" contacts page', () => {
    beforeEach(() => {
        cy.visit(getInDfeContactsUrl(TEST_ACADEMY_URN));
    });

    it('Checks the subpage header is correct', () => {
        schoolsPage
            .checkInDfeContactsSubpageHeaderIsCorrect();
    });

    it('Checks the trust relationship manager contact card is present', () => {
        schoolsPage
            .checkTrustRelationshipManagerContactCardPresent();
    });

    it('Checks the trust relationship manager contact title is displayed', () => {
        schoolsPage
            .checkTrustRelationshipManagerContactTitlePresent();
    });

    it('Checks the trust relationship manager contact name is displayed', () => {
        schoolsPage
            .checkTrustRelationshipManagerContactNamePresent();
    });

    it('Checks the trust relationship manager contact email is displayed', () => {
        schoolsPage
            .checkTrustRelationshipManagerContactEmailPresent();
    });

    it('Checks the SFSO lead contact card is present', () => {
        schoolsPage
            .checkSfsoLeadContactCardPresent();
    });

    it('Checks the SFSO lead contact title is displayed', () => {
        schoolsPage
            .checkSfsoLeadContactTitlePresent();
    });

    it('Checks the SFSO lead contact name is displayed', () => {
        schoolsPage
            .checkSfsoLeadContactNamePresent();
    });

    it('Checks the SFSO lead contact email is displayed', () => {
        schoolsPage
            .checkSfsoLeadContactEmailPresent();
    });

    it('Checks no internal use warning is present', () => {
        schoolsPage
            .checkNoInternalUseWarningPresent();
    });

    it('Checks no Change/Edit links are present for academy contacts (editing should be done at trust level only)', () => {
        schoolsPage
            .checkNoChangeLinksPresent();
    });
});

// Helper to generate random name and email for testing
function generateNameAndEmail() {
    const randomNumber = Math.floor(Math.random() * 9999);
    return {
        name: `Name${randomNumber}`,
        email: `email${randomNumber}@education.gov.uk`
    };
}

describe('Testing the school contact edit functionality', () => {
    describe('On the LA maintained school "in DfE" contacts page with edit functionality', () => {
        beforeEach(() => {
            cy.visit(getInDfeContactsUrl(TEST_SCHOOL_URN));
        });

        it('Can edit Regions group LA lead contact details successfully', () => {
            const { name, email } = generateNameAndEmail();

            schoolsPage
                .editRegionsGroupLaLead(name, email)
                .checkRegionsGroupLaLeadIsSuccessfullyUpdated(name, email);

            commonPage
                .checkSuccessPopup('Changes made to the Regions group local authority lead name and email were updated')
                .checkErrorPopupNotPresent();
        });

        it('Checks that when cancelling the edit of a Regions group LA lead contact that I am taken back to the previous page and that entered data is not saved', () => {
            schoolsPage
                .editRegionsGroupLaLeadWithoutSaving("Should Notbe Seen", "exittest@education.gov.uk")
                .clickContactUpdateCancelButton()
                .checkRegionsGroupLaLeadIsNotUpdated("Should Notbe Seen", "exittest@education.gov.uk");
        });
    });

    describe('Testing validation error handling for school contact edits', () => {
        beforeEach(() => {
            cy.visit(getInDfeContactsUrl(TEST_SCHOOL_URN));
        });

        it('Checks that a full non DfE email entered returns the correct error message', () => {
            schoolsPage
                .editRegionsGroupLaLead("Name", "email@hotmail.co.uk");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it('Checks that an incorrect email entered returns the correct error message', () => {
            schoolsPage
                .editRegionsGroupLaLead("Name", "email");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it('Checks that illegal characters entered returns the correct error message', () => {
            schoolsPage
                .editRegionsGroupLaLead("Name", "@£$$^&");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it('Checks that whitespace entered returns the correct error message', () => {
            schoolsPage
                .editRegionsGroupLaLead("Name", "a     b");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        context('Checks that an email address with an incorrect prefix formats entered returns the correct error message', () => {
            const incorrectPrefixes = ['@education.gov.uk', 'joe..bloggs@education.gov.uk', '.@education.gov.uk', '""@education.gov.uk'];
            incorrectPrefixes.forEach(prefix => {
                it(`Checks that an email address with an incorrect prefix ${prefix} entered returns the correct error message`, () => {
                    schoolsPage
                        .editRegionsGroupLaLead("Name", prefix);
                    commonPage
                        .checkErrorPopup(incorrectEmailFormatMessage);
                });
            });
        });
    });

    describe('Testing that LA maintained schools show edit links but academies do not', () => {
        it('Verifies LA maintained school has edit link for Regions group LA lead', () => {
            cy.visit(getInDfeContactsUrl(TEST_SCHOOL_URN));
            schoolsPage
                .checkRegionsGroupLaLeadContactCardPresent()
                .checkRegionsGroupLaLeadEditLinkPresent();
        });

        it('Verifies academy has no edit links for any contacts', () => {
            cy.visit(getInDfeContactsUrl(TEST_ACADEMY_URN));
            schoolsPage
                .checkTrustRelationshipManagerContactCardPresent()
                .checkSfsoLeadContactCardPresent()
                .checkNoChangeLinksPresent();
        });

        it('Verifies that LA maintained school does not show trust-level contacts', () => {
            cy.visit(getInDfeContactsUrl(TEST_SCHOOL_URN));
            // Verify only regions group LA lead is shown, not trust contacts
            schoolsPage
                .checkRegionsGroupLaLeadContactCardPresent();

            // Verify trust contacts are not present
            cy.get('[data-testid="contact-card-trust-relationship-manager"]').should('not.exist');
            cy.get('[data-testid="contact-card-sfso-lead"]').should('not.exist');
        });

        it('Verifies that academy shows trust-level contacts without edit links', () => {
            cy.visit(getInDfeContactsUrl(TEST_ACADEMY_URN));
            // Verify trust contacts are shown but regions group LA lead is not
            schoolsPage
                .checkTrustRelationshipManagerContactCardPresent()
                .checkSfsoLeadContactCardPresent();

            // Verify regions group LA lead is not present
            cy.get('[data-testid="contact-card-regions-group-la-lead"]').should('not.exist');
        });
    });
});
