using System.Net.Http.Json;
using EventSourcingAssessment.Domain.Commands;
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
        
        // Assert
        postResponse.EnsureSuccessStatusCode();
        getResponse.EnsureSuccessStatusCode();
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
        
        // Assert
        postResponse.EnsureSuccessStatusCode();
        putResponse.EnsureSuccessStatusCode();
        getResponse.EnsureSuccessStatusCode();
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
        
        // Assert
        postResponse.EnsureSuccessStatusCode();
        putResponse.EnsureSuccessStatusCode();
        getResponse.EnsureSuccessStatusCode();
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