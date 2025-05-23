using FizzBuzz.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => "Hello World!");

app.MapPost("/", (FizzBuzzRequest request) => {
    foreach (var fizzBuzzItem in request.Multiples)
    {
        if (fizzBuzzItem.Multiple <= 0)
        {
            return Results.Problem("Multiple must be greater than 0.", statusCode: 400);
        }

        if (string.IsNullOrWhiteSpace(fizzBuzzItem.WordToPrint))
        {
            return Results.Problem("Word is required.", statusCode: 400);
        }
    }

    var maxNumber = request.MaxNumber;

    if (maxNumber <= 0)
    {
        return Results.Problem("MaxNumber must be greater than 0.", statusCode: 400);
    }


    var result = new Dictionary<int, string>();

    // request might look like this
    // {"multiples": [{ "multiple": 3, "wordToPrint": "Fizz" }, { "multiple": 5, "wordToPrint": "Buzz" }], "maxNumber": 16 }
    // and the result should be like this
    // {"3": "Fizz", "5": "Buzz", "6": "Fizz", "9": "Fizz", "12": "Fizz", "15": "FizzBuzz" }

    var multiples = request.Multiples.ToDictionary(m => m.Multiple, m => m.WordToPrint);
    for (var i = 1; i <= maxNumber; i++)
    {
        var fizzBuzzWord = string.Empty;
        foreach (var (multiple, word) in multiples)
        {
            if (i % multiple == 0)
            {
                fizzBuzzWord += word;
            }
        }
        if (!string.IsNullOrEmpty(fizzBuzzWord))
        {
            result.Add(i, fizzBuzzWord);
        }
    }


    return Results.Ok(result);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();
