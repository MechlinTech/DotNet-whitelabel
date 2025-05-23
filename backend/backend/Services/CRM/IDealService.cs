using backend.DTOs.CRM;
using backend.Models.CRM;

namespace backend.Services.CRM
{
    public interface IDealService
    {
        Task<IEnumerable<DealDto>> GetAllDealsAsync(Guid tenantId);
        Task<IEnumerable<DealDto>> GetDealsByCustomerAsync(Guid customerId, Guid tenantId);
        Task<IEnumerable<DealDto>> GetDealsByStageAsync(DealStage stage, Guid tenantId);
        Task<DealDto> GetDealByIdAsync(Guid id, Guid tenantId);
        Task<DealDto> CreateDealAsync(CreateDealDto dealDto, Guid tenantId);
        Task<DealDto> UpdateDealAsync(Guid id, UpdateDealDto dealDto, Guid tenantId);
        Task DeleteDealAsync(Guid id, Guid tenantId);
    }
}
