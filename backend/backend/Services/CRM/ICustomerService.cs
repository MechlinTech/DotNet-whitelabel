using backend.DTOs.CRM;
using backend.Models.CRM;

namespace backend.Services.CRM
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(Guid tenantId);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id, Guid tenantId);
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto customerDto, Guid tenantId);
        Task<CustomerDto> UpdateCustomerAsync(
            Guid id,
            UpdateCustomerDto customerDto,
            Guid tenantId
        );
        Task DeleteCustomerAsync(Guid id, Guid tenantId);
        Task<IEnumerable<CustomerDto>> GetCustomersByStatusAsync(
            CustomerStatus status,
            Guid tenantId
        );
    }
}
