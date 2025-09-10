using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Mis_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Mstr;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Ops;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Sharepoint;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Tad;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PipelineAcademy;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Mocks;

public class MockAcademiesDbContext
{
    public readonly IAcademiesDbContext Object = Substitute.For<IAcademiesDbContext>();

    //application
    public MockDbSet<ApplicationEvent> ApplicationEvents { get; } = new();
    public MockDbSet<ApplicationSetting> ApplicationSettings { get; } = new();

    //gias
    public MockDbSet<GiasEstablishment> GiasEstablishments { get; } =
        new(AcademiesDbContext.GiasEstablishmentsQueryFilter);

    public MockDbSet<GiasGovernance> GiasGovernances { get; } = new();
    public MockDbSet<GiasGroup> GiasGroups { get; } = new(AcademiesDbContext.GiasGroupQueryFilter);
    public MockDbSet<GiasGroupLink> GiasGroupLinks { get; } = new(AcademiesDbContext.GiasGroupLinkQueryFilter);

    public MockDbSet<GiasEstablishmentLink> GiasEstablishmentLinks { get; } =
        new(AcademiesDbContext.GiasEstablishmentLinkQueryFilter);

    //mis_mstr
    public MockDbSet<MisMstrEstablishmentFiat> MisMstrEstablishmentFiat { get; } = new();

    public MockDbSet<MisMstrFurtherEducationEstablishmentFiat> MisMstrFurtherEducationEstablishmentFiat { get; } =
        new();

    //mstr
    public MockDbSet<MstrTrust> MstrTrusts { get; } = new();
    public MockDbSet<MstrAcademyConversion> MstrAcademyConversions { get; } = new();
    public MockDbSet<MstrAcademyTransfer> MstrAcademyTransfers { get; } = new();
    public MockDbSet<MstrFreeSchoolProject> MstrFreeSchoolProjects { get; } = new();

    //sharepoint
    public MockDbSet<SharepointTrustDocLink> SharepointTrustDocLinks { get; } =
        new(AcademiesDbContext.SharepointTrustDocLinkQueryFilter);

    //tad
    public MockDbSet<TadHeadTeacherContact> TadHeadTeacherContacts { get; } = new();
    public MockDbSet<TadTrustGovernance> TadTrustGovernances { get; } = new();
    
    // Pupil Census
    public MockDbSet<EdperfFiat> EdperfFiats { get; } = new();

    public MockAcademiesDbContext()
    {
        //application
        Object.ApplicationEvents.Returns(ApplicationEvents.Object);
        Object.ApplicationSettings.Returns(ApplicationSettings.Object);
        //gias
        Object.GiasEstablishments.Returns(GiasEstablishments.Object);
        Object.GiasGovernances.Returns(GiasGovernances.Object);
        Object.Groups.Returns(GiasGroups.Object);
        Object.GiasGroupLinks.Returns(GiasGroupLinks.Object);
        Object.GiasEstablishmentLink.Returns(GiasEstablishmentLinks.Object);
        //mis_mstr
        Object.MisMstrEstablishmentsFiat.Returns(MisMstrEstablishmentFiat.Object);
        Object.MisMstrFurtherEducationEstablishmentsFiat.Returns(MisMstrFurtherEducationEstablishmentFiat.Object);
        //mstr
        Object.MstrAcademyConversions.Returns(MstrAcademyConversions.Object);
        Object.MstrAcademyTransfers.Returns(MstrAcademyTransfers.Object);
        Object.MstrFreeSchoolProjects.Returns(MstrFreeSchoolProjects.Object);
        Object.MstrTrusts.Returns(MstrTrusts.Object);
        //sharepoint
        Object.SharepointTrustDocLinks.Returns(SharepointTrustDocLinks.Object);
        //tad
        Object.TadHeadTeacherContacts.Returns(TadHeadTeacherContacts.Object);
        Object.TadTrustGovernances.Returns(TadTrustGovernances.Object);
        // Pupil Census
        Object.EdperfFiats.Returns(EdperfFiats.Object);

        //Set up some unused data to ensure we are actually retrieving the right data in our tests
        var otherTrust = AddGiasGroupForTrust(name: "Some other trust");
        for (var i = 0; i < 15; i++)
        {
            //Completely unused
            AddGiasGroupForTrust(name: $"Unused Trust {i}");
            AddGiasGroupForFederation(name: $"Unused Federation{i}");
            AddMstrTrust(region: $"S{i}shire");
            AddGiasEstablishment(establishmentName: $"Unused academy {i}");
            TadHeadTeacherContacts.Add(new TadHeadTeacherContact
            {
                Urn = i, HeadFirstName = $"{i}ver", HeadLastName = $"{i}verson",
                HeadEmail = $"{i}ver.{i}verson@school.com"
            });

            //Entities linked to some other trust
            var otherAcademy = AddGiasEstablishment(establishmentName: $"Some other academy {i}");
            AddGiasGroupLinks(otherTrust, otherAcademy);
            GiasGovernances.Add(new GiasGovernance
                { Gid = $"{i}", Uid = otherTrust.GroupUid!, Forename1 = $"Governor {i}" });
            TadTrustGovernances.Add(new TadTrustGovernance { Gid = $"{i}", Email = $"governor{i}@othertrust.com" });
            AddMstrFreeSchoolProject(otherTrust.GroupId!);
            
            EdperfFiats.Add(new EdperfFiat{ Urn = i, DownloadYear = "1999-2000" });
        }
    }

