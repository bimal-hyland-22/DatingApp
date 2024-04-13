
using AutoMapper;
using API.DTOs;
using API.Extensions;
using API.Entities;
namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser,MemberDTO>().
            ForMember(dest=>dest.PhotoUrl,opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain).Url))
            .ForMember(dest=>dest.Age,opt=>opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoDTO>();
            CreateMap<MemberUpdateDTO,AppUser>();
            CreateMap<RegisterDTO,AppUser>();
            CreateMap<Message,MessageDto>()
            .ForMember(d=>d.SenderPhotoUrl,o=>o.MapFrom(s=>s.Sender.Photos.FirstOrDefault(x=>x.IsMain).Url))
             .ForMember(d=>d.RecipentPhotoUrl,o=>o.MapFrom(s=>s.Recipent.Photos.FirstOrDefault(x=>x.IsMain).Url));
        }
    }
}