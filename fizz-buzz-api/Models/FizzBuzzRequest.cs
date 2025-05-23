namespace FizzBuzz.Models;

public class FizzBuzzItem
{
    public int Multiple { get; init; }
    public string? WordToPrint { get; init; }
}

public class FizzBuzzRequest
{
    public FizzBuzzItem[] Multiples { get; init; } = [];
    public int MaxNumber { get; init; } = 100;
}
