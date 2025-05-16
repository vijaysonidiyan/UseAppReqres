using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserWebApp.Integration.Clients;
using UserWebApp.Integration.Models;

namespace UserWebApp.Integration.Service
{
	public class UserService
	{
		private readonly ReqResApiClient _apiClient;
		private readonly IMemoryCache _cache;
		private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

		public UserService(ReqResApiClient apiClient, IMemoryCache cache)
		{
			_apiClient = apiClient;
			_cache = cache;
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			return await _apiClient.GetUserByIdAsync(userId);
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			string cacheKey = "AllUsers";

			if (_cache.TryGetValue(cacheKey, out IEnumerable<User> cachedUsers))
			{				
				return cachedUsers;
			}

			var allUsers = new List<User>();
			int currentPage = 1;
			PagedResponse<User> pageResult;

			do
			{
				pageResult = await _apiClient.GetUsersPageAsync(currentPage);

				if (pageResult?.Data != null)
				{
					allUsers.AddRange(pageResult.Data);
				}

				currentPage++;

			} while (pageResult != null && currentPage <= pageResult.Total);

			_cache.Set(cacheKey, allUsers, _cacheExpiration);

			return allUsers;
		}
	}
}
