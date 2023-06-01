using AutoMapper;
using PlatformService.DTOs;
using PlatformService.Models;

namespace PlatformService.Profiles
{
	public class PlatformsProfile : Profile
	{
		public PlatformsProfile()
		{
			// Source -> Target
			CreateMap<Platform, PlatformReadDTO>();
			CreateMap<PlatformCreateDTO, Platform>();
			CreateMap<PlatformReadDTO, PlatformPublishedDTO>();
			CreateMap<Platform, GrpcPlatformModel>()
				.ForMember(destination => destination.PlatformId, opt => opt.MapFrom(source => source.Id))
				.ForMember(destination => destination.Name, opt => opt.MapFrom(source => source.Name))
				.ForMember(destination => destination.Publisher, opt => opt.MapFrom(source => source.Publisher));
		}
	}
}
