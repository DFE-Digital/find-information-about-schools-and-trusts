import { testTrustData, testSchoolData, testFederationData } from './test-data-store';
import type { ComprehensiveAuditHelper } from './comprehensive-audit-helper';

export class AuditPageDefinitions {
    constructor(private readonly auditHelper: ComprehensiveAuditHelper) { }

    /**
     * Audit all core application pages
     */
    auditCorePages(): void {
        this.auditHelper.auditPage('Home Page', 'Core', '/');
        this.auditHelper.auditPage('Search Page', 'Core', '/search');
        this.auditHelper.auditPage('Search Results', 'Core', '/search?searchTerm=academy');
        this.auditHelper.auditPage('Cookies Policy', 'Core', '/cookies');
        this.auditHelper.auditPage('Accessibility Statement', 'Core', '/accessibility');
        this.auditHelper.auditPage('Privacy Policy', 'Core', '/privacy');
    }

    /**
     * Audit all trust-related pages (overview, contacts, governance, ofsted, academies, financial)
     */
    auditTrustPages(): void {
        const trustUid = testTrustData[1].uid;
        const contactsTrustUid = testTrustData[0].uid;

        // Trust Overview subpages
        this.auditHelper.auditPage('Trust Overview - Details', 'Trusts', `/trusts/overview/trust-details?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Overview - Summary', 'Trusts', `/trusts/overview/trust-summary?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Overview - Reference Numbers', 'Trusts', `/trusts/overview/reference-numbers?uid=${trustUid}`);

        // Trust Contacts (sensitive data - no screenshots)
        this.auditHelper.auditPageWithoutScreenshots('Trust Contacts - In DfE', 'Trusts', `/trusts/contacts/in-dfe?uid=${contactsTrustUid}`);
        this.auditHelper.auditPageWithoutScreenshots('Trust Contacts - In Trust', 'Trusts', `/trusts/contacts/in-the-trust?uid=${contactsTrustUid}`);
        this.auditHelper.auditPageWithoutScreenshots('Edit Trust Relationship Manager', 'Trusts', `/trusts/contacts/edittrustrelationshipmanager?uid=${contactsTrustUid}`);
        this.auditHelper.auditPageWithoutScreenshots('Edit SFSO Lead', 'Trusts', `/trusts/contacts/editsfsolead?uid=${contactsTrustUid}`);

        // Trust Governance subpages
        this.auditHelper.auditPage('Trust Governance - Leadership', 'Trusts', `/trusts/governance/trust-leadership?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Governance - Trustees', 'Trusts', `/trusts/governance/trustees?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Governance - Members', 'Trusts', `/trusts/governance/members?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Governance - Historic Members', 'Trusts', `/trusts/governance/historic-members?uid=${trustUid}`);

        // Trust Ofsted subpages
        this.auditHelper.auditPage('Trust Ofsted - Single Headline Grades', 'Trusts', `/trusts/ofsted/single-headline-grades?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Ofsted - Current Ratings', 'Trusts', `/trusts/ofsted/current-ratings?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Ofsted - Previous Ratings', 'Trusts', `/trusts/ofsted/previous-ratings?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Ofsted - Safeguarding and Concerns', 'Trusts', `/trusts/ofsted/safeguarding-and-concerns?uid=${trustUid}`);

        // Trust Academies In-Trust subpages
        this.auditHelper.auditPage('Trust Academies - In Trust Details', 'Trusts', `/trusts/academies/in-trust/details?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Academies - In Trust Pupil Numbers', 'Trusts', `/trusts/academies/in-trust/pupil-numbers?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Academies - In Trust Free School Meals', 'Trusts', `/trusts/academies/in-trust/free-school-meals?uid=${trustUid}`);

        // Trust Pipeline Academies subpages
        this.auditHelper.auditPage('Trust Pipeline Academies - Pre Advisory', 'Trusts', `/trusts/academies/pipeline/pre-advisory-board?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Pipeline Academies - Post Advisory', 'Trusts', `/trusts/academies/pipeline/post-advisory-board?uid=${trustUid}`);
        this.auditHelper.auditPage('Trust Pipeline Academies - Free Schools', 'Trusts', `/trusts/academies/pipeline/free-schools?uid=${trustUid}`);

        // Trust Financial Documents (sensitive data - no screenshots)
        this.auditHelper.auditPageWithoutScreenshots('Trust Financial Documents - Statements', 'Trusts', `/trusts/financial-documents/financial-statements?uid=${trustUid}`);
        this.auditHelper.auditPageWithoutScreenshots('Trust Financial Documents - Management Letters', 'Trusts', `/trusts/financial-documents/management-letters?uid=${trustUid}`);
        this.auditHelper.auditPageWithoutScreenshots('Trust Financial Documents - Internal Scrutiny Reports', 'Trusts', `/trusts/financial-documents/internal-scrutiny-reports?uid=${trustUid}`);
        this.auditHelper.auditPageWithoutScreenshots('Trust Financial Documents - Self Assessment Checklists', 'Trusts', `/trusts/financial-documents/self-assessment-checklists?uid=${trustUid}`);
    }

    /**
     * Audit all school-related pages
     */
    auditSchoolPages(): void {
        const schoolUrn = testSchoolData[1].urn; // Academy
        const laMaintainedSchoolUrn = testSchoolData[0].urn; // LA maintained school
        const senSchoolUrn = testSchoolData[0].urn;
        const federationSchoolUrn = testFederationData.schoolWithFederationDetails.urn;

        // School Overview subpages
        this.auditHelper.auditPage('School Overview - Details', 'Schools', `/schools/overview/details?urn=${schoolUrn}`);
        this.auditHelper.auditPage('School Overview - SEN Provision', 'Schools', `/schools/overview/sen?urn=${senSchoolUrn}`);
        this.auditHelper.auditPage('School Overview - Federation Details', 'Schools', `/schools/overview/federation?urn=${federationSchoolUrn}`);

        // School Contacts - In School/Academy (sensitive data - no screenshots)
        this.auditHelper.auditPageWithoutScreenshots('LA Maintained School Contacts - In School', 'Schools', `/schools/contacts/in-the-school?urn=${laMaintainedSchoolUrn}`);
        this.auditHelper.auditPageWithoutScreenshots('Academy Contacts - In Academy', 'Schools', `/schools/contacts/in-the-school?urn=${schoolUrn}`);

        // School Contacts - In DfE (sensitive data - no screenshots)
        this.auditHelper.auditPageWithoutScreenshots('LA Maintained School Contacts - In DfE', 'Schools', `/schools/contacts/in-dfe?urn=${laMaintainedSchoolUrn}`);
        this.auditHelper.auditPageWithoutScreenshots('Academy Contacts - In DfE', 'Schools', `/schools/contacts/in-dfe?urn=${schoolUrn}`);

        // School Contact Edit Pages (sensitive data - no screenshots, LA maintained schools only)
        this.auditHelper.auditPageWithoutScreenshots('Edit Regions Group LA Lead Contact', 'Schools', `/schools/contacts/editregionsgrouplocalauthoritylead?urn=${laMaintainedSchoolUrn}`);
    }
} 
