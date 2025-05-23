using backend.DTOs.CRM;

namespace backend.Services.CRM
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetAllContactsAsync(Guid tenantId);
        Task<IEnumerable<ContactDto>> GetContactsByCustomerAsync(Guid customerId, Guid tenantId);
        Task<ContactDto> GetContactByIdAsync(Guid id, Guid tenantId);
        Task<ContactDto> CreateContactAsync(CreateContactDto contactDto, Guid tenantId);
        Task<ContactDto> UpdateContactAsync(Guid id, UpdateContactDto contactDto, Guid tenantId);
        Task DeleteContactAsync(Guid id, Guid tenantId);
    }
}
