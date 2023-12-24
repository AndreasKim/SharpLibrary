using FastEndpoints;
using Lending.API;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddAppSettings(out var settings)
    .AddCustomSerilog()
    .AddCustomSwagger()
    .AddOpenTelemetry();

// Add services to the container.
builder.Services
    .AddActorDictionary()
    .AddServiceDependencies(builder.Configuration, settings)
    .AddEndpointsApiExplorer()
    .AddControllers();

builder.Host.UseOrleans(silo =>
    {
        silo.UseLocalhostClustering()
            .ConfigureLogging(logging => logging.AddConsole());
        silo.AddAdoNetGrainStorage("libraryStore", options =>
        {
            options.Invariant = "System.Data.SqlClient";
            options.ConnectionString = settings.ConnectionStrings.GrainStorage;
        });
    })
    .UseConsoleLifetime();

var app = builder.Build();

app.UseFastEndpoints();
app.UseDefaultExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
}

var pathBase = builder.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseAuthorization();
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

app.Run();
