using Application.ApplicationSettingSections;
using Application.IServices;
using DataAccess.Services;
using RareCrewCSharpTest;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.Configure<ExternalURLs>(builder.Configuration.GetSection(nameof(ExternalURLs)));
builder.Services.Configure<BaseUrlSection>(builder.Configuration.GetSection(nameof(BaseUrlSection)));
builder.Services.AddTransient<ITimeEntryService, TimeEntryService>();
builder.Services.AddTransient<IChartService, ChartService>();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddHttpClient("Default", client =>
{
    client.BaseAddress = new Uri("https://localhost:7020/"); 
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();

app.UseHttpsRedirection();

app.UseStaticFiles(); // serve wwwroot static assets

app.MapControllers();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
