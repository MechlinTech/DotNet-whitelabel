using AutoMapper;
using backend.DBContext;
using backend.DTOs.CRM;
using backend.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.CRM
{
    public class ContactService : BaseCRMService, IContactService
    {
        public ContactService(
            TenantDbContext context,
            IMapper mapper,
            ILogger<ContactService> logger
        )
            : base(context, mapper, logger) { }

        public async Task<IEnumerable<ContactDto>> GetAllContactsAsync(Guid tenantId)
        {
            var contacts = await _context.Contacts.Where(c => c.TenantId == tenantId).ToListAsync();

            return _mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<IEnumerable<ContactDto>> GetContactsByCustomerAsync(
            Guid customerId,
            Guid tenantId
        )
        {
            var contacts = await _context
                .Contacts.Where(c => c.CustomerId == customerId && c.TenantId == tenantId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<ContactDto> GetContactByIdAsync(Guid id, Guid tenantId)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c =>
                c.Id == id && c.TenantId == tenantId
            );

            if (contact == null)
                throw new KeyNotFoundException($"Contact with ID {id} not found");

            return _mapper.Map<ContactDto>(contact);
        }

        public async Task<ContactDto> CreateContactAsync(CreateContactDto contactDto, Guid tenantId)
        {
            var contact = _mapper.Map<Contact>(contactDto);
            contact.Id = Guid.NewGuid();
            contact.TenantId = tenantId;
            contact.CreatedAt = DateTime.UtcNow;

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return _mapper.Map<ContactDto>(contact);
        }

        public async Task<ContactDto> UpdateContactAsync(
            Guid id,
            UpdateContactDto contactDto,
            Guid tenantId
        )
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c =>
                c.Id == id && c.TenantId == tenantId
            );

            if (contact == null)
                throw new KeyNotFoundException($"Contact with ID {id} not found");

            _mapper.Map(contactDto, contact);
            contact.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<ContactDto>(contact);
        }

        public async Task DeleteContactAsync(Guid id, Guid tenantId)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c =>
                c.Id == id && c.TenantId == tenantId
            );

            if (contact == null)
                throw new KeyNotFoundException($"Contact with ID {id} not found");

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }
}
