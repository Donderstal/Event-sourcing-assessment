using EventSourcingAssessment.Domain.Models;

namespace EventSourcingAssessment.Domain.Commands;

public record UpdateConsumerAddress(Guid ConsumerId, AddressDto Address);