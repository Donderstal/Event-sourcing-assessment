using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NEventStore;
using NEventStore.Serialization.Json;

namespace EventSourcingAssessment.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPersistenceServices()
        {
            services.AddEventStore();
            services.AddDatabase();
            return services;
        }

        private IServiceCollection AddEventStore()
        {
            var store = Wireup.Init()
                .UsingInMemoryPersistence()
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .Build();
        
            services.AddSingleton(_ => store);   
            return services;
        }

        private IServiceCollection AddDatabase()
        {
            services.AddDbContext<InMemoryDbContext>(options =>
                options.UseInMemoryDatabase("InMemoryDb"));
            return services;
        }
    }
}