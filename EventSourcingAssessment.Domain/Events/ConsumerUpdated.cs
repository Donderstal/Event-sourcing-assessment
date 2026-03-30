namespace EventSourcingAssessment.Domain.Events;

public record ConsumerUpdated(Guid Id, string FirstName, string LastName);