    public MstrTrust AddMstrTrust(string? groupUid = null, string? region = "North East")
    {
        var mstrTrust = new MstrTrust
        {
            GroupUid = groupUid ?? (MstrTrusts.Count + 1).ToString(),
            GORregion = region
        };
        MstrTrusts.Add(mstrTrust);
        return mstrTrust;
    }

    public void AddGiasGroupLinks(GiasGroup giasGroup, params GiasEstablishment[] giasEstablishments)
    {
        foreach (var giasEstablishment in giasEstablishments)
        {
            GiasGroupLinks.Add(new GiasGroupLink
            {
                GroupUid = giasGroup.GroupUid,
                Urn = giasEstablishment.Urn.ToString(),
                GroupType = giasGroup.GroupType,
                GroupId = giasGroup.GroupId,
                GroupStatusCode = giasGroup.GroupStatusCode,
                JoinedDate = "01/01/2022"
            });
        }
    }

    public GiasGroupLink[] AddGiasGroupLinks(string uid, int count)
    {
        var offset = int.Parse(GiasGroupLinks.GetNextId(g => g.Urn!));

        var urns = Enumerable.Range(0, count).Select(n => $"{n + offset}").ToArray();

        return AddGiasGroupLinks(uid, urns);
    }

    public GiasGroupLink[] AddGiasGroupLinks(string uid, params string[] urns)
    {
        var giasGroupLinks = urns.Select(urn => new GiasGroupLink
        {
            GroupUid = uid,
            Urn = urn,
            EstablishmentName = $"Academy {urn}",
            JoinedDate = "13/06/2023",
            GroupStatusCode = "OPEN"
        }).ToArray();

        GiasGroupLinks.AddRange(giasGroupLinks);

        return giasGroupLinks;
    }

