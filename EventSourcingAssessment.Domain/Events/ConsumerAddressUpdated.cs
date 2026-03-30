namespace EventSourcingAssessment.Domain.Events;

public record ConsumerAddressUpdated(Guid ConsumerId, string Street, string PostalCode, string HouseNumber);