using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Quartz;
using QueryX;
using Serilog;
using SimpleAuth.Application;
using SimpleAuth.Application.Common.Behaviors;
using SimpleAuth.Application.Server;
using SimpleAuth.Application.Settings.Queries;
using SimpleAuth.Domain.Model;
using SimpleAuth.Infrastructure.DataAccess.EF;
using System.Security.Cryptography.X509Certificates;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SimpleAuth.Infrastructure
{
    public static class DependenciesConfigurator
    {
        public static WebApplicationBuilder RegisterDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ServerSettingsOption>(
                builder.Configuration.GetSection(ServerSettingsOption.ServerSettings));

            var serverSettings = builder.Configuration.GetSection(ServerSettingsOption.ServerSettings).Get<ServerSettingsOption>();

            builder.Services.AddDbContext<SimpleAuthContext>(options =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("SimpleAuth"),
                    o => o.MigrationsHistoryTable("__EFMigrationsHistory", SimpleAuthContext.DbSchema));

                options.UseOpenIddict();
            });

            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<SimpleAuthContext>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
            });

            // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
            // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
            builder.Services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            builder.Services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<SimpleAuthContext>()
                       .ReplaceDefaultEntities<int>();

                // Enable Quartz.NET integration.
                options.UseQuartz();
            })

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the authorization, logout, token and userinfo endpoints.
                options.SetIssuer(new Uri(serverSettings.ServerUrl))
                       .SetAuthorizationEndpointUris("connect/authorize")
                       .SetLogoutEndpointUris("connect/logout")
                       .SetTokenEndpointUris("connect/token")
                       .SetIntrospectionEndpointUris("/connect/introspect");

                // Mark the "email", "profile" and "roles" scopes as supported scopes.
                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, Scopes.OfflineAccess);

                // Note: this sample only uses the authorization code flow but you can enable
                // the other flows if you need to support implicit, password or client credentials.
                options
                    .AllowRefreshTokenFlow()
                    .AllowAuthorizationCodeFlow()
                    .RequireProofKeyForCodeExchange();

                // Register the signing and encryption credentials.
                if (builder.Environment.IsDevelopment())
                {
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();
                }
                else
                {
                    var encryptionCertificate = new X509Certificate2(
                            serverSettings.EncryptionCertificatePath, serverSettings.EncryptionCertificatePassword);
                    var signinCertificate = new X509Certificate2(
                            serverSettings.SigninCertificatePath, serverSettings.SigninCertificatePassword);

                    options.AddEncryptionCertificate(encryptionCertificate);
                    options.AddSigningCertificate(signinCertificate);
                }

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserinfoEndpointPassthrough()
                       .EnableStatusCodePagesIntegration()
                       .DisableTransportSecurityRequirement();
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

            builder.Services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining(typeof(ValidationBehavior<,>));

            builder.Services.AddMediatR(typeof(SimpleAuthContext)/*, typeof(SimpleAuthCommand)*/);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            builder.Services.AddQueryX();

            builder.Services.AddMailKit(o =>
                o.UseMailKit(builder.Configuration.GetSection("EmailSettings").Get<MailKitOptions>(), ServiceLifetime.Singleton));

            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            builder.Logging.ClearProviders();
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            builder.Logging.AddSerilog(logger);

            builder.Services.AddHostedService<InitData>();

            builder.Services.Configure<AuthProvidersOption>(
                builder.Configuration.GetSection(AuthProvidersOption.AuthProviders));

            var authProviders = builder.Configuration.GetSection(AuthProvidersOption.AuthProviders).Get<AuthProvidersOption>();

            if (!string.IsNullOrEmpty(authProviders?.Google?.ClientId) && !string.IsNullOrEmpty(authProviders?.Google?.ClientSecret))
            {
                builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = authProviders.Google.ClientId;
                    options.ClientSecret = authProviders.Google.ClientSecret;
                });
            }

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "es" };
                options
                    .SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });

            return builder;
        }
    }
}
