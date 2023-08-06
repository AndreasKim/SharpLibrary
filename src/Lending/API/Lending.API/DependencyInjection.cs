// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Lending.API.IntegrationEvents.Handlers;
using Lending.API.Orchestrator;
using Lending.Infrastructure;
using Man.Dapr.Sidekick;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using FluentValidation;

namespace Lending.API;

public static class DependencyInjection
{
    public static readonly string AppId = nameof(Lending).ToLower();
    public const string ServiceVersion = "1.0.0";

    public static IServiceCollection AddServiceDependencies(this IServiceCollection services, AppSettings settings)
    {
        services.AddDaprClient();
        services.AddActors(options =>
        {
            options.ReentrancyConfig.Enabled = true;
            options.ReentrancyConfig.MaxStackDepth = 10;
            options.Actors.RegisterActor<LendingProcessActor>();
        });

        services.AddScoped<Handler>();
        services.AddScoped<IRepository>(p => new Repository(settings.ConnectionStrings.DefaultConnection));

        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddSingleton(new ActivitySource(AppId));

        return services;
    }  
    
    public static IServiceCollection AddSidekick(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDaprSidekick(configuration, p => p.Sidecar =
                new DaprSidecarOptions()
                {
                    AppId = AppId,
                    ComponentsDirectory = "..\\..\\..\\..\\dapr\\components",
                    ConfigFile = "..\\..\\..\\..\\dapr"
                });

        return services;
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
        //builder.Services.SwaggerDocument(c =>
        //{
        //    c.DocumentSettings = s =>
        //    {
        //        s.Title = $"SharpLibrary - {AppId}";
        //        s.Version = "v1";
        //    };
        //});
        return builder;
    }
    public static void UseCustomSwagger(this WebApplication app)
    {
        //app.UseOpenApi();
        //app.UseSwaggerUI(c =>
        //{
        //    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppId} V1");
        //});
    }
}
