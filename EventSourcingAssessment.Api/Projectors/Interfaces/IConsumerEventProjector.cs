namespace EventSourcingAssessment.Projectors.Interfaces;

public interface IConsumerEventProjector
{
    Task ProjectAsync(object @event, CancellationToken cancellationToken = default);
}
