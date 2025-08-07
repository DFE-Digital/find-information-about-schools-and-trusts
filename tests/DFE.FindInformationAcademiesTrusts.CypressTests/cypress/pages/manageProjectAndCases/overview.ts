class Overview {
    elements = {
        projectListCount: () =>
            cy.get('[data-testid="select-projectlist-filter-count"]'),
        mainHeading: () => cy.get('[data-testid="manage-my-casework-heading"]'),
        filters: {
            clearFilters: () => cy.get('[data-testid="clear-filter"]'),
            toggleButtons: {
                system: () => cy.get('[data-testid="select-projectlist-filter-system"]'),
                projectType: () => cy.get('[data-testid="select-projectlist-filter-project-type"]'),
            },
            systemFilters: {
                complete: () =>
                    cy
                        .get("#system-filter-section")
                        .contains(
                            "Complete conversions, transfers and changes"
                        ),
                mfsp: () =>
                    cy
                        .get("#system-filter-section")
                        .contains("Manage free school projects"),
                prepare: () =>
                    cy
                        .get("#system-filter-section")
                        .contains("Prepare conversions and transfers"),
                recast: () =>
                    cy
                        .get("#system-filter-section")
                        .contains("Record concerns and support for trusts"),
            },
            projectTypeFilters: {
                conversion: () =>
                    cy
                        .get("#projecttype-filter-section")
                        .contains("Conversion"),
                formAMat: () =>
                    cy
                        .get("#projecttype-filter-section")
                        .contains("Form a MAT"),
                governanceCapability: () =>
                    cy
                        .get("#projecttype-filter-section")
                        .contains("Governance capability"),
                nonCompliance: () =>
                    cy
                        .get("#projecttype-filter-section")
                        .contains("Non-compliance"),
                preOpening: () =>
                    cy
                        .get("#projecttype-filter-section")
                        .contains("Pre-opening"),
                preOpeningNotIncluded: () =>
                    cy
                        .get("#projecttype-filter-section")
                        .contains("Pre-opening - not included in figures"),
                safeguardingNonCompliance: () =>
                    cy
                        .get("#projecttype-filter-section")
                        .contains("Safeguarding non-compliance"),
                transfer: () =>
                    cy
                        .get("#projecttype-filter-section")
                        .contains("Transfer"),
            },
            applyButton: () => cy.get('[data-testid="apply-filter-button"]'),
        },
        cases: {
            caseTitles: () => cy.get('[id^="case-title-"]'),
            systems: () => cy.get('[data-testid^="case-system-"]'),
            caseSystem: (caseIndex: number) =>
                cy.get(`[data-testid="case-system-${caseIndex}"]`),
            caseProjectType: (caseIndex: number) =>
                cy.get(`[data-testid="case-project-type-${caseIndex}"]`),
            caseItemsList: (caseIndex: number, itemIndex: number) =>
                cy.get(`#case-item-${caseIndex}-${itemIndex}`),
        },
    };

    public CheckProjectCountIs(count: string): this {
        this.elements
            .projectListCount()
            .contains(`${count} result${count === "1" ? "" : "s"} found`);
        return this;
    }

    public CheckAllSystemsAreComplete(): this {
        this.elements.cases.systems().each(($el) => {
            cy.wrap($el).contains("Complete");
        });
        return this;
    }

    public CheckAllSystemsArePrepare(): this {
        this.elements.cases.systems().each(($el) => {
            cy.wrap($el).contains("Prepare conversions and transfers");
        });
        return this;
    }

    public ToggleSystemFilter(): this {
        this.elements.filters.toggleButtons.system().click();
        return this;
    }

    public ToggleProjectTypeFilter(): this {
        this.elements.filters.toggleButtons.projectType().click();
        return this;
    }

    public CheckSystemFiltersVisible(): this {
        this.elements.filters.systemFilters.complete().should("be.visible");
        this.elements.filters.systemFilters.mfsp().should("be.visible");
        this.elements.filters.systemFilters.prepare().should("be.visible");
        this.elements.filters.systemFilters.recast().should("be.visible");
        return this;
    }

    public ClickCompleteSystem(): this {
        this.elements.filters.systemFilters.complete().click();
        return this;
    }

    public ClickMfspSystem(): this {
        this.elements.filters.systemFilters.mfsp().click();
        return this;
    }

    public ClickPrepareSystem(): this {
        this.elements.filters.systemFilters.prepare().click();
        return this;
    }

    public ClickRecastSystem(): this {
        this.elements.filters.systemFilters.recast().click();
        return this;
    }

    public ClickApplyFilters(): this {
        this.elements.filters.applyButton().click();
        return this;
    }

    public ClickConversionProjectType(): this {
        this.elements.filters.projectTypeFilters.conversion().click();
        return this;
    }

    public ClickFormAMatProjectType(): this {
        this.elements.filters.projectTypeFilters.formAMat().click();
        return this;
    }

    public ClickGovernanceCapabilityProjectType(): this {
        this.elements.filters.projectTypeFilters.governanceCapability().click();
        return this;
    }

    public ClickNonComplianceProjectType(): this {
        this.elements.filters.projectTypeFilters.nonCompliance().click();
        return this;
    }

    public ClickPreOpeningProjectType(): this {
        this.elements.filters.projectTypeFilters.preOpening().click();
        return this;
    }

    public ClickPreOpeningNotIncludedProjectType(): this {
        this.elements.filters.projectTypeFilters.preOpeningNotIncluded().click();
        return this;
    }

    public ClickSafeguardingNonComplianceProjectType(): this {
        this.elements.filters.projectTypeFilters.safeguardingNonCompliance().click();
        return this;
    }

    public ClickTransferProjectType(): this {
        this.elements.filters.projectTypeFilters.transfer().click();
        return this;
    }

    public ClickClearFilters(): this {
        this.elements.filters.clearFilters().click();
        return this;
    }

    public CheckSortOrdering(): this {
        let originalTitles: string[] = [];

        this.elements.cases
            .caseTitles()
            .then(($els) => {
                originalTitles = Array.from($els, (el) => el.innerText);
            })
            .then(() => {
                cy.get("#sorting").select("createdAsc");
                cy.get("#sorting").trigger("change");
                cy.get("#sorting").should("have.value", "createdAsc");
                cy.get('[id^="case-title-"]').then(($els) => {
                    const newTitles = Array.from($els, (el) => el.innerText);
                    const expectedReversed = [...originalTitles].reverse();
                    console.log(newTitles);
                    console.log(originalTitles);
                    console.log(expectedReversed);
                    expect(newTitles).to.deep.equal(expectedReversed);
                });
            });

        return this;
    }

    public CheckCaseItemsHaveCorrectItems(caseIndex: number): this {
        this.elements.cases.caseSystem(caseIndex).then(($caseSystem) => {
            this.elements.cases
                .caseProjectType(caseIndex)
                .then(($caseProjectType) => {
                    const itemsToCheck = this.GetExpectedCaseItems(
                        $caseSystem,
                        $caseProjectType
                    );
                    itemsToCheck.forEach((expectedItem, caseItemIndex) => {
                        this.elements.cases
                            .caseItemsList(caseIndex, caseItemIndex)
                            .should("contain.text", expectedItem);
                    });
                });
        });
        return this;
    }

    private GetExpectedCaseItems(
        systemElement: JQuery<HTMLElement>,
        projectTypeElement: JQuery<HTMLElement>
    ): string[] {
        const systemText = systemElement.text().trim();
        const projectTypeText = projectTypeElement.text().trim();
        if (systemText === "Complete" && projectTypeText === "Conversion") {
            return ["Current confirmed conversion date", "Name", "LA"];
        }
        if (
            systemText === "Prepare conversions and transfers" &&
            projectTypeText === "Transfer"
        ) {
            return ["URN", "Incoming trust", "Outgoing trust", "Route"];
        }
        if (
            systemText === "Prepare conversions and transfers" &&
            projectTypeText === "Conversion"
        ) {
            return ["URN", "Incoming trust", "Local authority", "Route"];
        }
        if (
            systemText === "Manage free school projects" &&
            projectTypeText === "Presumption"
        ) {
            return [
                "Trust name",
                "Realistic year of opening",
                "School type",
                "Local authority",
                "Region",
            ];
        }
        if (
            systemText === "Manage free school projects" &&
            projectTypeText === "Central Route"
        ) {
            return [
                "Trust name",
                "Realistic year of opening",
                "School type",
                "Local authority",
                "Region",
            ];
        }

        return [];
    }
}

const overviewPage = new Overview();
export default overviewPage;
