using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NEventStore;

namespace EventSourcingAssesment.IntegrationsTests;

public class CustomApiFactory() : WebApplicationFactory<Program>
{
    // public IStoreEvents EventStore { get; }// = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
    //
    // protected override void ConfigureWebHost(IWebHostBuilder builder)
    // {
    //     builder.UseEnvironment("Testing");
    //
    //     builder.ConfigureServices(services =>
    //     {
    //         services.RemoveAll<IStoreEvents>();
    //         services.AddSingleton<IStoreEvents>(EventStore);
    //     });
    // }
}