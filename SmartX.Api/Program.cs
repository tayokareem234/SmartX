using SmartX.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<SmartXDataStore>();

builder.Services.AddSingleton<TelemetryProcessingService>();

builder.Services.AddSingleton<DeploymentValidationService>();

builder.Services.AddSingleton<EncryptedFileStorageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();