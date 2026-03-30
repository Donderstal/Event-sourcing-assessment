using EventSourcingAssessment.Domain.Entities;
using EventSourcingAssessment.Domain.Events;
using EventSourcingAssessment.Domain.Interfaces;
using EventSourcingAssessment.Projectors.Interfaces;

namespace EventSourcingAssessment.Projectors;

public sealed class ConsumerEventProjector(IEntityRepository<Consumer> consumerRepository) : IConsumerEventProjector
{
    public async Task ProjectAsync(object @event, CancellationToken cancellationToken = default)
    {
        switch (@event)
        {
            case ConsumerCreated consumerCreated:
                await Handle(consumerCreated);
                break;

            case ConsumerUpdated consumerUpdated:
                await Handle(consumerUpdated);
                break;

            case ConsumerAddressUpdated consumerAddressUpdated:
                await Handle(consumerAddressUpdated);
                break;
            
            default:
                break;
        }
    }

    private async Task Handle(ConsumerCreated eventMessage)
    {
        var consumer = new Consumer
        {
            Id = eventMessage.Id,
            FirstName = eventMessage.FirstName,
            LastName = eventMessage.LastName,
            Address = new Address
            {
                Id = Guid.NewGuid(),
                ConsumerId = eventMessage.Id,
                Street = eventMessage.Address.Street,
                PostalCode = eventMessage.Address.PostalCode,
                HouseNumber = eventMessage.Address.HouseNumber
            }
        };
        
        await consumerRepository.AddAsync(consumer);
    }

    private async Task Handle(ConsumerUpdated eventMessage)
    {
        var consumer = await consumerRepository.GetByIdAsync(eventMessage.Id, c => c.Address);
        if (consumer == null) return;

        consumer.FirstName = eventMessage.FirstName;
        consumer.LastName = eventMessage.LastName;
        
        await consumerRepository.UpdateAsync(consumer);
    }

    private async Task Handle(ConsumerAddressUpdated eventMessage)
    {
        var consumer = await consumerRepository.GetByIdAsync(eventMessage.ConsumerId, c => c.Address);
        if (consumer == null) return;

        consumer.Address.Street = eventMessage.Street;
        consumer.Address.HouseNumber = eventMessage.HouseNumber;
        consumer.Address.PostalCode = eventMessage.PostalCode;
        
        await consumerRepository.UpdateAsync(consumer);
    }
}