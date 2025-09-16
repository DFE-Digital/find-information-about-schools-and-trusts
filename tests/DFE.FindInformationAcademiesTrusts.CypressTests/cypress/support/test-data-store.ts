export class TestDataStore {
    public static readonly GetTrustSubpagesFor = (uid: number, pageName: string) => {

        const page = TestDataStore.GetAllTrustSubpagesForUid(uid).find(p => p.pageName == pageName);

        if (page === undefined)
            throw new Error(`Page ${pageName} is not in the Cypress test data. Is this a new page that needs adding to TestDataStore?`);

        return page.subpages;
    };

    public static readonly GetAllTrustSubpagesForUid = (uid: number) =>
        [
            {
                pageName: "Overview",
                subpages: [
                    { subpageName: "Trust details", url: `/trusts/overview/trust-details?uid=${uid}` },
                    { subpageName: "Trust summary", url: `/trusts/overview/trust-summary?uid=${uid}` },
                    { subpageName: "Reference numbers", url: `/trusts/overview/reference-numbers?uid=${uid}` },
                ]
            },
            {
                pageName: "Contacts",
                subpages: [
                    { subpageName: "In DfE", url: `/trusts/contacts/in-dfe?uid=${uid}` },
                    { subpageName: "In this trust", url: `/trusts/contacts/in-the-trust?uid=${uid}` },
                ]
            },
            {
                pageName: "Ofsted",
                subpages: [
                    { subpageName: "Single headline grades", url: `/trusts/ofsted/single-headline-grades?uid=${uid}` },
                    { subpageName: "Current ratings", url: `/trusts/ofsted/current-ratings?uid=${uid}` },
                    { subpageName: "Previous ratings", url: `/trusts/ofsted/previous-ratings?uid=${uid}` },
                    { subpageName: "Safeguarding and concerns", url: `/trusts/ofsted/safeguarding-and-concerns?uid=${uid}` }
                ]
            },
            {
                pageName: "Governance",
                subpages: [
                    { subpageName: `Trust leadership`, url: `/trusts/governance/trust-leadership?uid=${uid}` },
                    { subpageName: "Trustees", url: `/trusts/governance/trustees?uid=${uid}` },
                    { subpageName: "Members", url: `/trusts/governance/members?uid=${uid}` },
                    { subpageName: "Historic members", url: `/trusts/governance/historic-members?uid=${uid}` }
                ]
            },
        ];

    public static readonly GetAllSchoolSubpagesForUrn = (urn: number) =>
        [
            {
                pageName: "Overview",
                subpages: [
                    { subpageName: "School details", url: `/schools/overview/details?urn=${urn}` },
                    { subpageName: "Federation details", url: `/schools/overview/federation?urn=${urn}` },
                    { subpageName: "Reference numbers", url: `/schools/overview/referencenumbers?urn=${urn}` },
                    { subpageName: "SEN (special educational needs)", url: `/schools/overview/sen?urn=${urn}` },
                    { subpageName: "Religious characteristics", url: `/schools/overview/religiouscharacteristics?urn=${urn}` }
                ]
            },
            {
                pageName: "Contacts",
                subpages: [
                    { subpageName: "In DfE", url: `/schools/contacts/in-dfe?urn=${urn}` },
                    { subpageName: "In this school", url: `/schools/contacts/in-the-school?urn=${urn}` },
                ]
            },
            {
                pageName: "Ofsted",
                subpages: [
                    { subpageName: "Single headline grades", url: `/schools/ofsted/singleheadlinegrades?urn=${urn}` },
                    { subpageName: "Current ratings", url: `/schools/ofsted/currentratings?urn=${urn}` },
                    { subpageName: "Previous ratings", url: `/schools/ofsted/previousratings?urn=${urn}` },
                    { subpageName: "Safeguarding and concerns", url: `/schools/ofsted/safeguardingandconcerns?urn=${urn}` }
                ]
            },
            {
                pageName: "Governance",
                subpages: [
                    { subpageName: "Current governors", url: `/schools/governance/current?urn=${urn}` },
                    { subpageName: "Historic governors", url: `/schools/governance/historic?urn=${urn}` },
                ]
            },
        ];

    public static readonly GetAllAcademySubpagesForUrn = (urn: number) =>
        [
            {
                pageName: "Overview",
                subpages: [
                    { subpageName: "Academy details", url: `/schools/overview/details?urn=${urn}` },
                    { subpageName: "Reference numbers", url: `/schools/overview/referencenumbers?urn=${urn}` },
                    { subpageName: "SEN (special educational needs)", url: `/schools/overview/sen?urn=${urn}` },
                    { subpageName: "Religious characteristics", url: `/schools/overview/religiouscharacteristics?urn=${urn}` }
                ]
            },
            {
                pageName: "Contacts",
                subpages: [
                    { subpageName: "In DfE", url: `/schools/contacts/in-dfe?urn=${urn}` },
                    { subpageName: "In this academy", url: `/schools/contacts/in-the-school?urn=${urn}` },
                ]
            },
            {
                pageName: "Ofsted",
                subpages: [
                    { subpageName: "Single headline grades", url: `/schools/ofsted/singleheadlinegrades?urn=${urn}` },
                    { subpageName: "Current ratings", url: `/schools/ofsted/currentratings?urn=${urn}` },
                    { subpageName: "Previous ratings", url: `/schools/ofsted/previousratings?urn=${urn}` },
                    { subpageName: "Safeguarding and concerns", url: `/schools/ofsted/safeguardingandconcerns?urn=${urn}` }
                ]
            },
            {
                pageName: "Governance",
                subpages: [
                    { subpageName: "Current governors", url: `/schools/governance/current?urn=${urn}` },
                    { subpageName: "Historic governors", url: `/schools/governance/historic?urn=${urn}` },
                ]
            }
        ];
}

