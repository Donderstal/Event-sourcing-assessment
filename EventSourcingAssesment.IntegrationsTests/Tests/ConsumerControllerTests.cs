using System.Net.Http.Json;
using EventSourcingAssessment.Domain.Commands;
using EventSourcingAssessment.Domain.Constants;
using EventSourcingAssessment.Domain.Events;
using EventSourcingAssessment.Domain.Models;

namespace EventSourcingAssesment.IntegrationsTests.Tests;

public class ConsumerControllerIntegrationTests
{
    private CustomApiFactory _factory;
    
    private readonly Guid _consumerId = Guid.NewGuid();
    private readonly string _consumerFirstName = "John";
    private readonly string  _consumerLastName = "Doe";
    private readonly string  _addressStreet = "Main Street";
    private readonly string  _addressPostalCode = "7468BC";
    private readonly string  _addressHouseNumber = "12345";
    
    [SetUp]
    public void SetUp()
    {
        _factory = new CustomApiFactory();
    }

    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
    }
    
    [Test]
    public async Task CreateConsumer_ShouldCreateConsumer()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createConsumerCommand = GetCreateConsumer();
        
        // Act
        var postResponse = await client.PostAsJsonAsync("/api/consumers", createConsumerCommand);
        var getResponse = await client.GetAsync($"/api/consumers/{_consumerId}");
        var stream = await _factory.EventStore.OpenStreamAsync(
            EventSourcingConstants.ConsumerStreamName, _consumerId.ToString(), 0, int.MaxValue
        );
        
        // Assert
        postResponse.EnsureSuccessStatusCode();
        getResponse.EnsureSuccessStatusCode();
        
        var firstEventInStream = stream.CommittedEvents.First();
        var consumerCreatedEvent = firstEventInStream.Body as ConsumerCreated;
        Assert.That(
            stream.CommittedEvents.Count == 1
            && consumerCreatedEvent?.Id == _consumerId
            && consumerCreatedEvent.FirstName == _consumerFirstName
            && consumerCreatedEvent.LastName == _consumerLastName
            && consumerCreatedEvent.Address.Street == _addressStreet
            && consumerCreatedEvent.Address.PostalCode == _addressPostalCode
            && consumerCreatedEvent.Address.HouseNumber == _addressHouseNumber);
        
        var consumer = await getResponse.Content.ReadFromJsonAsync<Consumer>();
        Assert.That(
            consumer != null
            && consumer.Id == _consumerId
            && consumer.FirstName == _consumerFirstName
            && consumer.LastName == _consumerLastName
            && consumer.Address.Street == _addressStreet
            && consumer.Address.PostalCode == _addressPostalCode
            && consumer.Address.HouseNumber == _addressHouseNumber
        );
    }
    
    [Test]
    public async Task UpdateConsumer_ShouldUpdateExistingConsumer()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createConsumerCommand = GetCreateConsumer();
        
        var newFirstName = "Jane";
        var newLastName = "Johnson";
        var updateConsumerCommand = new UpdateConsumer(_consumerId, newFirstName, newLastName);
        
        // Act
        var postResponse = await client.PostAsJsonAsync("/api/consumers", createConsumerCommand);
        var putResponse = await client.PutAsJsonAsync("/api/consumers", updateConsumerCommand);
        var getResponse = await client.GetAsync($"/api/consumers/{_consumerId}");
        var stream = await _factory.EventStore.OpenStreamAsync(
            EventSourcingConstants.ConsumerStreamName, _consumerId.ToString(), 0, int.MaxValue
        );

        // Assert
        postResponse.EnsureSuccessStatusCode();
        putResponse.EnsureSuccessStatusCode();
        getResponse.EnsureSuccessStatusCode();
        
        var lastEventInStream = stream.CommittedEvents.Last();
        var consumerUpdated = lastEventInStream.Body as ConsumerUpdated;
        Assert.That(
            stream.CommittedEvents.Count == 2
            && consumerUpdated?.Id == _consumerId
            && consumerUpdated.FirstName == _consumerFirstName
            && consumerUpdated.LastName == _consumerLastName);
        
        var consumer = await getResponse.Content.ReadFromJsonAsync<Consumer>();
        Assert.That(
            consumer != null
            && consumer.Id == _consumerId
            && consumer.FirstName == newFirstName
            && consumer.LastName == _consumerLastName
            && consumer.Address.Street == _addressStreet
            && consumer.Address.PostalCode == _addressPostalCode
            && consumer.Address.HouseNumber == _addressHouseNumber
        );
    }
    
    [Test]
    public async Task UpdateConsumerAddress_ShouldUpdateExistingConsumerAddress()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createConsumerCommand = GetCreateConsumer();
        
        var newStreet = "Main Street 2";
        var newPostalCode = "7468 ABCD";
        var newHouseNumber = "12345678";
        var newAddress = new AddressDto(newStreet, newPostalCode, newHouseNumber);
        var updateConsumerCommand = new UpdateConsumerAddress(_consumerId, newAddress);
        
        // Act
        var postResponse = await client.PostAsJsonAsync("/api/consumers", createConsumerCommand);
        var putResponse = await client.PutAsJsonAsync("/api/consumers", updateConsumerCommand);
        var getResponse = await client.GetAsync($"/api/consumers/{_consumerId}");
        var stream = await _factory.EventStore.OpenStreamAsync(
            EventSourcingConstants.ConsumerStreamName, _consumerId.ToString(), 0, int.MaxValue
        );

        
        // Assert
        postResponse.EnsureSuccessStatusCode();
        putResponse.EnsureSuccessStatusCode();
        getResponse.EnsureSuccessStatusCode();
        
        var lastEventInStream = stream.CommittedEvents.Last();
        var consumerAddressUpdated = lastEventInStream.Body as ConsumerAddressUpdated;
        Assert.That(
            stream.CommittedEvents.Count == 2
            && consumerAddressUpdated?.ConsumerId == _consumerId
            && consumerAddressUpdated.Street == newStreet
            && consumerAddressUpdated.PostalCode == newPostalCode
            && consumerAddressUpdated.HouseNumber == newHouseNumber);
        
        var consumer = await getResponse.Content.ReadFromJsonAsync<Consumer>();
        Assert.That(
            consumer != null
            && consumer.Address.Street == newStreet
            && consumer.Address.PostalCode == newPostalCode
            && consumer.Address.HouseNumber == newHouseNumber
        );
    }

    private CreateConsumer GetCreateConsumer()
    {
        return new (
            _consumerId,
            new(_addressStreet, _addressPostalCode, _addressHouseNumber),
            _consumerFirstName,
            _consumerLastName
        );
    }
}