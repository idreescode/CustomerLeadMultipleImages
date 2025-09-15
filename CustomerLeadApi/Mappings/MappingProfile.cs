using AutoMapper;
using CustomerLeadApi.DTOs;
using CustomerLeadApi.Models;

namespace CustomerLeadApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Contact mappings
            CreateMap<Contact, ContactDTO>()
                .ForMember(dest => dest.ImageCount, opt => opt.MapFrom(src => src.Images.Count));

            CreateMap<CreateContactDTO, Contact>();
            CreateMap<UpdateContactDTO, Contact>();

            // ContactImage mappings
            CreateMap<ContactImage, ContactImageDTO>();
            CreateMap<UploadImageDTO, ContactImage>()
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
