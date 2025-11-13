using AutoMapper;
using Fadebook.DTOs;
using Fadebook.Models;

namespace Fadebook.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<BarberModel, BarberDto>().ReverseMap();
        CreateMap<CreateBarberDto, BarberModel>();
        CreateMap<CustomerModel, CustomerDto>().ReverseMap();
        CreateMap<ServiceModel, ServiceDto>().ReverseMap();
        CreateMap<AppointmentModel, AppointmentDto>().ReverseMap();
    }
}
