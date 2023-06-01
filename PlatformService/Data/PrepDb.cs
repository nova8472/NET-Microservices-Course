using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder app, IWebHostEnvironment environment)
		{
			using(IServiceScope serviceScope = app.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), environment.IsProduction());
			}
		}

		private static void SeedData(AppDbContext? context, bool isProduction)
		{
			if(context == null)
			{
				Console.WriteLine("--> No context.");
				return;
			}

			if (isProduction)
			{
				Console.WriteLine("--> Attempting to apply migrations...");
				try
				{
				context.Database.Migrate();
				}
				catch(Exception ex)
				{
					Console.WriteLine($"--> Could not run migrations {ex}");
				}
			}

			if(!context.Platforms.Any())
			{
				Console.WriteLine("--> Seeding data...");

				context.Platforms.AddRange(
					new Platform() { Name = "Dot net", Publisher = "Microsoft", Cost = "Free" },
					new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
					new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
				);

				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have data.");
			}
		}
	}
}
