namespace EventSourcingAssessment.Domain.Entities;

public class Address {
    
    public Guid Id { get; init; }
    
    public required string Street { get; set; }
    
    public required string PostalCode { get; set; }
    
    public required string HouseNumber { get; set; }
    
    public Guid ConsumerId { get; init; }
}