using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;

public class FindInformationAcademiesTrustContext(DbContextOptions<FindInformationAcademiesTrustContext> options, IConfiguration configuration, IMediator mediator, IUserContextService userContextService) : DbContext(options)
{
    public IConfiguration Configuration { get; } = configuration;
    public IMediator Mediator { get; } = mediator;
    public IUserContextService UserContextService { get; } = userContextService;
}
