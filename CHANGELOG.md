# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). To see an example from a mature product in the program [see the Complete products changelog that follows the same methodology](https://github.com/DFE-Digital/dfe-complete-conversions-transfers-and-changes/blob/main/CHANGELOG.md).

## [Unreleased][unreleased]

## [Release-40][release-40] (production-2025-07-04.6098)

### Changed

- Introduced a notification banner for scheduled downtime.
- Added the ability to edit DfE contacts for a school.

## [Release-39][release-39] (production-2025-07-02.6062)

### Changed

- Added link and details to schools pages that are not academies but are part of a trust.
- Automation tests for a11y.
- Added contacts 'in dfe' for school and academy.

## [Release-38][release-38] (production-2025-06-20.5972)

### Changed

- Fix HTTP 500 response when an academy is missing trust information 

## [Release-37][release-37] (production-2025-06-20.5960)

### Changed

- Added federations to school pages

## [Release-36][release-36] (production-2025-06-10.5886)

### Changed

- Added ability to view contacts in 'this school' and academy.
- Added new SQL table for upcoming changes to edit school contacts within DfE.

## [Release-35][release-35] (production-2025-06-04.5832)

### Changed

- Added school and academy SEN details.

## [Release-34][release-34] (production-2025-06-02.5812)

### Changed

- Fixed typo on information on what you can search for (of/or)
- Changed H3 to be a label and cleaup for better accessibility 

## [Release-33][release-33] (production-2025-05-29.5786)

### Changed

- Added auto complete search results for schools/academies.
- Added search results page for schools/academies.
- Increased number of results per page to 20

## [Release-32][release-32] (production-2025-05-21.5738)

### Changed

- Fix to set phase and age range in the format 'Phase 1-100'
- Fix to set age range in the format '1-100'
- Accessibility fixes for percentage sign being in a span or having a space after the number
- Date columns all sort correctly by date value
- Fixed missing pipeline academy sort by URN

## [Release-31][release-31] (production-2025-05-19.5713)

### Changed

- Moved single and multi academy trust filters out of global filters
- Added text to single headline grades page to clarify where the data came from
- Added filter to only show schools that are not closed
- Overview pages for academy and schools

## [Release-30][release-30] (production-2025-05-06.5593)

### Changed

- Added date joined trust within the 'Details' page of the Academies 'In this trust' page
- Removed England/Wales from the Rural or URban column within the 'Details' page of the Academies 'In this trust' page.

## [Release-29][release-29] (production-2025-04-10.5387)

### Changed

- Re-enable single headline grades for further education establishments

## [Release-28][release-28] (production-2025-04-04.5337)

### Changed

- Reordered academy download columns to match order in ofsted download
- Added local authority and national free school meals percentages to academy download

## [Release-27][release-27] (production-2025-04-02.5314)

### Changed

- Add financial document pages

## [Release-26][release-26] (production-2025-03-26.5267)

### Changed

- Bug fix to make ofsted grades available for new schools

## [Release-25][release-25] (production-2025-03-19.5191)

### Changed

- Bug fix for when there are multiple contacts for the same role
- Added feature flag to hide governance turnover calculation

## [Release-24][release-24] (production-2025-03-11.5152)

### Changed

- Move "internal use only" out of contact cards to single box at the top of the "Contacts in this trust" page

## [Release-23][release-23] (production-2025-02-28.5074)

### Changed

- Pipeline academies pages no longer show conversion projects which are "dAO revoked"

## [Release-22][release-22] (production-2025-02-27.5055)

### Added

- Added pipeline academies sub page and tabs
- Added pipeline academies spreadsheet download capability

### Changed

- Existing academies tabs moved to "Academies in this trust" sub page with new urls e.g. `/trusts/academies/in-trust/details`
- Renamed "Contacts in the trust" to "Contacts in this trust"
- Datasource component text changed from "All information taken from" to "All information was taken from"

## [Release-21][release-21] (production-2025-02-20.5021)

### Changed

- Changed Ofsted data download to include single headline grades
- Use buttons instead of links to accept/decline cookies in cookie banner

## [Release-20][release-20] (production-2025-02-04.4852)

### Added

- Added Single headline grades Ofsted subpage

### Changed

