using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
	public class CommandsProfile : Profile
	{
		public CommandsProfile()
		{
			CreateMap<Platform, PlatformReadDTO>();
			CreateMap<Command, CommandReadDTO>();
			CreateMap<Command, CommandCreateDTO>();
			CreateMap<CommandCreateDTO, Command>();
			CreateMap<PlatformPublishedDTO, Platform>()
				.ForMember(destination => destination.ExternalID, opt => opt.MapFrom(source => source.Id));
			CreateMap<GrpcPlatformModel, Platform>()
				.ForMember(destination => destination.ExternalID, opt => opt.MapFrom(source => source.PlatformId))
				.ForMember(destination => destination.Name, opt => opt.MapFrom(source => source.Name))
				.ForMember(destination => destination.Commands, opt => opt.Ignore());
		}
	}
}
