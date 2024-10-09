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

// Konfigurera Kestrel f�r att anv�nda Https
builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5176); 
	options.ListenAnyIP(7176, listenOptions =>
	{
		listenOptions.UseHttps(); 
	});
});

/*********** H�r l�ggs alla tj�nster till ***********/

// Tj�nster f�r Razor-komponenter och specificera interaktiva renderingstillst�nd.
// Detta g�r det m�jligt att anv�nda b�de server-rendering och WebAssembly-rendering f�r komponenterna.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// L�gg till SignalR
builder.Services.AddSignalR();

// Tj�nster f�r autentisering och anv�ndarhantering
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


// Tj�nster och konfiguration f�r databasen. 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// tj�nster och konfiguration f�r att anv�nda Identity Framework
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

/*********** H�r l�ggs all middleware till ***********/

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

// Mappa ChatHuben som anv�nder SignalR
app.MapHub<ChatHub>("/chathub");

// Mappa Razor-komponenter och specificera renderingstillst�ndet.
// Till�ter auto-rendering och ger m�jlighet att styra renderingsl�get per komponent.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LeChat.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