- Updated accessibility statement text
- Page spacing tweak on Ofsted subpages
- All Ofsted subpage tables - previous sub-judgements for academies with no previous inspection now say "Not inspected" (instead of "Not yet inspected" which is used for current sub-judgements)
- Ofsted data download - previous sub-judgements for academies with no previous inspection now say "Not inspected" (instead of "Not yet inspected" which is used for current sub-judgements)

### Removed

- Removed 'Date joined trust' from Ofsted data source list
- Removed Important dates Ofsted subpage

## [Release-19][release-19] (production-2025-01-20.4730)

### Changed

- Fix Privacy page having incorrect width
- Update name of cookie consent cookie to be consistent with application name and what is displayed in the Cookie UI
- Switched to using the MIS tables to the MIS_MSTR tables
- Updated the design of the source and updates panel

## [Release-18][release-18] (production-2024-12-19.4567)

### Added

- Added SharePoint folder link to details page

### Changed

- Updated the autocomplete so that searching clears the selected trust

## [Release-17][release-17] (production-2024-12-18.4551)

### Added

- Added download for Ofsted specific data to Ofsted pages

### Changed

- Fixed browser titles so they include the name of the sub page and are all formatted in a consistent way

## [Release-16][release-16] (production-2024-12-13.4477)

### Changed

- Academies pages are now tabs instead of using sub nav
- Move phase banner from header to footer
- Change from schools benchmarking tool to financial benchmarking and insights tool
- Update dependencies
- Update breadcrumbs to add page name
- Change `span` to `strong` to highlight text emphasis to screen readers on the Ofsted Safeguarding page
- Fix page on the cancel button on the edit contacts form

### Removed

- Removed the "Report a problem" link from most pages

## [Release-15][release-15] (production-2024-12-02.4279)

### Added

- Add trust page sub navigation
- Added Ofsted pages to service nav level

### Removed

- Remove academies in trust Ofsted page

### Changed

- Split up all trust pages into sub pages

## [Release-14][release-14] (production-2024-11-21.4136)

### Changed

- Overall effectiveness ofsted rating can now be 'not judged'
- Improve performance of search page and search autocomplete
- Minor performance improvement to all academies db database calls
- Refactored and split up the program file into separate config files

## [Release-13][release-13] (production-2024-11-14.4036)

### Added

- Added Governance turnover to governance page

### Changed

- Updated the node version in the github runners and docker image

## [Release-12][release-12] (production-2024-11-13.3974)

### Changed

- Updated the contacts page to include disclaimer text for the trust contacts
- Removed legacy trust provider and associated code
- Improved performance of academies data export
- Updated the landing page to add more information and links to other services
- Renamed the anti forgery cookie to a static name
- Updated wording for links on the landing page to tell users they open in new tabs
- Updated the ofsted ratings to collect information about the ofsted subgrades
- Remove the edit contact feature flag
- Change Trust Navigation from side nav to top nav (service navigation component)
- Update the Govuk frontend package to ^5.7.1
- Moved breadcrumbs into the trust banner
- Merged Trust details page into Trust Overview page
- Redirect successful search to Overview page
- Removed Ofsted summary card from Trust Overview

## [Release-11][release-11] (production-2024-10-17.3654)

### Changed

- The links on the error page now open in a new tab

## [Release-10][release-10] (production-2024-10-17.3629)

### Changed

- Changed when we intercept automation headers to give automation users claims for Edit contact tests
- Academies in this trust export now includes Age Range
- Updated the font to use the DfE standard Inter font
- Added data test ids to the source list
- Updated the footer link to the PASS form and added a feature flag to control the release

## [Release-9][release-9] (production-2024-10-02.3440)

### Added

- Added the ability to search for a trust by Trust Reference Number (TRN)

### Changed

- Move Successful update message to top of the trust layout for conatcts update
- Manually edit the Updated By text for TRAMs Migration to TRAMS Migration in the UI

## [Release-8][release-8] (production-2024-09-27.3319)

### Added

- Run migrations for FIAT database on application container startup
- Governance pages will now have relevant data for SATs, using the URN as an alternative to the trust UID
- Contacts pages will now have relevant data for SATs, using the URN as an alternative to the trust UID
- All Trust contacts now have the internal data that pertains to their email addresses, where present in our unpublished GIAS data.

### Changed

