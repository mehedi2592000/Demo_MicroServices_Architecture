using api_Product.Consumer;
using api_Product.OpenTelematry;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter("MassTransit", LogLevel.Debug);
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "validIssuer", // Same as used in Account project
            ValidateAudience = true,
            ValidAudience = "validAudience", // Same as used in Account project
            ValidateLifetime = true, // Ensure token hasn't expired
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

//builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
//{
//    tracerProviderBuilder
//        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ProductApi"))
//        .AddAspNetCoreInstrumentation()
//        .AddEntityFrameworkCoreInstrumentation()
//        .AddHttpClientInstrumentation()
//        .AddOtlpExporter(options =>
//        {
//            options.Endpoint = new Uri("http://localhost:4317"); // Replace with OTLP endpoint
//        });
//});

//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracerProviderBuilder =>
//    {
//        tracerProviderBuilder
//            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ProductApi"))
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            //.AddSqlClientInstrumentation()
//            .AddOtlpExporter(options =>
//            {
//                options.Endpoint = new Uri("http://localhost:18888"); // OTLP endpoint
//            });
//    });


//Uri openTelemetryUri = new Uri("http://localhost:18888");
//var openTelemetryConfig = !string.IsNullOrEmpty(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
//if (openTelemetryConfig)
//{
//    openTelemetryUri = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
//}

//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(res => res
//        .AddService(DiagnosticsConfig.ServiceName))
//    .WithMetrics(metrics =>
//    {
//        metrics
//            .AddHttpClientInstrumentation()
//            .AddAspNetCoreInstrumentation();
//            //.AddRuntimeInstrumentation();

//        metrics.AddMeter(DiagnosticsConfig.Meter.Name);

//        metrics.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
//    })
//    .WithTracing(tracing =>
//    {

//        tracing
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation();
//            //.AddEntityFrameworkCoreInstrumentation();

//        tracing.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);

//    }
//    );

//builder.Logging.AddOpenTelemetry(log =>
//{
//    log.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
//    log.IncludeScopes = true;
//    log.IncludeFormattedMessage = true;
//});

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
    //.ConfigureResource(resource => resource.AddService("CoffeeShopHasan"))
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation();
        //metrics.AddRuntimeInstrumentation()
        //   .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "System.Net.Http");
       
    })
    .WithTracing(tracing =>
    {
        tracing
           .AddAspNetCoreInstrumentation()
           .AddHttpClientInstrumentation();       
           
    });

var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.MapGet("/products", () => new[] { "Product 1", "Product 2", "Product 3" })
//    .RequireAuthorization();

app.Run();
