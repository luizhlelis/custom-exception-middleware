using CustomExceptionMiddleware.Responses;
using CustomExceptionMiddleware.WebAppTest;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CustomExceptionMiddleware.Tests
{
    public class CustomExceptionMiddlewareWebAppTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private string _url = "/customer";
        private const string UnexpectedError = "UNEXPECTED_ERROR";
        private const string ValidationErrors = "VALIDATION_ERRORS";

        public CustomExceptionMiddlewareWebAppTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Theory(DisplayName = "Should return Ok and get customers")]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(50)]
        public async Task GetAsync_GetCustomers_ShouldReturnOK(int customersCount)
        {
            // Arrange
            _url += $"?count={customersCount}";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be200Ok();
            var responseContent = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
            responseContent.Should().NotBeNullOrEmpty();
            responseContent.Should().HaveCount(customersCount);
        }

        [Fact(DisplayName = "Should return bad request and throw a domain exception")]
        public async Task GetAsync_ThrowDomainException_ShouldReturnBadRequest()
        {
            // Arrange
            _url += "/domain";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be400BadRequest();
            var responseContent = await response.Content.ReadAsAsync<CustomErrorResponse>();
            responseContent.Error.Msg.Should().Be("Custom domain exception message");
            responseContent.Type.Should().Be(ValidationErrors);
        }

        [Fact(DisplayName = "Should return forbidden and throw a cannot access exception")]
        public async Task GetAsync_ThrowCannotAccessException_ShouldReturnForbidden()
        {
            // Arrange
            _url += "/cannot-access";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be403Forbidden();
            var responseContent = await response.Content.ReadAsAsync<CustomErrorResponse>();
            responseContent.Error.Msg.Should().Be("Custom cannot access exception message");
            responseContent.Type.Should().Be(ValidationErrors);
        }

        [Fact(DisplayName = "Should return not found and throw a not found exception")]
        public async Task GetAsync_ThrowNotFoundException_ShouldNotFound()
        {
            // Arrange
            _url += "/not-found";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be404NotFound();
            var responseContent = await response.Content.ReadAsAsync<CustomErrorResponse>();
            responseContent.Error.Msg.Should().Be("Custom not found exception message");
            responseContent.Type.Should().Be(ValidationErrors);
        }
        
        [Fact(DisplayName = "Should return unauthorized and throw an unauthorized exception")]
        public async Task GetAsync_ThrowUnauthorizedException_ShouldUnauthorized()
        {
            // Arrange
            _url += "/unauthorized";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be401Unauthorized();
            var responseContent = await response.Content.ReadAsAsync<CustomErrorResponse>();
            responseContent.Error.Msg.Should().Be("Custom unauthorized exception message");
            responseContent.Type.Should().Be(ValidationErrors);
        }

        [Fact(DisplayName = "Should return internal server error and throw an exception")]
        public async Task GetAsync_ThrowException_ShouldReturnInternalServerError()
        {
            // Arrange
            _url += "/exception";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be500InternalServerError();
            var responseContent = await response.Content.ReadAsAsync<CustomErrorResponse>();
            responseContent.Error.Msg.Should().Be("Custom exception message");
            responseContent.Type.Should().Be(UnexpectedError);
        }

        [Fact(DisplayName = "Should return Ok and get customers from domain url")]
        public async Task GetAsyncDomain_GetCustomers_ShouldReturnOK()
        {
            // Arrange
            _url += $"/domain?returnCustomer={true}";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be200Ok();
            var responseContent = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Should return Ok and get customers from cannot access url")]
        public async Task GetAsyncCannotAccess_GetCustomers_ShouldReturnOK()
        {
            // Arrange
            _url += $"/cannot-access?returnCustomer={true}";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be200Ok();
            var responseContent = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Should return Ok and get customers from not found url")]
        public async Task GetAsyncNotFound_GetCustomers_ShouldReturnOK()
        {
            // Arrange
            _url += $"/not-found?returnCustomer={true}";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be200Ok();
            var responseContent = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
            responseContent.Should().NotBeNullOrEmpty();
        }
        
        [Fact(DisplayName = "Should return Ok and get customers from unauthorized url")]
        public async Task GetAsyncUnauthorized_GetCustomers_ShouldReturnOK()
        {
            // Arrange
            _url += $"/unauthorized?returnCustomer={true}";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be200Ok();
            var responseContent = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Should return Ok and get customers from exception url")]
        public async Task GetAsyncException_GetCustomers_ShouldReturnOK()
        {
            // Arrange
            _url += $"/exception?returnCustomer={true}";

            // Act
            var response = await _client.GetAsync(_url);

            // Assert
            response.Should().Be200Ok();
            var responseContent = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
            responseContent.Should().NotBeNullOrEmpty();
        }
    }
}
