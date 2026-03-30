using EventSourcingAssessment.Domain.Models;

namespace EventSourcingAssessment.Domain.Events;

public class ConsumerCreated(Guid Id, string FirstName, string LastName, AddressDto Address);