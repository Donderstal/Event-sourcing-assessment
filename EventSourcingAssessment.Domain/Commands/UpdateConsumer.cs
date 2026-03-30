namespace EventSourcingAssessment.Domain.Commands;

public record UpdateConsumer(Guid Id, string? FirstName, string? LastName);