import 'wick-a11y';
import 'cypress-axe';
import { generateComprehensiveAccessibilityReport } from '../../support/accessibility-report-generator';
import { ComprehensiveAuditHelper } from '../../support/comprehensive-audit-helper';
import { AuditPageDefinitions } from '../../support/audit-page-definitions';

/**
 * Comprehensive Accessibility Audit Report Generator
 * 
 * Generates ONE comprehensive HTML audit report covering ALL application areas:
 * - Core pages (home, search, cookies, accessibility, privacy)
 * - Trust pages (all overview, contacts, governance, ofsted, academies, financial subpages)
 * - School pages (all overview, contacts, SEN, federation subpages)
 * 
 * This audit covers EVERY page tested in the UI regression tests to ensure
 * complete accessibility coverage across the entire application.
 * 
 * The report includes:
 * - Executive summary with overall statistics
 * - Impact level breakdown (critical, serious, moderate, minor)
 * - Detailed findings for each page area
 * - Comprehensive coverage of all accessibility rules
 */

describe('Comprehensive Application Accessibility Audit', () => {
    const auditHelper = new ComprehensiveAuditHelper();
    const pageDefinitions = new AuditPageDefinitions(auditHelper);

    beforeEach(() => {
        cy.injectAxe();
    });

    // Main audit test - covers ALL application areas in ONE comprehensive report
    it('should generate single comprehensive accessibility audit covering EVERY page in the application', () => {
        // Clear previous results
        auditHelper.clearResults();

        // ============================================================================
        // 1. CORE APPLICATION PAGES
        // ============================================================================
        pageDefinitions.auditCorePages();

        // ============================================================================
        // 2. TRUST PAGES - COMPLETE COVERAGE OF ALL SUBPAGES
        // ============================================================================
        pageDefinitions.auditTrustPages();

        // ============================================================================
        // 3. SCHOOL PAGES - COMPLETE COVERAGE OF ALL SUBPAGES
        // ============================================================================
        pageDefinitions.auditSchoolPages();

        // ============================================================================
        // 4. GENERATE COMPREHENSIVE REPORT AFTER ALL PAGES AUDITED
        // ============================================================================
        cy.then(() => {
            const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
            const auditResults = auditHelper.getResults();
            const comprehensiveReport = generateComprehensiveAccessibilityReport(auditResults, timestamp);

            cy.writeFile(`cypress/accessibility/audit-reports/COMPREHENSIVE-AUDIT-REPORT-${timestamp}.html`, comprehensiveReport);

            // Log final summary
            auditHelper.logFinalSummary();
        });
    });
}); 
