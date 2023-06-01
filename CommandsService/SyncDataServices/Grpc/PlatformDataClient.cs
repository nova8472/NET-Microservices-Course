using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
	public class PlatformDataClient : IPlatformDataClient
	{
		private readonly IConfiguration _configuration;
		private readonly IMapper _mapper;

		public PlatformDataClient(IConfiguration configuration, IMapper mapper)
		{
			_configuration = configuration;
			_mapper = mapper;
		}

		public IEnumerable<Platform>? ReturnAllPlatforms()
		{
			var grpcPlatform = _configuration["GrpcPlatform"] ?? string.Empty;
			Console.WriteLine($"--> Calling GRPC service {grpcPlatform}");

			var channel = GrpcChannel.ForAddress(grpcPlatform);
			var client = new GrpcPlatform.GrpcPlatformClient(channel);
			var request = new GetAllRequest();

			try
			{
				var reply = client.GetAllPlatforms(request);
				return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
			}
			catch(Exception ex)
			{
				Console.WriteLine($"--> Could not call GRPC server: {ex.Message}");
				return null;
			}
		}
	}
}
