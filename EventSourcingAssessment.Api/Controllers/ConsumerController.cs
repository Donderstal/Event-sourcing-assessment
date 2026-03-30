using EventSourcingAssessment.Domain.Commands;
using EventSourcingAssessment.Domain.Interfaces;
using EventSourcingAssessment.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingAssessment.Controllers;

[Route("api/[controller]")]
public class ConsumerController(IEntityRepository<Consumer> consumerRepository) : Controller
{
    [HttpGet]
    public async Task<ActionResult<List<Consumer>>> GetAll() => await consumerRepository.GetAllAsync(c => c.Address);

    [HttpGet("{id}")]
    public async Task<ActionResult<Consumer>> GetSingle(Guid id)
    {
        var consumer = await consumerRepository.GetByIdAsync(id, c => c.Address);
        return consumer != null ? Ok(consumer) : NotFound();
    }
    
    [HttpPost("createConsumer")]
    public async Task<ActionResult<Consumer>> CreateConsumer(CreateConsumer createConsumerCommand)
    {
        var consumerId = Guid.NewGuid();
        var consumer = new Consumer{
            Id = consumerId,
            FirstName = createConsumerCommand.FirstName,
            LastName = createConsumerCommand.LastName,
            Address = new Address()
            {
                Id = Guid.NewGuid(),
                ConsumerId = consumerId,
                Street = createConsumerCommand.Address.Street,
                PostalCode = createConsumerCommand.Address.PostalCode,
                HouseNumber = createConsumerCommand.Address.HouseNumber
            }
        };
        
        await consumerRepository.AddAsync(consumer);
        return CreatedAtAction(nameof(GetSingle), new { id = consumer.Id }, consumer);
    }
    
    [HttpPut("updateConsumer")]
    public async Task<ActionResult<Consumer>> UpdateConsumer(UpdateConsumer updateConsumerCommand)
    {
        var consumer = await consumerRepository.GetByIdAsync(updateConsumerCommand.Id, c => c.Address);
        if (consumer == null) return NotFound();
        
        consumer.FirstName = updateConsumerCommand.FirstName ?? consumer.FirstName;
        consumer.LastName = updateConsumerCommand.LastName ?? consumer.LastName;
        
        await consumerRepository.UpdateAsync(consumer);
        return Ok(consumer);
    }

    [HttpPut("updateConsumerAddress")]
    public async Task<ActionResult<Consumer>> UpdateConsumerAddress(UpdateConsumerAddress updateConsumerAddressCommand)
    {
        var consumer = await consumerRepository.GetByIdAsync(updateConsumerAddressCommand.ConsumerId, c => c.Address);
        if (consumer == null) return NotFound();

        consumer.Address.Street = updateConsumerAddressCommand.Address.Street;
        consumer.Address.HouseNumber = updateConsumerAddressCommand.Address.HouseNumber;
        consumer.Address.PostalCode = updateConsumerAddressCommand.Address.PostalCode;
        
        await consumerRepository.UpdateAsync(consumer);
        return Ok(consumer);
    }
}