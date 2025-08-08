import homePage from "../../pages/homePage";
import navigation from "../../pages/navigation";
import commonPage from "../../pages/commonPage";

describe("Testing the components of the home page", () => {
  beforeEach(() => {
    cy.visit("/");
  });

  it("Checks the browser title is correct", () => {
    commonPage
      .checkThatBrowserTitleMatches("Home page - Find information about schools and trusts");
  });

  it("Should check that the what you can find section is collapsed when you first land on the home screen ", () => {
    homePage
      .checkWhatYouCanFindPresent()
      .checkWhatYouCanFindListCollapsed();
  });

  it("Should check that the what you can find section is collapsed when you return to the home screen ", () => {
    homePage
      .checkWhatYouCanFindPresent()
      .checkWhatYouCanFindListCollapsed();

    cy.visit("/trusts/contacts/in-dfe?uid=5712");

    navigation
      .checkCurrentURLIsCorrect("/trusts/contacts/in-dfe?uid=5712");

    cy.visit("/");

    homePage
      .checkWhatYouCanFindPresent()
      .checkWhatYouCanFindListCollapsed();
  });

  it("Should check that the what you can find section is expanded when clicked on ", () => {
    homePage
      .checkWhatYouCanFindPresent()
      .clickWhatYouCanFindList()
      .checkWhatYouCanFindListOpen();
  });

  it("Should check that the expected contents of the what you can find section are all present", () => {
    homePage
      .checkWhatYouCanFindPresent()
      .clickWhatYouCanFindList()
      .checkWhatYouCanFindListOpen()
      .checkWhatYouCanFindListItemsPresent();
  });

  it("Should not have Manage projects and cases present", () => {
    homePage
      .checkIfManageProjectsAndCasesNotPresent()
  });
});

describe("Check manage projects and cases role", () => {
  beforeEach(() => {
    cy.login({role: "User.Role.MPCViewer"})
    cy.visit("/");
  });

  it("Should have Manage projects and cases present", () => {
    homePage
      .checkIfManageProjectsAndCasesPresent()
  });
});
