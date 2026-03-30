using EventSourcingAssessment.Domain.Models;

namespace EventSourcingAssessment.Domain.Events;

public record ConsumerCreated(Guid Id, string FirstName, string LastName, AddressDto Address);