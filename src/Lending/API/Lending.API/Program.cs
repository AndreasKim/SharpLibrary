using FastEndpoints;
using Lending.API;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddAppSettings(out var settings)
    .AddCustomSerilog()
    .AddCustomSwagger()
    .AddOpenTelemetry();

// Add services to the container.
builder.Services
    .AddServiceDependencies(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddControllers();

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
