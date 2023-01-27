using System.Text.Json.Serialization;
using System.Text.Json;
using SimpleAuth.Infrastructure;
using SimpleAuth.Server.ExceptionHandling;
using Microsoft.AspNetCore.HttpOverrides;
using SimpleAuth.Application.Server;
using Microsoft.Extensions.Options;
using System.Reflection;
using SimpleAuth.Server.Resources.Localizers;
using Microsoft.OpenApi.Models;
using SimpleAuth.Server.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddLocalization();

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Account/Manage");
})
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            return factory.Create(typeof(ModelValidationResource));
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddControllersAsServices();

builder.Services.AddSingleton<SharedResourceLocalizer>();
builder.Services.AddSingleton<EmailResourceLocalizer>();

builder.RegisterDependencies();

builder.Services.AddCors();

var serverSettings = builder.Configuration.GetSection(ServerSettingsOption.ServerSettings).Get<ServerSettingsOption>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DescribeAllParametersInCamelCase();

    options.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OpenIdConnect,
        OpenIdConnectUrl = new Uri($"{serverSettings.ServerUrl}/.well-known/openid-configuration")
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Authorization"
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    var serverUrl = new Uri(serverSettings.ServerUrl);
    var httpsHostString = new HostString(serverUrl.Host, serverUrl.Port);
    context.Request.Host = httpsHostString;
    await next.Invoke();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("swagger-ui");
        options.OAuthScopes("openid", "profile");
        options.OAuthUsePkce();
    });
}

//app.UseExceptionHandler("/Error");
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Replace with registered clients?
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();
app.MapDefaultControllerRoute();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.Run();
