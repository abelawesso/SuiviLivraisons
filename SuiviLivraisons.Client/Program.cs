using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-CA");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fr-CA");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