- Ofsted data now uses inspection start date as inspection date
- Reduced the load time of the Trust overview page
- DfE contacts now get their data from Fiat Db instead of academies Db
- Edited contacts are now saved to the database

## [Release-7][release-7] (production-2024-09-23.3213)

### Added

- Created Edit contacts UI and feature flag to hide it on production environment

### Changed

- Ofsted logic can now handle null dates
- Updated contacts page UI to split each contact into it own section

## [Release-6][release-6] (production-2024-09-17.3129)

### Added

- Academies in this trust data now exportable
- Add Home link to all pages except "there is a problem with the product" page

### Changed

- Reduced the load time of the Academies in trust pupil numbers page
- Reduced the load time of the Academies in trust free school meals page
- Reduced the load time of the Trust contacts page

## [Release-5][release-5] (production-2024-09-05.2971)

### Added

- Add Trust Governance Page

### Changed

- Update the sorting behaviour for dates on the tables
- Reduced the load time of the Academies in trust Ofsted page

### Removed

- Removed Scottish design library as we no longer have a sidenav

## [Release-4][release-4] (production-2024-08-22.2794)

### Added

- Feature flags added to allow for selectively showing features in different environments
- Add search autocomplete to the header of the application

### Changed

- Fix side nav not showing current for "Academies in trust" pages
- Major updates to UI dependencies including:
  - GOV.UK design system (v4.8 -> v5.5)
  - DfE frontend (alpha -> v2)
  - Scottish design system (sidenav in Trust pages)
  - MoJ frontend (sortable tables)
  - Accessible autocomplete (search autocomplete)
  - Webpack bundlers/loaders
- Updated feedback url to match RSD feedback form.
- Reduced the load time of the Academies in trust details page
- Updated the footer design to match the updated RSD design.

## [Release-3][release-3] (production-2024-08-09.2694)

### Changed

- Accessibility statement has been updated to reflect the recent accessibility audit.
- Trust details page load time decreased.
- Implemented service and repository design patterns for retrieving data from AcademiesDb.
- Data source information cached to increase performance of all Trust pages.
- Trust summary information (UID, name, type and number of academies) cached to improve performance of all Trust pages.
- Updated hardcoded free schools meals data to be updated to 23/24, resolving a bug relating to new Local Authorities within that data

## [Release-2][release-2] (production-2024-07-29.2601)

### Changed

- Filter so only the open Gias Groups (Trusts) are shown across the application
- Updated Feedback link to reflect new feedback form for the product

## [Release-1][release-1] (production-2024-07-18.2517)

### Changed

- Migrated the application to LTS .NET 8. (FIAT was on .NET 7 which Microsoft ended support for in May).
- Updated text on "404 - Not Found" page to adhere to DfE design pattern and added the not found url to the support email template.

[unreleased]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/compare/production-2025-07-04.6098...HEAD
[release-1]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-07-18.2517
[release-2]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-07-29.2601
[release-3]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-08-09.2694
[release-4]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-08-22.2794
[release-5]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-09-05.2971
[release-6]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-09-17.3129
[release-7]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-09-23.3213
[release-8]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-09-27.3319
[release-9]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-10-02.3440
[release-10]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-10-17.3629
[release-11]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-10-17.3654
[release-12]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-11-13.3974
[release-13]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-11-14.4036
[release-14]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-11-21.4136
[release-15]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-12-02.4279
[release-16]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-12-13.4477
[release-17]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-12-18.4551
[release-18]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2024-12-19.4567
[release-19]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-01-20.4730
[release-20]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-02-04.4852
[release-21]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-02-20.5021
[release-22]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-02-27.5055  
[release-23]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-02-28.5074
[release-24]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-03-11.5152
[release-25]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-03-19.5191
[release-26]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-03-26.5267
[release-27]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-04-02.5314
[release-28]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-04-04.5337
[release-29]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-04-10.5387
[release-30]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-05-06.5593
[release-31]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-05-19.5713
[release-32]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-05-21.5738
[release-33]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-05-29.5786
[release-34]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-06-02.5812
[release-35]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-06-04.5832
[release-36]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-06-10.5886
[release-37]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-06-20.5960
[release-38]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-06-20.5972
[release-39]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-07-02.6062
[release-40]:
  https://github.com/DFE-Digital/find-information-about-academies-and-trusts/releases/tag/production-2025-07-04.6098
