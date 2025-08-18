/* eslint-disable @typescript-eslint/no-explicit-any, @typescript-eslint/no-unsafe-member-access, @typescript-eslint/no-unsafe-call, @typescript-eslint/no-unsafe-assignment */

/**
 * Accessibility Report Generator
 * 
 * Generates comprehensive HTML accessibility audit reports with:
 * - Executive summary with overall statistics
 * - Impact level breakdown (critical, serious, moderate, minor)
 * - Detailed findings for each page area
 * - Professional styling and responsive design
 */

export interface AuditPageResult {
    pageName: string;
    pageCategory: string;
    url: string;
    summary: {
        violations: number;
        passes: number;
        totalRulesChecked: number;
    };
    impactBreakdown: Record<string, number>;
    impactViolations: Record<string, any[]>;
    violations: any[];
    passes: any[];
}

export const generateComprehensiveAccessibilityReport = (allResults: AuditPageResult[], timestamp: string): string => {
    // Calculate overall statistics
    const overallStats = {
        totalPages: allResults.length,
        totalViolations: 0,
        totalPasses: 0,
        impactBreakdown: {
            critical: 0,
            serious: 0,
            moderate: 0,
            minor: 0,
            unknown: 0
        }
    };

    allResults.forEach(page => {
        overallStats.totalViolations += page.summary.violations;
        overallStats.totalPasses += page.summary.passes;

        Object.entries(page.impactBreakdown).forEach(([impact, count]) => {
            overallStats.impactBreakdown[impact as keyof typeof overallStats.impactBreakdown] += count;
        });
    });

    return `<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Comprehensive Accessibility Audit Report - Find Information about Schools and Trusts</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; background: #f5f5f5; line-height: 1.6; }
        .container { max-width: 1400px; margin: 0 auto; background: white; padding: 40px; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.1); }
        .header { border-bottom: 4px solid #1f7a8c; padding-bottom: 30px; margin-bottom: 40px; text-align: center; }
        .header h1 { color: #1f7a8c; margin: 0 0 15px 0; font-size: 2.5em; }
        .header .meta { color: #666; font-size: 1.1em; }
        .header .meta strong { color: #333; }
        
        .executive-summary { background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%); padding: 30px; border-radius: 12px; margin-bottom: 40px; border-left: 6px solid #1f7a8c; }
        .executive-summary h2 { margin-top: 0; color: #1f7a8c; font-size: 1.8em; }
        .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin: 25px 0; }
        .stat-card { background: white; padding: 25px; border-radius: 8px; text-align: center; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
        .stat-card h3 { margin: 0 0 10px 0; font-size: 2.2em; color: #333; }
        .stat-card p { margin: 0; color: #666; font-weight: 600; }
        .stat-card.total { border-left: 6px solid #6c757d; }
        .stat-card.passed { border-left: 6px solid #28a745; }
        .stat-card.failed { border-left: 6px solid #dc3545; }
        .stat-card.incomplete { border-left: 6px solid #ffc107; }

        .impact-overview { margin: 30px 0; }
        .impact-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(140px, 1fr)); gap: 15px; margin: 20px 0; }
        .impact-card { padding: 20px; border-radius: 8px; text-align: center; color: white; font-weight: bold; }
        .impact-card.critical { background: linear-gradient(135deg, #dc3545, #c82333); }
        .impact-card.serious { background: linear-gradient(135deg, #fd7e14, #e8650e); }
        .impact-card.moderate { background: linear-gradient(135deg, #ffc107, #e0a800); color: #000; }
        .impact-card.minor { background: linear-gradient(135deg, #20c997, #17a689); }
        .impact-card.unknown { background: linear-gradient(135deg, #6c757d, #5a6268); }
        .impact-card h3 { margin: 0 0 8px 0; font-size: 2em; }
        .impact-card p { margin: 0; font-size: 0.9em; }

        .page-results { margin: 40px 0; }
        .page-category { margin: 30px 0; }
        .category-header { background: #343a40; color: white; padding: 15px 25px; border-radius: 8px 8px 0 0; margin: 0; font-size: 1.3em; }
        .page-list { background: #f8f9fa; border: 1px solid #dee2e6; border-top: none; border-radius: 0 0 8px 8px; }
        .page-item { padding: 20px 25px; border-bottom: 1px solid #dee2e6; }
        .page-item:last-child { border-bottom: none; }
        .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
        .page-name { font-weight: bold; font-size: 1.1em; color: #333; }
        .page-url { color: #666; font-size: 0.9em; font-family: monospace; }
        .page-stats { display: flex; gap: 20px; flex-wrap: wrap; }
        .page-stat { display: flex; align-items: center; gap: 5px; }
        .badge { display: inline-block; padding: 4px 8px; border-radius: 12px; font-size: 0.8em; font-weight: bold; color: white; }
        .badge.passed { background: #28a745; }
        .badge.failed { background: #dc3545; }
        .badge.incomplete { background: #ffc107; color: #000; }
        .badge.critical { background: #dc3545; }
        .badge.serious { background: #fd7e14; }
        .badge.moderate { background: #ffc107; color: #000; }
        .badge.minor { background: #20c997; }

        .violations-detail { margin: 40px 0; }
        .violation-item { background: #fff; border: 1px solid #dee2e6; border-radius: 8px; margin: 15px 0; overflow: hidden; }
        .violation-header { background: #f8d7da; padding: 15px 20px; border-bottom: 1px solid #f5c6cb; }
        .violation-content { padding: 20px; }
        .violation-title { font-weight: bold; color: #721c24; margin: 0 0 5px 0; }
        .violation-meta { color: #666; font-size: 0.9em; }
        .violation-description { margin: 15px 0; }
        .violation-help { background: #e2e3e5; padding: 15px; border-radius: 6px; margin: 15px 0; }
        .affected-elements { margin: 15px 0; }
        .element-list { background: #f8f9fa; padding: 15px; border-radius: 6px; }
        .element-item { font-family: monospace; background: #e9ecef; padding: 8px 12px; margin: 5px 0; border-radius: 4px; font-size: 0.9em; }

        .footer { margin-top: 60px; padding-top: 30px; border-top: 2px solid #e9ecef; text-align: center; color: #666; }
        .recommendation { background: #d1ecf1; border: 1px solid #bee5eb; padding: 20px; border-radius: 8px; margin: 20px 0; }
        .recommendation h4 { color: #0c5460; margin-top: 0; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🛡️ Comprehensive Accessibility Audit Report</h1>
            <div class="meta">
                <strong>Application:</strong> Find Information about Schools and Trusts<br>
                <strong>Generated:</strong> ${timestamp}<br>
                <strong>Pages Audited:</strong> ${overallStats.totalPages}<br>
                <strong>Total Rules Evaluated:</strong> ${overallStats.totalPasses + overallStats.totalViolations}
            </div>
        </div>

        <div class="executive-summary">
            <h2>📊 Executive Summary</h2>
            <p>This comprehensive audit evaluated <strong>${overallStats.totalPages} key pages</strong> across the Find Information about Schools and Trusts application, testing against <strong>all applicable WCAG 2.1 AA accessibility standards</strong>.</p>
            
            <div class="stats-grid">
                <div class="stat-card total">
                    <h3>${overallStats.totalPages}</h3>
                    <p>Pages Audited</p>
                </div>
                <div class="stat-card passed">
                    <h3>${overallStats.totalPasses}</h3>
                    <p>Rules Passed</p>
                </div>
                <div class="stat-card failed">
                    <h3>${overallStats.totalViolations}</h3>
                    <p>Violations Found</p>
                </div>

            </div>

            ${overallStats.totalViolations > 0 ? `
            <div class="impact-overview">
                <h3>🚨 Violations by Impact Level</h3>
                <div class="impact-grid">
                    ${Object.entries(overallStats.impactBreakdown)
                .filter(([, count]) => count > 0)
                .map(([impact, count]) => `
                            <div class="impact-card ${impact}">
                                <h3>${count}</h3>
                                <p>${impact.toUpperCase()}</p>
                            </div>
                        `).join('')}
                </div>
            </div>
            ` : `
            <div class="recommendation">
                <h4>🎉 Excellent Accessibility Compliance!</h4>
                <p>No accessibility violations were found across any of the audited pages. This indicates strong adherence to WCAG 2.1 AA standards.</p>
            </div>
            `}
        </div>

        <div class="page-results">
            <h2>📋 Detailed Results by Application Area</h2>

            ${['Core', 'Trusts', 'Schools', 'Manage Projects and Cases'].map(category => {
                    const categoryPages = allResults.filter(page => page.pageCategory === category);
                    if (categoryPages.length === 0) return '';

                    return `
                <div class="page-category">
                    <h3 class="category-header">${category} Pages</h3>
                    <div class="page-list">
                        ${categoryPages.map(page => `
                            <div class="page-item">
                                <div class="page-header">
                                    <div>
                                        <div class="page-name">${page.pageName}</div>
                                        <div class="page-url">${page.url}</div>
                                    </div>
                                </div>
                                <div class="page-stats">
                                    <div class="page-stat">
                                        <span class="badge passed">${page.summary.passes} Passed</span>
                                    </div>
                                    ${page.summary.violations > 0 ? `<div class="page-stat"><span class="badge failed">${page.summary.violations} Failed</span></div>` : ''}
                                    ${Object.entries(page.impactBreakdown)
                            .filter(([, count]) => count > 0)
                            .map(([impact, count]) => `<div class="page-stat"><span class="badge ${impact}">${impact}: ${count}</span></div>`)
                            .join('')}
                                </div>
                            </div>
                        `).join('')}
                    </div>
                </div>
                `;
                }).join('')}
        </div>

        ${overallStats.totalViolations > 0 ? `
        <div class="violations-detail">
            <h2>🔍 Detailed Violation Analysis</h2>
            ${Object.entries(overallStats.impactBreakdown)
                .filter(([, count]) => count > 0)
                .sort(([a], [b]) => {
                    const order = { critical: 1, serious: 2, moderate: 3, minor: 4, unknown: 5 };
                    return (order[a as keyof typeof order] || 99) - (order[b as keyof typeof order] || 99);
                })
                .map(([impact]) => {
                    const violationsOfThisImpact: any[] = [];
                    allResults.forEach(page => {
                        page.violations.forEach((violation: any) => {
                            const violationImpact = violation.impact ?? 'unknown';
                            if (violationImpact === impact) {
                                violationsOfThisImpact.push({
                                    ...violation,
                                    pageName: page.pageName,
                                    pageUrl: page.url
                                });
                            }
                        });
                    });

                    if (violationsOfThisImpact.length === 0) return '';

                    // Extract color logic for readability
                    const getImpactColor = (impactLevel: string): string => {
                        switch (impactLevel) {
                            case 'critical': return '#dc3545';
                            case 'serious': return '#fd7e14';
                            case 'moderate': return '#ffc107';
                            case 'minor': return '#20c997';
                            default: return '#6c757d';
                        }
                    };

                    const impactColor = getImpactColor(impact);

                    return `
                    <div class="impact-section">
                        <h3 style="color: ${impactColor};">
                            ${impact.toUpperCase()} Impact Violations (${violationsOfThisImpact.length})
                        </h3>
                        ${violationsOfThisImpact.map(violation => {
                        // Extract nested conditions for clarity
                        const hasNodes = violation.nodes && violation.nodes.length > 0;
                        const hasExtraNodes = violation.nodes && violation.nodes.length > 5;
                        const hasHelpUrl = violation.helpUrl;

                        const nodesList = hasNodes ? violation.nodes.slice(0, 5).map((node: any) => `
                                <div class="element-item">${node.target ? node.target.join(' > ') : 'Unknown element'}</div>
                            `).join('') : '';

                        const extraNodesText = hasExtraNodes ?
                            `<div style="margin-top: 10px; color: #666; font-style: italic;">... and ${violation.nodes.length - 5} more elements</div>` : '';

                        const affectedElementsSection = hasNodes ? `
                                <div class="affected-elements">
                                    <strong>Affected Elements (${violation.nodes.length}):</strong>
                                    <div class="element-list">
                                        ${nodesList}
                                        ${extraNodesText}
                                    </div>
                                </div>
                            ` : '';

                        const helpUrlSection = hasHelpUrl ? `
                                <div style="margin-top: 15px;">
                                    <a href="${violation.helpUrl}" target="_blank" style="color: #007bff; text-decoration: none;">📖 Learn more about this rule</a>
                                </div>
                            ` : '';

                        return `
                                <div class="violation-item">
                                    <div class="violation-header">
                                        <div class="violation-title">${violation.id}: ${violation.description}</div>
                                        <div class="violation-meta">Found on: ${violation.pageName} (${violation.pageUrl})</div>
                                    </div>
                                    <div class="violation-content">
                                        <div class="violation-description">
                                            <strong>Issue:</strong> ${violation.description}
                                        </div>
                                        <div class="violation-help">
                                            <strong>How to fix:</strong> ${violation.help}
                                        </div>
                                        ${affectedElementsSection}
                                        ${helpUrlSection}
                                    </div>
                                </div>
                            `;
                    }).join('')}
                    </div>
                    `;
                }).join('')}
        </div>
        ` : ''}

        <div class="footer">
            <p><strong>Report generated by:</strong> Cypress wick-a11y automated accessibility testing</p>
            <p><strong>Standards:</strong> WCAG 2.1 Level AA compliance evaluation</p>
            <p><strong>Engine:</strong> axe-core® accessibility testing engine</p>
            <p><strong>Generated:</strong> ${timestamp}</p>
        </div>
    </div>
</body>
</html>`;
}; 
