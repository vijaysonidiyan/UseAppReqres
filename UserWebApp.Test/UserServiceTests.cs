using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using UserWebApp.Integration.Clients;
using UserWebApp.Integration.Configration;
using UserWebApp.Integration.Service;

namespace UserWebApp.Test
{
	public class UserServiceTests
	{
		[Fact]
		public async Task GetUserByIdAsync_ReturnsUser_WhenApiReturnsSuccess()
		{
			// Arrange
			var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
			var userJson = @"{
            ""data"": {
                ""id"": 1,
                ""email"": ""george.bluth@reqres.in"",
                ""first_name"": ""George"",
                ""last_name"": ""Bluth"",
                ""avatar"": ""https://reqres.in/img/faces/1-image.jpg""
            }
        }";

			mockHttpMessageHandler
	.Protected()
	.Setup<Task<HttpResponseMessage>>(
		"SendAsync", // Method name (must match protected method)
		ItExpr.IsAny<HttpRequestMessage>(),
		ItExpr.IsAny<CancellationToken>()
	)
	.ReturnsAsync(new HttpResponseMessage
	{
		StatusCode = HttpStatusCode.OK,
		Content = new StringContent(userJson)
	});

			var httpClient = new HttpClient(mockHttpMessageHandler.Object)
			{
				BaseAddress = new Uri("https://reqres.in/api/")
			};

			var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
			var logger = loggerFactory.CreateLogger<ReqResApiClient>();

			var apiClient = new ReqResApiClient(httpClient, logger);

			var memoryCache = new MemoryCache(new MemoryCacheOptions());
			var userService = new UserService(apiClient, memoryCache);

			// Act
			var result = await userService.GetUserByIdAsync(1);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			Assert.Equal("George", result.First_Name);
		}
	}
}