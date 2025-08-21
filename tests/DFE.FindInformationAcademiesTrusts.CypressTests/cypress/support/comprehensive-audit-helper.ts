/* eslint-disable @typescript-eslint/no-explicit-any, @typescript-eslint/no-unsafe-assignment, @typescript-eslint/no-unsafe-member-access */
import { type AuditPageResult } from './accessibility-report-generator';

export class ComprehensiveAuditHelper {
    private readonly auditResults: AuditPageResult[] = [];

    /**
     * Clear previous audit results to start fresh
     */
    clearResults(): void {
        this.auditResults.length = 0;
    }

    /**
     * Get all collected audit results
     */
    getResults(): AuditPageResult[] {
        return this.auditResults;
    }

    /**
     * Audit a page with full screenshot and reporting capability
     */
    auditPage(pageName: string, pageCategory: string, url: string): void {
        cy.visit(url);
        cy.injectAxe();
        this.runAxeAudit(pageName, pageCategory, url, false);
    }

    /**
     * Audit a page with full screenshot and reporting capability
     */
    auditPageWithRole(roleName: string, pageName: string, pageCategory: string, url: string): void {
        cy.login({role: roleName});
        cy.visit(url);
        cy.injectAxe();
        this.runAxeAudit(pageName, pageCategory, url, false);
    }


    /**
     * Audit a page without screenshots (for sensitive data like contacts/financial info)
     */
    auditPageWithoutScreenshots(pageName: string, pageCategory: string, url: string): void {
        cy.visit(url);
        cy.injectAxe();
        this.runAxeAudit(pageName, pageCategory, url, true);
    }

    /**
     * Run axe audit and process results
     */
    private runAxeAudit(pageName: string, pageCategory: string, url: string, isSensitiveData: boolean): void {
        cy.window().then((win) => {
            cy.wrap(null).then(() => {
                return win.axe.run(win.document, {
                    resultTypes: ['violations', 'passes']
                });
            }).then((results) => {
                const processedResults = this.processAxeResults(results, pageName, pageCategory, url);
                this.checkForFailingViolations(processedResults, pageName);
                this.logAuditResults(processedResults, pageCategory, isSensitiveData);
                this.auditResults.push(processedResults);
            });
        });
    }

    /**
     * Process raw axe results into our standardized format
     */
    private processAxeResults(results: any, pageName: string, pageCategory: string, url: string): AuditPageResult {
        const impactBreakdown: Record<string, number> = {};
        const impactViolations: Record<string, any[]> = {
            critical: [],
            serious: [],
            moderate: [],
            minor: [],
            unknown: []
        };

        // eslint-disable-next-line @typescript-eslint/no-unsafe-call
        results.violations?.forEach((violation: any) => {
            const impact = violation.impact ?? 'unknown';
            impactBreakdown[impact] = (impactBreakdown[impact] ?? 0) + 1;
            impactViolations[impact].push(violation);
        });

        return {
            pageName,
            pageCategory,
            url,
            summary: {
                violations: results.violations?.length ?? 0,
                passes: results.passes?.length ?? 0,
                totalRulesChecked: (results.violations?.length ?? 0) + (results.passes?.length ?? 0)
            },
            impactBreakdown,
            impactViolations,
            violations: results.violations ?? [],
            passes: results.passes ?? []
        };
    }

    /**
     * Check for critical or serious violations and fail the test if found
     */
    private checkForFailingViolations(results: AuditPageResult, pageName: string): void {
        const criticalViolations = results.impactViolations.critical || [];
        const seriousViolations = results.impactViolations.serious || [];
        const failingViolations = [...criticalViolations, ...seriousViolations];

        if (failingViolations.length > 0) {
            const violationList = failingViolations.map(v => `- ${v.id}: ${v.description}`).join('\n');
            const failureMessage = `❌ Accessibility audit failed for ${pageName}!\n` +
                `🚨 ${criticalViolations.length} critical violations\n` +
                `⚠️ ${seriousViolations.length} serious violations\n` +
                `Failing violations:\n${violationList}`;

            cy.log(failureMessage);
            throw new Error(failureMessage);
        }
    }

    /**
     * Log audit results to Cypress console
     */
    private logAuditResults(results: AuditPageResult, pageCategory: string, isSensitiveData: boolean): void {
        const sensitiveNote = isSensitiveData ? ' (no screenshots - sensitive data)' : '';

        cy.log(`🎯 Audited ${pageCategory}: ${results.pageName}${sensitiveNote}`);
        cy.log(`✅ ${results.summary.passes} passed | ❌ ${results.summary.violations} failed`);

        if (results.summary.violations > 0) {
            const impactSummary = Object.entries(results.impactBreakdown)
                .map(([impact, count]) => `${impact}:${count}`)
                .join(', ');
            cy.log(`📊 Impact: ${impactSummary}`);
        }
    }

    /**
     * Generate final summary statistics
     */
    getSummaryStats(): { totalPages: number; totalViolations: number; totalPasses: number; } {
        return {
            totalPages: this.auditResults.length,
            totalViolations: this.auditResults.reduce((sum, page) => sum + page.summary.violations, 0),
            totalPasses: this.auditResults.reduce((sum, page) => sum + page.summary.passes, 0)
        };
    }

    /**
     * Log final audit summary
     */
    logFinalSummary(): void {
        const stats = this.getSummaryStats();

        cy.log(`🎉 COMPREHENSIVE AUDIT COMPLETE`);
        cy.log(`📊 ${stats.totalPages} pages audited in ONE report`);
        cy.log(`✅ ${stats.totalPasses} rules passed across all pages`);
        cy.log(`❌ ${stats.totalViolations} violations found across all pages`);
    }
} 
