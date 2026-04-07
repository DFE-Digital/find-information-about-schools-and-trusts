import overview from "../../../pages/manageProjectAndCases/overview";

describe("Testing the components of the home page", () => {
  beforeEach(() => {
    cy.login({role: "User.Role.MPCViewer"});
    cy.visit("/manageprojectsandcases/overview/", { failOnStatusCode: false });
  });

  it("Should load with all projects visible", () => {
    overview
      .ClickApplyFilters()
      .CheckProjectCountIs("11")
      .CheckSortOrdering()
      .CheckCaseItemsHaveCorrectItemsRange(0, 10);
  });

  it("Should apply filters for different systems", () => {
    overview
      .CheckSystemFiltersVisible()
      .ClickCompleteSystem()
      .ClickApplyFilters()
      .CheckProjectCountIs("1")
      .CheckAllSystemsAreComplete()
      .ClickCompleteSystem()
      .ClickPrepareSystem()
      .ClickApplyFilters()
      .CheckAllSystemsArePrepare()
      .CheckProjectCountIs("2")
      .ClickMfspSystem()
      .ClickApplyFilters()
      .CheckProjectCountIs("4")
      .ClickMfspSystem()
      .ClickPrepareSystem()
      .ClickRecastSystem()
      .ClickApplyFilters()
      .CheckProjectCountIs("0")
      .ClickClearFilters()
      .CheckProjectCountIs("11")
      .ToggleProjectTypeFilter()
      .ClickConversionProjectType()
      .ClickApplyFilters()
      .CheckProjectCountIs("2")
      .ClickCompleteSystem()
      .ClickApplyFilters()
      .CheckProjectCountIs("1")
      .CheckAllSystemsAreComplete()
      .ClickClearFilters()
      .CheckProjectCountIs("11")
      .ClickSigChangeSystem()
      .ClickApplyFilters()
      .CheckProjectCountIs("6")
      .CheckAllSystemsAreSigChange();
  });
});