    public void AddEstablishmentFiat(int urn, string? inspectionStartDate = null)
    {
        MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = urn,
            InspectionStartDate = inspectionStartDate
        });
    }

    public GiasEstablishment AddGiasEstablishment(int? urn = null, string? establishmentName = null, string? establishmentType = null)
    {
        var giasEstablishment = new GiasEstablishment
        {
            Urn = urn ?? GiasEstablishments.GetNextId(e => e.Urn),
            EstablishmentName = establishmentName ?? $"Academy {GiasEstablishments.Count + 1}",
            EstablishmentTypeGroupName = establishmentType,
            EstablishmentStatusName = "Open"
        };
        GiasEstablishments.Add(giasEstablishment);

        return giasEstablishment;
    }

    public void AddApplicationEvent(string description,
        DateTime? dateTime,
        string? message = "Finished", char? eventType = 'I')
    {
        ApplicationEvents.Add(new ApplicationEvent
        {
            Id = ApplicationEvents.GetNextId(e => e.Id),
            DateTime = dateTime,
            Source = "source",
            UserName = "Test User",
            EventType = eventType,
            Level = 1,
            Code = 1,
            Severity = 'S',
            Description = description,
            Message = message,
            Trace = "test trace",
            ProcessID = 1,
            LineNumber = 2
        });
    }

    public void AddApplicationSetting(string key, DateTime? modified)
    {
        ApplicationSettings.Add(new ApplicationSetting
        {
            Key = key,
            Modified = modified
        });
    }

    public GiasGroup AddGiasGroupForFederation(string? uid = null, string? name = null, bool open = true)
    {
        var nextGroupUid = uid ?? GiasGroups.GetNextId(g => g.GroupUid!);

        var giasGroup = new GiasGroup
        {
            GroupName = name ?? $"Federation {nextGroupUid}",
            GroupUid = nextGroupUid,
            GroupType = "Federation",
            GroupStatusCode = open ? "OPEN" : "CLOSED"
        };
        GiasGroups.Add(giasGroup);

        return giasGroup;
    }

    public GiasGroup AddGiasGroupForTrust(string? uid = null, string? name = null, string? trustReferenceNumber = null,
        string? groupType = null, bool open = true)
    {
        var nextGroupUid = uid ?? GiasGroups.GetNextId(g => g.GroupUid!);

        var giasGroup = new GiasGroup
        {
            GroupId = trustReferenceNumber ?? $"TR0{nextGroupUid}",
            GroupName = name ?? $"Trust {nextGroupUid}",
            GroupUid = nextGroupUid,
            GroupType = groupType ?? "Multi-academy trust",
            GroupStatusCode = open ? "OPEN" : "CLOSED"
        };
        GiasGroups.Add(giasGroup);

        return giasGroup;
    }

    public void AddMstrFreeSchoolProject(string trustId,
        string projectApplicationType = "Presumption",
        string stage = PipelineStatuses.FreeSchoolPipeline,
        string? projectName = null,
        int? statutoryLowestAge = null,
        int? statutoryHighestAge = null,
        int? newUrn = null,
        string? localAuthority = null,
        DateTime? provisionalOpeningDate = null,
        DateTime? lastDataRefresh = null)
    {
        MstrFreeSchoolProjects.Add(new MstrFreeSchoolProject
        {
            SK = MstrFreeSchoolProjects.GetNextId(e => e.SK),
            TrustID = trustId,
            Stage = stage,
            RouteOfProject = "Free School",
            ProjectApplicationType = projectApplicationType,
            ProjectName = projectName,
            StatutoryLowestAge = statutoryLowestAge,
            StatutoryHighestAge = statutoryHighestAge,
            NewURN = newUrn,
            LocalAuthority = localAuthority,
            ProvisionalOpeningDate = provisionalOpeningDate,
            LastDataRefresh = lastDataRefresh
        });
    }

    public void AddMstrAcademyConversion(string trustId, AdvisoryType advisoryType, string projectStatus,
        string projectName, string? daoProgress = null)
    {
        AddMstrAcademyConversion(trustId, projectStatus,
            advisoryType == AdvisoryType.PreAdvisory,
            advisoryType == AdvisoryType.PostAdvisory,
            projectName: projectName, daoProgress: daoProgress);
    }

    public void AddMstrAcademyConversion(
        string trustId,
        string projectStatus,
        bool? inPrepare,
        bool? inComplete,
        string routeOfProject = "Converter",
        string? projectName = null,
        int? urn = null,
        int? statutoryLowestAge = null,
        int? statutoryHighestAge = null,
        DateTime? expectedOpeningDate = null,
        string? daoProgress = null)
    {
        MstrAcademyConversions.Add(new MstrAcademyConversion
        {
            SK = MstrAcademyConversions.GetNextId(e => e.SK),
            TrustID = trustId,
            ProjectStatus = projectStatus,
            InPrepare = inPrepare,
            InComplete = inComplete,
            RouteOfProject = routeOfProject,
            ProjectName = projectName,
            URN = urn,
            StatutoryLowestAge = statutoryLowestAge,
            StatutoryHighestAge = statutoryHighestAge,
            ExpectedOpeningDate = expectedOpeningDate,
            DaoProgress = daoProgress
        });
    }

    public void AddMstrAcademyTransfer(string newProvisionalTrustId,
        string academyTransferStatus,
        bool? inPrepare,
        bool? inComplete,
        string? academyName = null,
        int? academyUrn = null,
        int? statutoryLowestAge = null,
        int? statutoryHighestAge = null,
        string? localAuthority = null,
        DateTime? expectedTransferDate = null,
        DateTime? lastDataRefresh = null)
    {
        MstrAcademyTransfers.Add(new MstrAcademyTransfer
        {
            SK = MstrAcademyTransfers.GetNextId(e => e.SK),
            NewProvisionalTrustID = newProvisionalTrustId,
            AcademyTransferStatus = academyTransferStatus,
            InPrepare = inPrepare,
            InComplete = inComplete,
            AcademyName = academyName,
            AcademyURN = academyUrn,
            StatutoryLowestAge = statutoryLowestAge,
            StatutoryHighestAge = statutoryHighestAge,
            LocalAuthority = localAuthority,
            ExpectedTransferDate = expectedTransferDate,
            LastDataRefresh = lastDataRefresh
        });
    }

    public SharepointTrustDocLink[] AddTrustDocLinks(string trustReferenceNumber, string folderPrefix, int number)
    {
        var sharepointTrustDocLinks = Enumerable.Range(1, number).Select(i =>
            new SharepointTrustDocLink
            {
                FolderPrefix = folderPrefix,
                TrustRefNumber = trustReferenceNumber,
                DocumentFilename = $"Trust Document {i}",
                DocumentLink = $"www.trustDocumentLink{i}.com",
                FolderYear = 2000 + i
            }).ToArray();

        SharepointTrustDocLinks.AddRange(sharepointTrustDocLinks);
        return sharepointTrustDocLinks;
    }
}
