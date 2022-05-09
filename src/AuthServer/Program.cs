using AuthServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


//SEE https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.
    AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/authentication/login";
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

        //client credentials flow
        .AllowClientCredentialsFlow()
        .AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange()
        .AllowRefreshTokenFlow()
        .AllowPasswordFlow()

        //Endpoints
        // for auth code flow - returns the authorization code once the user authorizes the application - this is exchanged for access token from token endpoint
        .SetAuthorizationEndpointUris("/connect/authorize")          
        // used to issue tokens for all flows
        .SetTokenEndpointUris("/connect/token")
        // used to retrieve extra info about user other than what is in the id or access tokens
        .SetUserinfoEndpointUris("/connect/userinfo");                     
         

        // Encryption and signing of tokens
        options
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey();

        // Register scopes (permissions) - in addition to openid scope which is there already
        options.RegisterScopes("api");


        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough();

        options
            .DisableAccessTokenEncryption();

    });

//add a test client
builder.Services.
   AddHostedService<TestDataService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization(); //used for the userinfo endpoint
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();
