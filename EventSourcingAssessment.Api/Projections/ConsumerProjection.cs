using EventSourcingAssessment.Projectors.Interfaces;
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
        _client = new PollingClient2(
            storeEvents.Advanced,
            commit =>
            {
                foreach (var eventMessage in commit.Events)
                {
                    var @event = eventMessage.Body;
                    using var serviceScope = scopeFactory.CreateScope();
                    var projector = serviceScope.ServiceProvider.GetService<IConsumerEventProjector>();
                    projector.ProjectAsync(@event).GetAwaiter().GetResult();
                }

                return PollingClient2.HandlingResult.MoveToNext;
            },
            waitInterval: 500);

        _client.StartFromBucket("consumer");

        return Task.CompletedTask;
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