using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlatformsController : ControllerBase
	{
		private readonly IPlatformRepo _repository;
		private readonly IMapper _mapper;
		private readonly ICommandDataClient _commandDataClient;
		private readonly IMessageBusClient _messageBusClient;

		public PlatformsController(
			IPlatformRepo repository, 
			IMapper mapper,
			ICommandDataClient commandDataClient,
			IMessageBusClient messageBusClient)
		{
			_repository = repository;
			_mapper = mapper;
			_commandDataClient = commandDataClient;
			_messageBusClient = messageBusClient;
		}

		[HttpGet]
		public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
		{
			Console.WriteLine("---> Getting repository...");
			IEnumerable<Platform> platformItems = _repository.GetAllPlatforms();

			return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformItems));
		}

		[HttpGet("{id}", Name = nameof(GetPlatformById))]
		public ActionResult<PlatformReadDTO> GetPlatformById(int id)
		{
			Platform? platformItem = _repository.GetPlatformById(id);
			if (platformItem != null)
			{
				return Ok(_mapper.Map<PlatformReadDTO>(platformItem));
			}

			return NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<PlatformReadDTO>> CreatePlatform(PlatformCreateDTO platformCreateDTO)
		{
			Platform platformModel = _mapper.Map<Platform>(platformCreateDTO);

			_repository.CreatePlatform(platformModel);
			_repository.SaveChanges();

			PlatformReadDTO platformReadDTO = _mapper.Map<PlatformReadDTO>(platformModel);

			try
			{
				await _commandDataClient.SendPlatformToCommand(platformReadDTO);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			try
			{
				PlatformPublishedDTO platformPublishedDTO = _mapper.Map<PlatformPublishedDTO>(platformReadDTO);
				platformPublishedDTO.Event = "Platform_Published";
				_messageBusClient.PublishNewPlatform(platformPublishedDTO);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDTO.Id }, platformReadDTO);
		}
	}
}
