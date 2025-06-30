using Application.ApplicationSettingSections;
using Application.IServices;
using DataAccess.Services;
using RareCrewCSharpTest;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.Configure<TimeEntriesSection>(builder.Configuration.GetSection(nameof(TimeEntriesSection)));
builder.Services.AddTransient<ITimeEntryService, TimeEntryService>();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddHttpClient("Default", client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/"); // Use your actual dev URL and port
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
