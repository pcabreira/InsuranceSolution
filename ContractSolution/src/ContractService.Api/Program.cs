using ContractService.Application.Commands;
using ContractService.Application.Consumers;
using ContractService.Application.Ports;
using ContractService.Infrastructure;
using ContractService.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "ContractService", Version = "v1" }));

// infrastructure (DbContext, repository)
builder.Services.AddInfrastructure(builder.Configuration);

// MediatR (registra handlers do assembly Application)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateContractCommand).Assembly));

// RabbitMQ consumer como HostedService
builder.Services.AddHostedService<ProposalApprovalConsumer>();

builder.Services.AddScoped<IContractRepository, ContractRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
