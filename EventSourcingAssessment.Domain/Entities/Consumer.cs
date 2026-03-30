using System.ComponentModel.DataAnnotations.Schema;

namespace EventSourcingAssessment.Domain.Entities;

public class Consumer {

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; init; }
    
    public required Address Address { get; init; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
}