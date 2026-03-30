using System.Net.Http.Json;
using EventSourcingAssessment.Domain.Commands;
using EventSourcingAssessment.Domain.Models;

namespace EventSourcingAssesment.IntegrationsTests.Tests;

public class ConsumerControllerIntegrationTests
{
    private CustomApiFactory _factory;
    
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
        var consumerId = Guid.NewGuid();
        var client = _factory.CreateClient();
        var createConsumerCommand = GetCreateConsumer(consumerId);
        
        // Act
        var postResponse = await client.PostAsJsonAsync("/api/consumers", createConsumerCommand);
        var consumer = await WaitForConsumerAsync(
            client,
            consumerId,
            consumer => consumer.Id == consumerId
                        && consumer.FirstName == _consumerFirstName
                        && consumer.LastName == _consumerLastName
                        && consumer.Address.Street == _addressStreet
                        && consumer.Address.PostalCode == _addressPostalCode
                        && consumer.Address.HouseNumber == _addressHouseNumber
        );
        
        // Assert
        postResponse.EnsureSuccessStatusCode();
        Assert.That(consumer, Is.Not.Null);
    }
    
    [Test]
    public async Task UpdateConsumer_ShouldUpdateExistingConsumer()
    {
        // Arrange
        var consumerId = Guid.NewGuid();
        var client = _factory.CreateClient();
        var createConsumerCommand = GetCreateConsumer(consumerId);
        
        var newFirstName = "Jane";
        var newLastName = "Johnson";
        var updateConsumerCommand = new UpdateConsumer(consumerId, newFirstName, newLastName);
        
        // Act
        var postResponse = await client.PostAsJsonAsync("/api/consumers", createConsumerCommand);
        await Task.Delay(100);
        var putResponse = await client.PutAsJsonAsync("/api/consumers", updateConsumerCommand);
        
        var consumer = await WaitForConsumerAsync(
            client,
            consumerId,
            c => c.FirstName == newFirstName && c.LastName == newLastName
        );

        // Assert
        postResponse.EnsureSuccessStatusCode();
        putResponse.EnsureSuccessStatusCode();

        Assert.That(consumer, Is.Not.Null);
    }
    
    [Test]
    public async Task UpdateConsumerAddress_ShouldUpdateExistingConsumerAddress()
    {
        // Arrange
        var consumerId = Guid.NewGuid();
        var client = _factory.CreateClient();
        var createConsumerCommand = GetCreateConsumer(consumerId);
        
        var newStreet = "Main Street 2";
        var newPostalCode = "7468 ABCD";
        var newHouseNumber = "12345678";
        var newAddress = new AddressDto(newStreet, newPostalCode, newHouseNumber);
        var updateConsumerAddress = new UpdateConsumerAddress(consumerId, newAddress);
        
        // Act
        var postResponse = await client.PostAsJsonAsync("/api/consumers", createConsumerCommand);
        await Task.Delay(100);
        var putResponse = await client.PutAsJsonAsync("/api/consumers/address", updateConsumerAddress);
        
        var consumer = await WaitForConsumerAsync(
            client,
            consumerId,
            consumer => consumer.Address.Street == newStreet 
                        && consumer.Address.PostalCode == newPostalCode
                        && consumer.Address.HouseNumber == newHouseNumber
        );

        // Assert
        postResponse.EnsureSuccessStatusCode();
        putResponse.EnsureSuccessStatusCode();

        Assert.That(consumer, Is.Not.Null);
    }

    private CreateConsumer GetCreateConsumer(Guid id)
    {
        return new (
            id,
            new(_addressStreet, _addressPostalCode, _addressHouseNumber),
            _consumerFirstName,
            _consumerLastName
        );
    }
    
    static async Task<Consumer?> WaitForConsumerAsync(
        HttpClient client,
        Guid consumerId,
        Func<Consumer, bool> predicate,
        int retries = 30,
        int delayMs = 100)
    {
        for (var i = 0; i < retries; i++)
        {
            var response = await client.GetAsync($"/api/consumers/{consumerId}");
            if (response.IsSuccessStatusCode)
            {
                var consumer = await response.Content.ReadFromJsonAsync<Consumer>();
                if (consumer != null && predicate(consumer))
                    return consumer;
            }

            await Task.Delay(delayMs);
        }

        return null;
    }
}