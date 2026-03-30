using EventSourcingAssessment.Domain.Models;

namespace EventSourcingAssessment.Domain.Commands;

public record CreateConsumer(Guid Id, AddressDto Address, string? FirstName, string? LastName);