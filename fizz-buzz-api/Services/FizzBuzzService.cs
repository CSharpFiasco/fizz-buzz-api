using FizzBuzz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FizzBuzz.Services;

public interface IFizzBuzzService
{
    IResult Process(FizzBuzzRequest request);
}

public class FizzBuzzService: IFizzBuzzService
{
    public IResult Process(FizzBuzzRequest request)
    {
        // check for empty request
        if (request == null || request.Multiples.Length == 0)
        {
            return Results.Problem("Request is empty.", statusCode: 400);
        }

        // check for duplicates
        var duplicates = request.Multiples.GroupBy(x => x.Multiple)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicates.Count != 0)
        {
            return Results.Problem($"Duplicates found. {string.Join(", ", duplicates)}", statusCode: 400);
        }

        // check for invalid multiples and words
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
    }
}
