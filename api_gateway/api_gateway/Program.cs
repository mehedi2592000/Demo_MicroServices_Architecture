using api_gateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//builder.Services.AddReverseProxy()
//    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


var productServiceUrl = Environment.GetEnvironmentVariable("PRODUCT_SERVICE_URL");

if (string.IsNullOrEmpty(productServiceUrl))
{
    throw new InvalidOperationException("Environment variable PRODUCT_SERVICE_URL is not set.");
}

// Configure routes and clusters dynamically
var routes = new[]
{
    new RouteConfig
    {
        RouteId = "ProductRoute",
        ClusterId = "ProductCluster",
        Match = new RouteMatch { Path = "/api/Products/{**catch-all}" }
    }
};

var clusters = new[]
{
    new ClusterConfig
    {
        ClusterId = "ProductCluster",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            ["Destination1"] = new DestinationConfig
            {
                Address = productServiceUrl
            }
        }
    }
};

// Load the dynamic configuration into YARP
builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var proxyConfig = builder.Configuration.GetSection("ReverseProxy").Get<ReverseProxyConfig>();
if (proxyConfig?.Clusters != null)
{
    foreach (var cluster in proxyConfig.Clusters)
    {
        Console.WriteLine($"xxxx Cluster: {cluster.Key}, Destination Address: {string.Join(", ", cluster.Value.Destinations.Select(d => d.Value.Address))}");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy();

app.Run();
