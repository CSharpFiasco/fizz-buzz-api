using FizzBuzz.Models;
using System.Text;

namespace FizzBuzz.Services;

public interface IFizzBuzzService
{
    IResult Process(FizzBuzzRequest request);
}

public class FizzBuzzService : IFizzBuzzService
{
    private readonly ILogger<FizzBuzzService> _logger;
    public FizzBuzzService(ILogger<FizzBuzzService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <summary>
    /// Takes a FizzBuzzRequest and processes it to return a dictionary of numbers and their corresponding FizzBuzz words.
    /// </summary>
    /// <param name="request">
    /// request might look like this <br />
    /// {"multiples": [{ "multiple": 3, "wordToPrint": "Fizz" }, { "multiple": 5, "wordToPrint": "Buzz" }], "maxNumber": 16 } <br />
    /// and the result should be like this <br />
    /// {"3": "Fizz", "5": "Buzz", "6": "Fizz", "9": "Fizz", "12": "Fizz", "15": "FizzBuzz" } <br />
    /// </param>
    /// <returns></returns>
    public IResult Process(FizzBuzzRequest request)
    {
        // check for empty request
        if (request == null || request.Multiples.Length == 0)
        {
            _logger.LogWarning("Request is empty or null.");
            return Results.Problem("Request is empty.", statusCode: 400);
        }

        if (request.Multiples.Length > 100)
        {
            _logger.LogWarning("Too many multiples in request: {Count}", request.Multiples.Length);
            return Results.Problem("Too many multiples", statusCode: 400);
        }

        // check for duplicates
        var duplicates = request.Multiples.GroupBy(x => x.Multiple)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count != 0)
        {
            _logger.LogWarning("Duplicates found in request: {Duplicates}", string.Join(", ", duplicates));
            return Results.Problem($"Duplicates found. {string.Join(", ", duplicates)}", statusCode: 400);
        }

        // check for invalid multiples and words
        foreach (var fizzBuzzItem in request.Multiples)
        {
            if (fizzBuzzItem.Multiple <= 0)
            {
                _logger.LogWarning("Multiple must be greater than 0. {Multiple}", fizzBuzzItem.Multiple);
                return Results.Problem("Multiple must be greater than 0.", statusCode: 400);
            }

            if (string.IsNullOrWhiteSpace(fizzBuzzItem.WordToPrint))
            {
                _logger.LogWarning("WordToPrint is required");
                return Results.Problem("Word is required.", statusCode: 400);
            }

            if (fizzBuzzItem.WordToPrint.Length > 1024)
            {
                _logger.LogWarning("WordToPrint must be less than or equal to 1024 characters.");
                return Results.Problem("Word must be less than or equal to 1024 characters.", statusCode: 400);
            }
        }

        var maxNumber = request.MaxNumber;

        if (maxNumber <= 0)
        {
            return Results.Problem("MaxNumber must be greater than 0.", statusCode: 400);
        }

        var result = new Dictionary<int, string>();

        for (var i = 1; i <= maxNumber; i++)
        {
            var fizzBuzzWordBuilder = new StringBuilder();
            foreach (var item in request.Multiples)
            {
                if (i % item.Multiple == 0)
                {
                    fizzBuzzWordBuilder.Append(item.WordToPrint);
                }
            }
            if (fizzBuzzWordBuilder.Length > 0)
            {
                result.Add(i, fizzBuzzWordBuilder.ToString());
            }
        }


        return Results.Ok(result.AsReadOnly());
    }
}
