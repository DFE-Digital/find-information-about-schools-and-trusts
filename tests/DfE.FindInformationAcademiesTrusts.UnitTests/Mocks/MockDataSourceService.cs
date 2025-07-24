using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;

public static class MockDataSourceService
{
    private static readonly DateTime StaticTime = new(2023, 11, 9, 0, 0, 0, DateTimeKind.Utc);
    private static readonly string UpdatedBy = "some.user@education.gov.uk";

    public static IDataSourceService CreateSubstitute()
    {
        var mockDataSourceService = Substitute.For<IDataSourceService>();

        mockDataSourceService
            .GetAsync(Arg.Any<Source>())
            .Returns(args =>
            {
                var source = (Source)args[0];
                return Task.FromResult(GetDummyDataSource(source));
            });

        mockDataSourceService
            .GetSchoolContactDataSourceAsync(Arg.Any<int>(), Arg.Any<SchoolContactRole>())
            .Returns(Fiat);

        mockDataSourceService
            .GetTrustContactDataSourceAsync(Arg.Any<int>(), Arg.Any<TrustContactRole>())
            .Returns(Fiat);

        return mockDataSourceService;
    }

    private static DataSourceServiceModel GetDummyDataSource(Source source)
    {
        return new DataSourceServiceModel(source, StaticTime, source switch
        {
            Source.Cdm => UpdateFrequency.Daily,
            Source.Complete => UpdateFrequency.Daily,
            Source.Gias => UpdateFrequency.Daily,
            Source.ManageFreeSchoolProjects => UpdateFrequency.Daily,
            Source.Mstr => UpdateFrequency.Daily,
            Source.Prepare => UpdateFrequency.Daily,
            Source.Mis => UpdateFrequency.Monthly,
            Source.ExploreEducationStatistics => UpdateFrequency.Annually,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        });
    }

    public static DataSourceServiceModel Complete { get; } = GetDummyDataSource(Source.Complete);
    public static DataSourceServiceModel Gias { get; } = GetDummyDataSource(Source.Gias);
    public static DataSourceServiceModel Prepare { get; } = GetDummyDataSource(Source.Prepare);

    public static DataSourceServiceModel Fiat { get; } = new(Source.FiatDb, StaticTime, null, UpdatedBy);

    public static DataSourceServiceModel ManageFreeSchool { get; } =
        GetDummyDataSource(Source.ManageFreeSchoolProjects);
}
