using System.ComponentModel.DataAnnotations.Schema;

namespace EventSourcingAssessment.Domain.Models;

public class Consumer {

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    
    public required Address Address { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
}