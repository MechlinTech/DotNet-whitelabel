using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs.CRM;
using backend.Services.CRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.Controllers.CRM
{
    [Authorize(Roles = "Admin,Manager,Sales")]
    public class ContactsController : BaseCRMController
    {
        private readonly IContactService _contactService;

        public ContactsController(
            IContactService contactService,
            ILogger<ContactsController> logger
        )
            : base(logger)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
        {
            try
            {
                var tenantId = GetTenantId();
                var contacts = await _contactService.GetAllContactsAsync(tenantId);
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ContactDto>>(ex);
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContactsByCustomer(
            Guid customerId
        )
        {
            try
            {
                var tenantId = GetTenantId();
                var contacts = await _contactService.GetContactsByCustomerAsync(
                    customerId,
                    tenantId
                );
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ContactDto>>(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetContact(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var contact = await _contactService.GetContactByIdAsync(id, tenantId);
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return HandleException<ContactDto>(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<ActionResult<ContactDto>> CreateContact(CreateContactDto contactDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var contact = await _contactService.CreateContactAsync(contactDto, tenantId);
                return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
            }
            catch (Exception ex)
            {
                return HandleException<ContactDto>(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateContact(Guid id, UpdateContactDto contactDto)
        {
            try
            {
                var tenantId = GetTenantId();
                await _contactService.UpdateContactAsync(id, contactDto, tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _contactService.DeleteContactAsync(id, tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
