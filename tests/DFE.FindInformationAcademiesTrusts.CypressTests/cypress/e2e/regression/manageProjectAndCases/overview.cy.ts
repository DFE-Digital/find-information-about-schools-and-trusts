import overview from "../../../pages/manageProjectAndCases/overview";

describe("Testing the components of the home page", () => {
  beforeEach(() => {
    cy.login({role: "User.Role.MPCViewer"});
    cy.visit("/manageprojectsandcases/overview/");
  });

  it("Should load with all projects visible", () => {
    overview
      .CheckProjectCountIs("5")
      .CheckSortOrdering()
      .CheckCaseItemsHaveCorrectItems(0)
      .CheckCaseItemsHaveCorrectItems(1)
      .CheckCaseItemsHaveCorrectItems(2)
      .CheckCaseItemsHaveCorrectItems(3)
      .CheckCaseItemsHaveCorrectItems(4);
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
      .CheckProjectCountIs("5")
      .ToggleProjectTypeFilter()
      .ClickConversionProjectType()
      .ClickApplyFilters()
      .CheckProjectCountIs("2")
      .ClickCompleteSystem()
      .ClickApplyFilters()
      .CheckProjectCountIs("1")
      .CheckAllSystemsAreComplete()
  });
});


describe("Access control for /manageprojectsandcases/overview/", () => {
  it.only("should return 403 for unauthorized user", () => {
    cy.request("/manageprojectsandcases/overview/", {
      failOnStatusCode: false,
      headers: {
        'Authorization': `Bearer ${Cypress.env("AUTH_KEY")}`,
      }
    }).then((response) => {
      expect(response.status).to.eq(403);
    });
  });
});