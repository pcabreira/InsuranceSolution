using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProposalService.Application.Ports;
using ProposalService.Domain.Ports;
using ProposalService.Infrastructure.Data;
using ProposalService.Infrastructure.Messaging;
using ProposalService.Infrastructure.Repositories;

namespace ProposalService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProposalDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProposalRepository, ProposalRepository>();
        services.AddSingleton<IEventPublisher, RabbitMQPublisher>();
        return services;
    }
}
