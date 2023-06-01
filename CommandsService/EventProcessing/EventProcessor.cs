using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using System.Text.Json;

namespace CommandsService.EventProcessing
{
	public class EventProcessor : IEventProcessor
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IMapper _mapper;

		public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
		{
			_scopeFactory = scopeFactory;
			_mapper = mapper;
		}

		public void ProcessEvent(string message)
		{
			var eventType = DetermineEvent(message);

			switch (eventType)
			{
				case EventType.PlatformPublished:
					AddPlatform(message);
					break;
				case EventType.Undertermined:
					break;
				default:
					break;
			}
		}

		private EventType DetermineEvent(string notificationMessage)
		{
			Console.WriteLine($"--> Determining event type: {notificationMessage}");

			var eventType = JsonSerializer.Deserialize<GenericEventDTO>(notificationMessage);

			switch (eventType?.Event)
			{
				case "Platform_Published":
					{
						Console.WriteLine("--> Platform publish event determined.");
						return EventType.PlatformPublished;
					}
				default:
					{
						Console.WriteLine("--> Platform publish event determined.");
						return EventType.Undertermined;
					}
			}
		}

		private void AddPlatform(string platformPublishedMessage) 
		{
			using(var scope = _scopeFactory.CreateScope())
			{
				var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

				var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);

				try
				{
					var platform = _mapper.Map<Platform>(platformPublishedDTO);
					if(!repo.ExternalPlatformExists(platform.ExternalID))
					{
						repo.CreatePlatform(platform);
						repo.SaveChanges();
						Console.WriteLine($"--> Platform created: {platform.Id}");
					}
					else
					{
						Console.WriteLine("--> Platform already exists. "); 
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine($"--> Could not add platform to db: {ex.Message}");
				}
			}
		}
	}

	enum EventType
	{
		PlatformPublished,
		Undertermined
	}
}
