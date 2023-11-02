using Demo;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => {

    var hc = new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    };

    return hc;

});

await builder.Build().RunAsync();
