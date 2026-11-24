using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using SuiviLivraisons.Api;
using SuiviLivraisons.Components;
using SuiviLivraisons.Data;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-CA");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fr-CA");

var builder = WebApplication.CreateBuilder(args);

// Rendu hybride : pages en rendu statique côté serveur (léger, sans connexion permanente)
// + interactivité WebAssembly uniquement là où elle est utile (écran de réception).
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("SuiviLivraisons"));

var app = builder.Build();

// Prototype : création de la base et données de démonstration au démarrage.
// En production, on utiliserait plutôt des migrations EF Core.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Initialiser(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseAntiforgery();
app.MapStaticAssets();

app.MapLivraisonsApi();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SuiviLivraisons.Client._Imports).Assembly);

await app.RunAsync();
