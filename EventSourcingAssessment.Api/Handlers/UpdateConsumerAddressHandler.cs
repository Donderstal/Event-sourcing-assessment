using EventSourcingAssessment.Domain.Commands;
using EventSourcingAssessment.Domain.Events;
using NEventStore;

namespace EventSourcingAssessment.Handlers;

public class UpdateConsumerAddressCommandHandler(IStoreEvents storeEvents): ICommandHandler<UpdateConsumerAddress>
{
    public Task Handle(UpdateConsumerAddress command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Basic validation example

        if (string.IsNullOrWhiteSpace(command.ConsumerId.ToString()))
            throw new ArgumentException("ConsumerId is required.", nameof(command));

        if (string.IsNullOrWhiteSpace(command.Address.Street))
            throw new ArgumentException("Street is required.", nameof(command));
        
        if (string.IsNullOrWhiteSpace(command.Address.PostalCode))
            throw new ArgumentException("PostalCode is required.", nameof(command));
        
        if (string.IsNullOrWhiteSpace(command.Address.HouseNumber))
            throw new ArgumentException("HouseNumber is required.", nameof(command));

        // Build the domain event
        var domainEvent = new ConsumerAddressUpdated(
            command.ConsumerId,
            command.Address.Street,
            command.Address.PostalCode,
            command.Address.HouseNumber
        );

        // Persist the event
        using var stream = storeEvents.OpenStream(
            "consumer",
            command.ConsumerId.ToString(),
            0,
            int.MaxValue
        );

        stream.Add(new EventMessage { Body = domainEvent });
        stream.CommitChanges(Guid.NewGuid());

        return Task.CompletedTask;
    }
}