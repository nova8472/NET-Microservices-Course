using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder applicationBuilder)
		{
			using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
			{
				var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
				var platformsCollection = grpcClient?.ReturnAllPlatforms();
				SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platformsCollection);
			}
		}

		private static void SeedData(ICommandRepo? repo, IEnumerable<Platform>? platforms)
		{
			if (repo == null)
			{
				return;
			}

			if (platforms == null)
			{
				return;
			}

			Console.WriteLine($"--> Seeding new platforms... ");

			foreach (var platform in platforms)
			{
				if (!repo.ExternalPlatformExists(platform.ExternalID))
				{
					repo.CreatePlatform(platform);
				}

				repo.SaveChanges();
			}
		}
	}
}
