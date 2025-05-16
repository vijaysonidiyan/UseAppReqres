using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using UserWebApp.Integration;
using UserWebApp.Integration.Clients;
using UserWebApp.Integration.Configration;
using UserWebApp.Integration.Service; // Namespace from your class library

class Program
{
	static async Task Main(string[] args)
	{
		var services = new ServiceCollection();
				
		services.AddLogging(builder => builder.AddConsole());

		services.AddMemoryCache();

		services.Configure<ReqResOptions>(options =>
		{
			options.BaseUrl = "https://reqres.in/api/";
		});

		services.AddHttpClient<ReqResApiClient>((sp, client) =>
		{
			var options = sp.GetRequiredService<IOptions<ReqResOptions>>().Value;
			client.BaseAddress = new Uri(options.BaseUrl);
		});
		
		services.AddTransient<UserService>();

		var provider = services.BuildServiceProvider();

		var userService = provider.GetRequiredService<UserService>();

		try
		{
			var users = await userService.GetAllUsersAsync();
			Console.WriteLine($"Fetched {users.Count()} users:");
			foreach (var user in users)
			{
				Console.WriteLine($"{user.Id}: {user.First_Name} {user.Last_Name} - {user.Email}");
			}

			Console.Write("Do you want to fetch users from cache? (y/n): ");
			var input = Console.ReadLine();

			if (input?.Trim().ToLower() == "y")
			{
				var cacheUsers = await userService.GetAllUsersAsync();
				Console.WriteLine($"Cache User List Fetched {cacheUsers.Count()} users:");
				foreach (var user in cacheUsers)
				{
					Console.WriteLine($"{user.Id}: {user.First_Name} {user.Last_Name} - {user.Email}");
				}
			}
			else
			{
				Console.WriteLine("Skipping cached user list.");
			}

			Console.Write("Enter User Id to get detail Data : ");
			var inputUserId = Console.ReadLine();

			if (!string.IsNullOrEmpty(inputUserId))
			{
				var userById = await userService.GetUserByIdAsync(Convert.ToInt32(inputUserId));
				Console.WriteLine($"Fetched single users:");
				Console.WriteLine($"{userById.Id}: {userById.First_Name} {userById.Last_Name} - {userById.Email}");
			}
			else
			{
				Console.WriteLine(".");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}
	
	}
}
