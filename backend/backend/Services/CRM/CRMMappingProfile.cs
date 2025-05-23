using AutoMapper;
using backend.DTOs.CRM;
using backend.Models.CRM;

namespace backend.Services.CRM
{
    public class CRMMappingProfile : Profile
    {
        public CRMMappingProfile()
        {
            // Customer mappings
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Contact mappings
            CreateMap<Contact, ContactDto>();
            CreateMap<CreateContactDto, Contact>();
            CreateMap<UpdateContactDto, Contact>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Deal mappings
            CreateMap<Deal, DealDto>();
            CreateMap<CreateDealDto, Deal>();
            CreateMap<UpdateDealDto, Deal>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
