using AutoMapper;
using CommitsManager.Domain.Entities;
using Octokit;

namespace CommitsManager.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Octokit.Repository, RepositoryEntity>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner.Login))
                .ReverseMap();
            CreateMap<Commit, CommitEntity>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Name))
                .ReverseMap();
        }
    }
}
