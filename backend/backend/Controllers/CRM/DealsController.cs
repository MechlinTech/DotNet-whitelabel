using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs.CRM;
using backend.Models.CRM;
using backend.Services.CRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.Controllers.CRM
{
    [Authorize(Roles = "Admin,Manager,Sales")]
    public class DealsController : BaseCRMController
    {
        private readonly IDealService _dealService;

        public DealsController(IDealService dealService, ILogger<DealsController> logger)
            : base(logger)
        {
            _dealService = dealService ?? throw new ArgumentNullException(nameof(dealService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetDeals()
        {
            try
            {
                _logger.LogInformation("Getting all deals for tenant");
                var tenantId = GetTenantId();
                var deals = await _dealService.GetAllDealsAsync(tenantId);
                return Ok(deals);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<DealDto>>(ex);
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetDealsByCustomer(Guid customerId)
        {
            try
            {
                _logger.LogInformation("Getting deals for customer {CustomerId}", customerId);
                var tenantId = GetTenantId();
                var deals = await _dealService.GetDealsByCustomerAsync(customerId, tenantId);
                return Ok(deals);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<DealDto>>(ex);
            }
        }

        [HttpGet("stage/{stage}")]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetDealsByStage(DealStage stage)
        {
            try
            {
                _logger.LogInformation("Getting deals for stage {Stage}", stage);
                var tenantId = GetTenantId();
                var deals = await _dealService.GetDealsByStageAsync(stage, tenantId);
                return Ok(deals);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<DealDto>>(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DealDto>> GetDeal(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting deal {DealId}", id);
                var tenantId = GetTenantId();
                var deal = await _dealService.GetDealByIdAsync(id, tenantId);
                return Ok(deal);
            }
            catch (Exception ex)
            {
                return HandleException<DealDto>(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<ActionResult<DealDto>> CreateDeal(CreateDealDto dealDto)
        {
            try
            {
                _logger.LogInformation("Creating new deal");
                var tenantId = GetTenantId();
                var deal = await _dealService.CreateDealAsync(dealDto, tenantId);
                return CreatedAtAction(nameof(GetDeal), new { id = deal.Id }, deal);
            }
            catch (Exception ex)
            {
                return HandleException<DealDto>(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateDeal(Guid id, UpdateDealDto dealDto)
        {
            try
            {
                _logger.LogInformation("Updating deal {DealId}", id);
                var tenantId = GetTenantId();
                await _dealService.UpdateDealAsync(id, dealDto, tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDeal(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting deal {DealId}", id);
                var tenantId = GetTenantId();
                await _dealService.DeleteDealAsync(id, tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
