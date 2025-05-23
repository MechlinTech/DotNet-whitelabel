using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs.CRM;
using backend.Models.CRM;
using backend.Services.CRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.CRM
{
    // [Authorize(Roles = "Admin,Manager,Sales")]
    public class CustomersController : BaseCRMController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(
            ICustomerService customerService,
            ILogger<CustomersController> logger
        )
            : base(logger)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            try
            {
                var tenantId = GetTenantId();
                var customers = await _customerService.GetAllCustomersAsync(tenantId);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<CustomerDto>>(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var customer = await _customerService.GetCustomerByIdAsync(id, tenantId);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return HandleException<CustomerDto>(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto customerDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var customer = await _customerService.CreateCustomerAsync(customerDto, tenantId);
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                return HandleException<CustomerDto>(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateCustomer(Guid id, UpdateCustomerDto customerDto)
        {
            try
            {
                var tenantId = GetTenantId();
                await _customerService.UpdateCustomerAsync(id, customerDto, tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _customerService.DeleteCustomerAsync(id, tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomersByStatus(
            CustomerStatus status
        )
        {
            try
            {
                var tenantId = GetTenantId();
                var customers = await _customerService.GetCustomersByStatusAsync(status, tenantId);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<CustomerDto>>(ex);
            }
        }
    }
}
