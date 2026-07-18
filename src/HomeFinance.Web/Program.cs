using HomeFinance.Core;
using HomeFinance.Data;
using HomeFinance.Web.Authentication;
using HomeFinance.Web.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices();
builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HomeFinanceDbContext>();
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<IUserSeeder>().SeedAsync(default);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapAuthenticationEndpoints();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
