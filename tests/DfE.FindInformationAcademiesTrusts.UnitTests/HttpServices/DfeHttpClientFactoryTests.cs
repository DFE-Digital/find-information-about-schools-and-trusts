using System.Diagnostics;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using DfE.FindInformationAcademiesTrusts.Http;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.HttpServices;

public class DfeHttpClientFactoryTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CreateClient_uses_current_trace_id_and_replaces_existing_header(bool useTempFactory)
    {
        using var activity = new Activity("Factory test").SetIdFormat(ActivityIdFormat.W3C).Start();
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-correlationId", "existing");
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient("test").Returns(client);

        var result = CreateClient(factory, useTempFactory);

        result.Should().BeSameAs(client);
        result.DefaultRequestHeaders.GetValues("x-correlationId").Should().ContainSingle()
            .Which.Should().Be(activity.TraceId.ToString());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CreateClient_without_activity_generates_a_guid(bool useTempFactory)
    {
        var previousActivity = Activity.Current;
        Activity.Current = null;
        try
        {
            using var client = new HttpClient();
            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient("test").Returns(client);

            var result = CreateClient(factory, useTempFactory);

            var header = result.DefaultRequestHeaders.GetValues("x-correlationId").Should().ContainSingle().Which;
            Guid.TryParse(header, out var correlationId).Should().BeTrue();
            correlationId.Should().NotBe(Guid.Empty);
        }
        finally
        {
            Activity.Current = previousActivity;
        }
    }

    private static HttpClient CreateClient(IHttpClientFactory factory, bool useTempFactory)
    {
        return useTempFactory
            ? new DfeHttpClientFactoryTemp(factory).CreateClient("test")
            : new DfeHttpClientFactory(factory).CreateClient("test");
    }
}
