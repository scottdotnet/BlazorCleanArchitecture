using BlazorCleanArchitecture.WebUI.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

ConfigureServices(builder.Services, builder.Configuration, builder.HostEnvironment);

await builder.Build().RunAsync();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebAssemblyHostEnvironment environment)
    => services.AddWebClientServices(configuration, environment);