export const testSchoolData = [
    {
        schoolName: "The Meadows Primary School",
        typeOfSchool: "Community school",
        urn: 123452,
        schoolOrAcademy: "school"
    },
    {
        schoolName: "Abbey Grange Church of England Academy",
        typeOfSchool: "Academy converter",
        urn: 137083,
        schoolOrAcademy: "academy"
    }
];

export const testPreAdvisoryData = [
    {
        uid: 16002
    },
    {
        uid: 4921
    }
];

export const testPostAdvisoryData = [
    {
        uid: 17584
    },
    {
        uid: 16857
    }
];

export const testFreeSchoolsData = [
    {
        uid: 17538
    },
    {
        uid: 15786
    }
];

export const testTrustData = [
    {
        trustName: "Ashton West End Primary Academy",
        typeOfTrust: "single academy trust with contacts",
        uid: 5527
    },
    {
        trustName: "Aspire North East Multi Academy Trust",
        typeOfTrust: "multi academy trust with contacts",
        uid: 5712
    }
];

export const trustsWithGovernanceData = [
    {
        typeOfTrust: "single academy trust with governance data",
        uid: 5527
    },
    {
        typeOfTrust: "multi academy trust with governance data",
        uid: 5712
    }
];

// Trust UIDs for financial documents testing across multiple test suites
export const testFinanceData = [
    {
        uid: 5143
    },
    {
        uid: 4921
    }
];

// Trust UIDs with Ofsted inspection data for comprehensive testing
export const testTrustOfstedData = [
    {
        typeOfTrust: "single academy trust",
        uid: 5527
    },
    {
        typeOfTrust: "multi academy trust",
        uid: 5712
    }
];

// Trust UIDs with no governance data for testing empty state scenarios
export const trustsWithNoGovernanceData = [
    {
        typeOfTrust: "single academy trust with no governance data",
        uid: 17022
    },
    {
        typeOfTrust: "multi academy trust with no governance data",
        uid: 17637
    }
];

// School URNs for testing federation functionality and edge cases
export const testFederationData = {
    schoolWithFederationDetails: {
        urn: 107188,
        type: "Community school with federation details"
    },
    schoolWithoutFederationDetails: {
        urn: 100000,
        type: "School with no federation"
    },
    academy: {
        urn: 142768,
        type: "Academy (no federation)"
    }
};

// School URNs for testing breadcrumb navigation across different school types
export const testBreadcrumbSchoolData = {
    communitySchool: {
        urn: 107188,
        type: "Community school"
    },
    academyConverter: {
        urn: 137083,
        type: "Academy converter"
    }
};

// School URNs with SEN provision data for testing special educational needs functionality
export const senSchoolData = [
    {
        typeOfSchool: "school with SEN provision",
        urn: 122957
    },
    {
        typeOfSchool: "academy with SEN provision",
        urn: 143934
    }
];

// School URNs with governance data for testing school governance functionality
export const schoolsWithGovernanceData = [
    {
        typeOfSchool: "community school with governance data",
        urn: 107188,
        description: "Abbey Green Nursery School - Local authority nursery school with current and historic governors"
    },
    {
        typeOfSchool: "academy with governance data",
        urn: 134224,
        description: "Manchester Academy - Academy sponsor led with governance information"
    }
];

// School URNs with no governance data for testing empty state scenarios
export const schoolsWithNoGovernanceData = [
    {
        typeOfSchool: "school with no governance data",
        urn: 130423,
        description: "School with no current or historic governors for testing empty states"
    }
];

// School URNs for comprehensive governance testing across different school types
export const testSchoolGovernanceData = [
    {
        urn: 107188,
        type: "Community school",
        description: "Community school with comprehensive governance data"
    },
    {
        urn: 137083,
        type: "Academy converter",
        description: "Academy with comprehensive governance data"
    }
];

// School URNs with religious characteristics data for testing religious characteristics functionality
export const religiousCharacteristicsSchoolData = [
    {
        typeOfSchool: "school",
        urn: 152130
    },
    {
        typeOfSchool: "academy",
        urn: 147669
    }
];


// Trust UID for testing empty data states and error scenarios
export const testTrustWithNoDataUid = 17728;

// Primary test entities for navigation and general functionality testing
export const testNavigationData = {
    trustUid: 5527,
    schoolUrn: 107188,
    academyUrn: 140214
};

// Trust UIDs with sufficient academy data for detailed testing scenarios
export const testAcademiesData = [
    {
        uid: 5143,
        description: "Trust with academy data for testing details, pupil numbers, and free school meals"
    },
    {
        uid: 5712,
        description: "Multi-academy trust for testing academy listings"
    }
];

// Trust UIDs for pipeline academy testing across different stages
export const testPipelineData = {
    preAdvisoryUid: 16002,
    postAdvisoryUid: 17584,
    freeSchoolsUid: 17584
};

// Trust UID with sufficient Ofsted data for comprehensive inspection testing
export const testOfstedWithDataUid = 5143;

// Trust UIDs with sufficient data volume for pagination component testing
export const testPaginationData = {
    academiesInTrustUid: 5143,
    ofstedRatingsUid: 5143
};

export const referenceNumbersTestData = [
    {
        urn: 122957,
        description: 'local authority maintained school'
    },
    {
        urn: 136354,
        description: 'school in a trust'
    }
];
