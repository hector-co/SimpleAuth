using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using QueryX;
using Serilog;
using SimpleAuth.Application;
using SimpleAuth.Application.Behaviors;
using SimpleAuth.Domain.Model;
using SimpleAuth.Infrastructure.DataAccess.EF;

namespace SimpleAuth.Infrastructure
{
    public static class DependenciesConfigurator
    {
        public static WebApplicationBuilder RegisterDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<SimpleAuthContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("SimpleAuth"),
                    o => o.MigrationsHistoryTable("__EFMigrationsHistory", SimpleAuthContext.DbSchema)));

            builder.Services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<SimpleAuthContext>();

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

            return builder;
        }
    }
}
