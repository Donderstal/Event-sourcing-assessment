using EventSourcingAssessment.Domain.Commands;
using EventSourcingAssessment.Domain.Entities;
using EventSourcingAssessment.Domain.Interfaces;
using EventSourcingAssessment.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingAssessment.Controllers;

[Route("api/[controller]")]
public class ConsumersController(
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
    
    [HttpPost]
    public async Task<ActionResult> CreateConsumer([FromBody] CreateConsumer createConsumerCommand)
    {
        await createConsumerHandler.Handle(createConsumerCommand);
        return Ok();
    }
    
    [HttpPut]
    public async Task<ActionResult> UpdateConsumer([FromBody] UpdateConsumer updateConsumerCommand)
    {
        await updateConsumerHandler.Handle(updateConsumerCommand);
        return Ok();
    }

    [HttpPut("address")]
    public async Task<ActionResult> UpdateConsumerAddress([FromBody] UpdateConsumerAddress updateConsumerAddressCommand)
    {
        await updateConsumerAddressHandler.Handle(updateConsumerAddressCommand);
        return Ok();
    }
}