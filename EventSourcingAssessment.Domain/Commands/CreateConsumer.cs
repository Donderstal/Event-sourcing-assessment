using EventSourcingAssessment.Domain.Models;

namespace EventSourcingAssessment.Domain.Commands;

public record CreateConsumer(AddressDto Address, string? FirstName, string? LastName);