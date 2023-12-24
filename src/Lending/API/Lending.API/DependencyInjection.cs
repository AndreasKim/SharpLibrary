// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Linq;
using Core.Application;
using Core.Application.Interfaces;
using FastEndpoints;
using FastEndpoints.Swagger;
using Lending.API.IntegrationEvents.Handlers;
using Lending.API.Orchestrator;
using Lending.Infrastructure;
using Man.Dapr.Sidekick;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Lending.API;

public static class DependencyInjection
{
    public static readonly string AppId = nameof(Lending).ToLower();
    public const string ServiceVersion = "1.0.0";

    public static IServiceCollection AddServiceDependencies(this IServiceCollection services, IConfiguration configuration, AppSettings settings)
    {
        services.AddDaprClient();
        services.AddDaprSidekick(configuration, p => p.Sidecar =
            new DaprSidecarOptions()
            {
                AppId = AppId,
                ComponentsDirectory = "..\\..\\..\\dapr\\components",
                ConfigFile = "..\\..\\..\\dapr"
            });

        services.AddFastEndpoints();
        services.AddScoped<Handler>();
        services.AddScoped<IEBus, EventBus>();
        services.AddScoped<IDomainEventHandler, BookActor>();
        services.AddScoped<IRepository>(p => new Repository(settings.ConnectionStrings.DefaultConnection));
 
        services.AddSingleton(new ActivitySource(AppId));

        return services;
    }    
    
    public static IServiceCollection AddActorDictionary(this IServiceCollection services)
    {
        var localActorDictionary = Assembly.GetExecutingAssembly().GetTypes()
            .Where(p => typeof(IDomainEventHandler).IsAssignableFrom(p) && p.IsInterface)
            .Distinct()
            .ToDictionary(GetGenericInterfaceArgument, p => p);

        services.AddScoped(p => new ActorDictionary(localActorDictionary));

        return services;
    }

    private static Type GetGenericInterfaceArgument(Type p)
    {
        return p.GetInterfaces().First(x => x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)).GetGenericArguments()[0];
    }

    public static WebApplicationBuilder AddAppSettings(this WebApplicationBuilder builder, out AppSettings settings)
    {
        settings = new AppSettings();
        builder.Configuration.Bind(settings);
        builder.Services.AddSingleton(settings);
        return builder;
    }

    public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var config = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationId", AppId);

        var seqServerUrl = builder.Configuration["SeqServerUrl"];
        if (!string.IsNullOrWhiteSpace(seqServerUrl))
        {
            config = config.WriteTo.Seq(seqServerUrl);
        }

        Log.Logger = config.CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var meter = new Meter(AppId);
        var counter = meter.CreateCounter<long>("app.request-counter");
        var appResourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: AppId, serviceVersion: ServiceVersion);

        var hasZipkinUrl = Uri.TryCreate(builder.Configuration["ZipkinServerUrl"], new UriCreationOptions(), out var zipkinServerUrl);

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddSource(AppId)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: AppId, serviceVersion: ServiceVersion))
                    .AddGrpcClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                if (hasZipkinUrl)
                {
                    tracerProviderBuilder
                        .AddZipkinExporter(p => p.Endpoint = zipkinServerUrl);
                }
            })
            .WithMetrics(metricProviderBuilder =>
            {
                metricProviderBuilder
                    .AddMeter(meter.Name)
                    .SetResourceBuilder(appResourceBuilder)
                    .AddAspNetCoreInstrumentation();
            }).StartWithHost();

        return builder;
    }

    public static WebApplicationBuilder AddCustomSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.SwaggerDocument(c =>
        {
            c.DocumentSettings = s =>
            {
                s.Title = $"SharpLibrary - {AppId}";
                s.Version = "v1";
            };
        });
        return builder;
    }
    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseOpenApi();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppId} V1");
        });
    }
}
