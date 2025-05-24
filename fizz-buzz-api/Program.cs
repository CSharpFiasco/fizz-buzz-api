using FizzBuzz.CachePolicies;
using FizzBuzz.Models;
using FizzBuzz.Services;
using Microsoft.Extensions.Options;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFizzBuzzService, FizzBuzzService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("CachePost", PostCachePolicy.Instance);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://fizz-buzz.carlosmartos.com")
                                 .WithOrigins("http://localhost:4200")
                                 .AllowAnyMethod()
                                 .AllowAnyHeader()
                          ;
                      });
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// todo: setup probe for liveness and readiness
app.MapHealthChecks("/");

app.MapPost("/", (FizzBuzzRequest request, IFizzBuzzService service) => {
    return service.Process(request);
}).CacheOutput("CachePost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();
