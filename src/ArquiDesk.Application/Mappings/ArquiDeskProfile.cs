using AutoMapper;
using ArquiDesk.Application.DTOs;
using ArquiDesk.Domain.Entities;

namespace ArquiDesk.Application.Mappings;

public class ArquiDeskProfile : Profile
{
    public ArquiDeskProfile()
    {
        CreateMap<Client, ClientDto>().ReverseMap();

        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Lead != null ? src.Lead.Name : string.Empty))
            .ReverseMap();

        CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue(DateTime.UtcNow)))
            .ReverseMap();
    }
}
