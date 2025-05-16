using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserWebApp.Integration.Models;

namespace UserWebApp.Integration.Clients
{
	public class ApiException : Exception
	{
		public HttpStatusCode StatusCode { get; }
		public ApiException(string message, HttpStatusCode statusCode) : base(message)
		{
			StatusCode = statusCode;
		}
	}

	public class NotFoundException : ApiException
	{
		public NotFoundException(string message) : base(message, HttpStatusCode.NotFound) { }
	}

	public class ReqResApiClient
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<ReqResApiClient> _logger;

		public ReqResApiClient(HttpClient httpClient, ILogger<ReqResApiClient> logger)
		{
			_httpClient = httpClient;
			_logger = logger;

			// Example: Add default header if needed
			_httpClient.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");
		}

		public async Task<PagedResponse<User>> GetUsersPageAsync(int page)
		{
			try
			{
				var response = await _httpClient.GetAsync($"users?page={page}");

				if (response.StatusCode == HttpStatusCode.NotFound)
				{
					_logger.LogWarning("Page {Page} not found.", page);
					throw new NotFoundException($"Page {page} not found.");
				}

				response.EnsureSuccessStatusCode();

				var json = await response.Content.ReadAsStringAsync();

				var result = JsonSerializer.Deserialize<PagedResponse<User>>(json, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});

				if (result == null)
				{
					throw new JsonException("Failed to deserialize users page response.");
				}

				return result;
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "Network error while fetching page {Page}.", page);
				throw new Exception("Network error occurred when fetching users.", ex);
			}
			catch (JsonException ex)
			{
				_logger.LogError(ex, "Deserialization error on page {Page}.", page);
				throw;
			}
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"users/{userId}");

				if (response.StatusCode == HttpStatusCode.NotFound)
				{
					_logger.LogWarning("User {UserId} not found.", userId);
					throw new NotFoundException($"User with ID {userId} not found.");
				}

				response.EnsureSuccessStatusCode();

				var json = await response.Content.ReadAsStringAsync();

				var result = JsonSerializer.Deserialize<UserResponse>(json, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});

				if (result?.Data == null)
				{
					throw new JsonException($"Failed to deserialize user {userId} response.");
				}

				return result.Data;
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "Network error while fetching user {UserId}.", userId);
				throw new Exception("Network error occurred when fetching user.", ex);
			}
			catch (JsonException ex)
			{
				_logger.LogError(ex, "Deserialization error for user {UserId}.", userId);
				throw;
			}
		}
	}
}
