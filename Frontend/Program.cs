using ForkFrontend;
using ForkFrontend.Logic.Services.Connections;
using ForkFrontend.Logic.Services.HttpsClients;
using ForkFrontend.Logic.Services.Managers;
using ForkFrontend.Logic.Services.Notifications;
using ForkFrontend.Logic.Services.Translation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.SetMinimumLevel(LogLevel.Debug);
//builder.Logging.AddFork(options => { });

builder.Services.AddBlazorContextMenu();

//TODO CKE replace with customizable backend port
builder.Services.AddHttpClient<BackendClient>(client => client.BaseAddress = new Uri("http://localhost:35565"));
builder.Services.AddHttpClient<LocalClient>(client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
// All services are singletons so it's easier to use them and the whole Blazor App is one scope anyway
builder.Services.AddSingleton<ApplicationConnectionService>();
builder.Services.AddSingleton<EntityConnectionService>();
builder.Services.AddSingleton<CreateEntityConnectionService>();
builder.Services.AddSingleton<TranslationService>();
builder.Services.AddSingleton<NotificationService>();

builder.Services.AddSingleton<ApplicationStateManager>();
builder.Services.AddSingleton<ToastManager>();


await builder.Build().RunAsync();