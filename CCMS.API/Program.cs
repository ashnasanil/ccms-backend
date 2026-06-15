using CCMS.API.Extensions;
using CCMS.API.Middleware;
using CCMS.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using CCMS.Application.Interfaces;
using CCMS.Infrastructure.Services;
using CCMS.Infrastructure.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiServices(builder.Configuration);

// Batch Module DI
builder.Services.AddScoped<IBatchService, BatchService>();
builder.Services.AddHostedService<AccountValidationBackgroundService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CCMS API v1"));

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

var attachmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Attachments");
if (!Directory.Exists(attachmentsPath))
{
    Directory.CreateDirectory(attachmentsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(attachmentsPath),
    RequestPath = "/api/attachments"
});

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    services.SeedDatabase();
}
app.Run();

public partial class Program { }