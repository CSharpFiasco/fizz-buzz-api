# Fizz Buzz API

This is not your average Fizz Buzz API. It allows you to specify the range of numbers and the Fizz Buzz rules, making it highly customizable.

## Features

### Logging

The API logs using Serilog and writes logs to a file in the `LogFiles` directory. A production application should use a more robust logging solution, a logging service (Application Insights/Splunk) or a database.

### Error Handling

The API includes error handling that logs exceptions by way of Serilog. We use `ProblemDetails` according to the [RFC 9457](https://www.rfc-editor.org/rfc/rfc9457.html) standard for error responses.

### Caching

The API uses in-memory caching to store the results of Fizz Buzz calculations. This improves performance by avoiding repeated calculations for the same range and rules.

### More Remarks

More remarks can be found in the frontend repository: [Fizz Buzz Frontend](https://github.com/CSharpFiasco/fizz-buzz)