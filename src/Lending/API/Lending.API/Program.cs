using Lending.API;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddAppSettings(out var settings)
    .AddCustomSerilog()
    .AddCustomSwagger()
    .AddOpenTelemetry();

// Add services to the container.
builder.Services
    .AddServiceDependencies(settings)
    .AddEndpointsApiExplorer()
    .AddControllers();

if (builder.Environment.EnvironmentName == Environments.Development)
{
    builder.Services.AddSidekick(builder.Configuration);
}

var app = builder.Build();

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
app.MapActorsHandlers();

app.Run();
