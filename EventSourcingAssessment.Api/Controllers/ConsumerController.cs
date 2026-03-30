using EventSourcingAssessment.Domain.Commands;
using EventSourcingAssessment.Domain.Interfaces;
using EventSourcingAssessment.Domain.Models;
using EventSourcingAssessment.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingAssessment.Controllers;

[Route("api/[controller]")]
public class ConsumerController(
    IEntityRepository<Consumer> consumerRepository,
    ICommandHandler<CreateConsumer> createConsumerHandler,
    ICommandHandler<UpdateConsumer> updateConsumerHandler,
    ICommandHandler<UpdateConsumerAddress> updateConsumerAddressHandler) : Controller
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
    public async Task<ActionResult> CreateConsumer(CreateConsumer createConsumerCommand)
    {
        await createConsumerHandler.Handle(createConsumerCommand);
        return Ok();
    }
    
    [HttpPut("updateConsumer")]
    public async Task<ActionResult> UpdateConsumer(UpdateConsumer updateConsumerCommand)
    {
        await updateConsumerHandler.Handle(updateConsumerCommand);
        return Ok();
    }

    [HttpPut("updateConsumerAddress")]
    public async Task<ActionResult> UpdateConsumerAddress(UpdateConsumerAddress updateConsumerAddressCommand)
    {
        await updateConsumerAddressHandler.Handle(updateConsumerAddressCommand);
        return Ok();
    }
}