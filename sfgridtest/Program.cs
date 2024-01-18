using sfgridtest;
using sfgridtest.Components;
using Syncfusion.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzAyOTAzNkAzMjM0MmUzMDJlMzBBSlVpTzRDcWIveEZad2V2bnFiSmZuTzNZRUxwZkJ4eGdpRUtDV1dCRmpvPQ ==; MzAyOTAzN0AzMjM0MmUzMDJlMzBjZkpxZUZQYmtSdVB3by9pVm43N2ZxREpXdWM0M3VpeVFTQUJXY0ZaWnJJPQ ==; MzAyOTAzOEAzMjM0MmUzMDJlMzBrMGRnWjN5SldYU2duNnVocGcweHgzb0VEVGFDSDFRd2NSQTJIZ2xSTzBBPQ ==");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSingleton<PriceDistributor>();
builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHub<TestHub>("/TestHub");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
