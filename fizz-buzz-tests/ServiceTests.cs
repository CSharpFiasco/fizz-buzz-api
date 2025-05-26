using FizzBuzz.Models;
using FizzBuzz.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.ObjectModel;

namespace FizzBuzz.Tests
{
    public class FizzBuzzServiceTests
    {
        private IFizzBuzzService _service;
        private readonly Mock<ILogger<FizzBuzzService>> _loggerMock;
        public FizzBuzzServiceTests()
        {
            _loggerMock = new();
            _service = new FizzBuzzService(_loggerMock.Object);
        }

        [Fact]
        public void GivenFizzBuzzService_WhenNullRequest_ThenReturn400()
        {
            // Arrange
            FizzBuzzRequest request = null!;
            // Act
            var result = _service.Process(request);
            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(400, problemResult.StatusCode);
        }

        [Fact]
        public void GivenFizzBuzzService_WhenEmptyRequest_ThenReturn400()
        {
            // Arrange
            var request = new FizzBuzzRequest { Multiples = [], MaxNumber = 0 };
            // Act
            var result = _service.Process(request);
            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(400, problemResult.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void GivenFizzBuzzService_WhenInvalidMaxNumber_ThenReturn400(int invalidValue)
        {
            // Arrange
            var request = new FizzBuzzRequest
            {
                Multiples =
                [
                    new FizzBuzzItem { Multiple = 3, WordToPrint = "Fizz" },
                    new FizzBuzzItem { Multiple = 5, WordToPrint = "Buzz" }
                ],
                MaxNumber = invalidValue
            };
            // Act
            var result = _service.Process(request);
            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(400, problemResult.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void GivenFizzBuzzService_WhenInvalidMultiple_ThenReturn400(int invalidValue)
        {
            // Arrange
            var request = new FizzBuzzRequest
            {
                Multiples =
                [
                    new FizzBuzzItem { Multiple = invalidValue, WordToPrint = "Fizz" },
                    new FizzBuzzItem { Multiple = 5, WordToPrint = "Buzz" }
                ],
                MaxNumber = 16
            };
            // Act
            var result = _service.Process(request);
            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(400, problemResult.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")] // whitespace
        [InlineData(null)] // null
        public void GivenFizzBuzzService_WhenInvalidWordToPrint_ThenReturn400(string? invalidValue)
        {
            // Arrange
            var request = new FizzBuzzRequest
            {
                Multiples =
                [
                    new FizzBuzzItem { Multiple = 3, WordToPrint = invalidValue },
                    new FizzBuzzItem { Multiple = 5, WordToPrint = "Buzz" }
                ],
                MaxNumber = 16
            };
            // Act
            var result = _service.Process(request);
            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(400, problemResult.StatusCode);
        }

        [Fact]
        public void GivenFizzBuzzService_WhenValidRequest_ThenReturn200()
        {
            // Arrange
            var request = new FizzBuzzRequest
            {
                Multiples =
                [
                    new FizzBuzzItem { Multiple = 3, WordToPrint = "Fizz" },
                    new FizzBuzzItem { Multiple = 5, WordToPrint = "Buzz" },
                    new FizzBuzzItem { Multiple = 6, WordToPrint = "Bazz" }
                ],
                MaxNumber = 19
            };
            // Act
            var result = _service.Process(request);
            // Assert
            var okResult = Assert.IsType<Ok<ReadOnlyDictionary<int, string>>>(result);
            Assert.Equal(200, okResult.StatusCode);

            var okBody = okResult.Value;
            Assert.Collection(okBody, (item) =>
            {
                Assert.Equal(3, item.Key);
                Assert.Equal("Fizz", item.Value);
            }, (item) =>
            {
                Assert.Equal(5, item.Key);
                Assert.Equal("Buzz", item.Value);
            }, (item) =>
            {
                Assert.Equal(6, item.Key);
                Assert.Equal("FizzBazz", item.Value);
            }, (item) =>
            {
                Assert.Equal(9, item.Key);
                Assert.Equal("Fizz", item.Value);
            }, (item) =>
            {
                Assert.Equal(10, item.Key);
                Assert.Equal("Buzz", item.Value);
            }, (item) =>
            {
                Assert.Equal(12, item.Key);
                Assert.Equal("FizzBazz", item.Value);
            }, (item) =>
            {
                Assert.Equal(15, item.Key);
                Assert.Equal("FizzBuzz", item.Value);
            }, (item) =>
            {
                Assert.Equal(18, item.Key);
                Assert.Equal("FizzBazz", item.Value);
            });
        }
    }
}
