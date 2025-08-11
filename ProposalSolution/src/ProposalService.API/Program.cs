using ProposalService.Application.Commands;
using ProposalService.Application.Ports;
using ProposalService.Infrastructure;
using ProposalService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Configurações padrões
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProposalCommand).Assembly));

// Registra RabbitMQPublisher como singleton (concreto e interface)
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddSingleton<IEventPublisher>(sp => sp.GetRequiredService<RabbitMQPublisher>());

// Outras infraestruturas (DbContext, repositórios etc)
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
