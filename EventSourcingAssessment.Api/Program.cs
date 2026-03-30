using EventSourcingAssessment.Domain.Commands;
using EventSourcingAssessment.Handlers;
using EventSourcingAssessment.Persistence;
using EventSourcingAssessment.Projections;
using EventSourcingAssessment.Projectors;
using EventSourcingAssessment.Projectors.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistenceServices();
builder.Services.AddHostedService<ConsumerProjection>();
builder.Services.AddScoped<IConsumerEventProjector, ConsumerEventProjector>();

// Command handlers
builder.Services.AddScoped<ICommandHandler<CreateConsumer>, CreateConsumerCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateConsumer>, UpdateConsumerCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateConsumerAddress>, UpdateConsumerAddressCommandHandler>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }