using EventSourcingAssessment.Domain.Events;
using EventSourcingAssessment.Domain.Interfaces;
using EventSourcingAssessment.Domain.Models;
using NEventStore;
using NEventStore.PollingClient;

namespace EventSourcingAssessment.Projections;

public class ConsumerProjection(
    IStoreEvents storeEvents,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private PollingClient2? _client;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        _client = new PollingClient2(
            storeEvents.Advanced,
            commit =>
            {
                foreach (var eventMessage in commit.Events)
                {
                    var @event = eventMessage.Body;

                    switch (@event)
                    {
                        case ConsumerCreated consumerCreated:
                            Handle(consumerCreated).GetAwaiter().GetResult();
                            break;
                        case ConsumerUpdated consumerUpdated:
                            Handle(consumerUpdated).GetAwaiter().GetResult();
                            break;
                        case ConsumerAddressUpdated consumerAddressUpdated:
                            Handle(consumerAddressUpdated).GetAwaiter().GetResult();
                            break;
                        default:
                            break;
                    }
                }

                return PollingClient2.HandlingResult.MoveToNext;
            },
            waitInterval: 500);

        _client.StartFromBucket("consumer");

        return Task.CompletedTask;
    }

    private async Task Handle(ConsumerCreated eventMessage)
    {
        var consumer = new Consumer{
            Id = eventMessage.Id,
            FirstName = eventMessage.FirstName,
            LastName = eventMessage.LastName,
            Address = new Address()
            {
                Id = Guid.NewGuid(),
                ConsumerId = eventMessage.Id,
                Street = eventMessage.Address.Street,
                PostalCode = eventMessage.Address.PostalCode,
                HouseNumber = eventMessage.Address.HouseNumber
            }
        };
        
        var consumerRepository = scopeFactory.CreateScope().ServiceProvider.GetService<IEntityRepository<Consumer>>();
        await consumerRepository.AddAsync(consumer);
    }
    
    private async Task Handle(ConsumerUpdated eventMessage)
    {
        var consumerRepository = scopeFactory.CreateScope().ServiceProvider.GetService<IEntityRepository<Consumer>>();
        var consumer = await consumerRepository.GetByIdAsync(eventMessage.Id, c => c.Address);
        if (consumer == null) return;
        
        consumer.FirstName = eventMessage.FirstName ?? consumer.FirstName;
        consumer.LastName = eventMessage.LastName ?? consumer.LastName;
        
        await consumerRepository.UpdateAsync(consumer);
    }
    
    private async Task Handle(ConsumerAddressUpdated eventMessage)
    {
        var consumerRepository = scopeFactory.CreateScope().ServiceProvider.GetService<IEntityRepository<Consumer>>();
        var consumer = await consumerRepository.GetByIdAsync(eventMessage.ConsumerId, c => c.Address);
        if (consumer == null) return;

        consumer.Address.Street = eventMessage.Street;
        consumer.Address.HouseNumber = eventMessage.HouseNumber;
        consumer.Address.PostalCode = eventMessage.PostalCode;
        
        await consumerRepository.UpdateAsync(consumer);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if(_client is not null)
        {
            _client.Stop();
            _client.Dispose();
            _client = null;
        }

        return base.StopAsync(cancellationToken);
    }
}