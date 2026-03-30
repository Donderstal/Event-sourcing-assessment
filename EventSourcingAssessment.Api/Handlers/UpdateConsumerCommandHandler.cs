using EventSourcingAssessment.Domain.Commands;
using EventSourcingAssessment.Domain.Events;
using NEventStore;

namespace EventSourcingAssessment.Handlers;

public class UpdateConsumerCommandHandler(IStoreEvents storeEvents): ICommandHandler<UpdateConsumer>
{
    public Task Handle(UpdateConsumer command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Basic validation example

        if (string.IsNullOrWhiteSpace(command.FirstName))
            throw new ArgumentException("FirstName is required.", nameof(command));

        if (string.IsNullOrWhiteSpace(command.LastName))
            throw new ArgumentException("LastName is required.", nameof(command));

        // Build the domain event
        var domainEvent = new ConsumerUpdated(
            command.Id,
            command.FirstName,
            command.LastName
        );

        // Persist the event
        using var stream = storeEvents.OpenStream(
            "consumer",
            command.Id.ToString(),
            0,
            int.MaxValue
        );

        stream.Add(new EventMessage { Body = domainEvent });
        stream.CommitChanges(Guid.NewGuid());

        return Task.CompletedTask;
    }
}