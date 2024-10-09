using LeChat.Client.Pages;
using LeChat.Components;
using LeChat.Components.Account;
using LeChat.Data;
using LeChat.Data.Services;
using LeChat.Hubs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Konfigurera Kestrel för att använda Https
builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5176); 
	options.ListenAnyIP(7176, listenOptions =>
	{
		listenOptions.UseHttps(); 
	});
});

/*********** Här läggs alla tjänster till ***********/

// Tjänster för Razor-komponenter och specificera interaktiva renderingstillstånd.
// Detta gör det möjligt att använda både server-rendering och WebAssembly-rendering för komponenterna.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Lägg till SignalR
builder.Services.AddSignalR();

// Tjänster för autentisering och användarhantering
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<ChatMessageService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();


// Tjänster och konfiguration för databasen. 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// tjänster och konfiguration för att använda Identity Framework
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

/*********** Här läggs all middleware till ***********/

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseHsts();
	app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseStaticFiles();
app.UseAntiforgery();

// Mappa ChatHuben som använder SignalR
app.MapHub<ChatHub>("/chathub");

// Mappa Razor-komponenter och specificera renderingstillståndet.
// Tillåter auto-rendering och ger möjlighet att styra renderingsläget per komponent.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LeChat.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
