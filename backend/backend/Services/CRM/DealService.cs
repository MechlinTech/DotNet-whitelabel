using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using backend.DBContext;
using backend.DTOs.CRM;
using backend.Models.CRM;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services.CRM
{
    public class DealService : BaseCRMService, IDealService
    {
        public DealService(TenantDbContext context, IMapper mapper, ILogger<DealService> logger)
            : base(context, mapper, logger) { }

        public async Task<IEnumerable<DealDto>> GetAllDealsAsync(Guid tenantId)
        {
            try
            {
                await EnsureDatabaseExists();
                var deals = await _context.Deals.Include(d => d.Customer).ToListAsync();
                return _mapper.Map<IEnumerable<DealDto>>(deals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all deals for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<IEnumerable<DealDto>> GetDealsByCustomerAsync(
            Guid customerId,
            Guid tenantId
        )
        {
            try
            {
                await EnsureDatabaseExists();
                var deals = await _context
                    .Deals.Include(d => d.Customer)
                    .Where(d => d.CustomerId == customerId)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<DealDto>>(deals);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting deals for customer {CustomerId} in tenant {TenantId}",
                    customerId,
                    tenantId
                );
                throw;
            }
        }

        public async Task<IEnumerable<DealDto>> GetDealsByStageAsync(DealStage stage, Guid tenantId)
        {
            try
            {
                await EnsureDatabaseExists();
                var deals = await _context
                    .Deals.Include(d => d.Customer)
                    .Where(d => d.Stage == stage)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<DealDto>>(deals);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting deals for stage {Stage} in tenant {TenantId}",
                    stage,
                    tenantId
                );
                throw;
            }
        }

        public async Task<DealDto> GetDealByIdAsync(Guid id, Guid tenantId)
        {
            try
            {
                await EnsureDatabaseExists();
                var deal = await _context
                    .Deals.Include(d => d.Customer)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (deal == null)
                {
                    throw new KeyNotFoundException($"Deal with ID {id} not found");
                }

                return _mapper.Map<DealDto>(deal);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting deal {DealId} for tenant {TenantId}",
                    id,
                    tenantId
                );
                throw;
            }
        }

        public async Task<DealDto> CreateDealAsync(CreateDealDto dealDto, Guid tenantId)
        {
            try
            {
                await EnsureDatabaseExists();
                var deal = _mapper.Map<Deal>(dealDto);
                deal.CreatedAt = DateTime.UtcNow;

                _context.Deals.Add(deal);
                await _context.SaveChangesAsync();

                return _mapper.Map<DealDto>(deal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating deal for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<DealDto> UpdateDealAsync(Guid id, UpdateDealDto dealDto, Guid tenantId)
        {
            try
            {
                await EnsureDatabaseExists();
                var deal = await _context.Deals.FindAsync(id);

                if (deal == null)
                {
                    throw new KeyNotFoundException($"Deal with ID {id} not found");
                }

                _mapper.Map(dealDto, deal);
                deal.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return _mapper.Map<DealDto>(deal);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating deal {DealId} for tenant {TenantId}",
                    id,
                    tenantId
                );
                throw;
            }
        }

        public async Task DeleteDealAsync(Guid id, Guid tenantId)
        {
            try
            {
                await EnsureDatabaseExists();
                var deal = await _context.Deals.FindAsync(id);

                if (deal == null)
                {
                    throw new KeyNotFoundException($"Deal with ID {id} not found");
                }

                _context.Deals.Remove(deal);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting deal {DealId} for tenant {TenantId}",
                    id,
                    tenantId
                );
                throw;
            }
        }
    }
}
