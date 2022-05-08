using AuthServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.
    AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/auth/login";
    });

builder.Services
    .AddDbContext<DbContext>(options =>
    {
        options.UseInMemoryDatabase(nameof(DbContext));
        options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
        .UseDbContext<DbContext>();
    })
    .AddServer(options =>
    {
        options

        //choose applicable OIDC flows (grant types)
        .AllowClientCredentialsFlow()

        //Endpoints
        .SetTokenEndpointUris("/connect/token");

        // Encryption and signing of tokens
        options
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey();

        // Register scopes (permissions) - in addition to openid scope which is there already
        options.RegisterScopes("api");


        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough();

    });

//add a test client
builder.Services.
   AddHostedService<TestDataService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();
