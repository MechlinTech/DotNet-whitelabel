using AutoMapper;
using backend.DBContext;
using backend.DTOs.CRM;
using backend.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.CRM
{
    public class CustomerService : BaseCRMService, ICustomerService
    {
        public CustomerService(
            TenantDbContext context,
            IMapper mapper,
            ILogger<CustomerService> logger
        )
            : base(context, mapper, logger) { }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(Guid tenantId)
        {
            var customers = await _context
                .Customers.Where(c => c.TenantId == tenantId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id, Guid tenantId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c =>
                c.Id == id && c.TenantId == tenantId
            );

            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {id} not found");

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateCustomerAsync(
            CreateCustomerDto customerDto,
            Guid tenantId
        )
        {
            var customer = _mapper.Map<Customer>(customerDto);
            customer.Id = Guid.NewGuid();
            customer.TenantId = tenantId;
            customer.CreatedAt = DateTime.UtcNow;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> UpdateCustomerAsync(
            Guid id,
            UpdateCustomerDto customerDto,
            Guid tenantId
        )
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c =>
                c.Id == id && c.TenantId == tenantId
            );

            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {id} not found");

            _mapper.Map(customerDto, customer);
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task DeleteCustomerAsync(Guid id, Guid tenantId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c =>
                c.Id == id && c.TenantId == tenantId
            );

            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {id} not found");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersByStatusAsync(
            CustomerStatus status,
            Guid tenantId
        )
        {
            var customers = await _context
                .Customers.Where(c => c.Status == status && c.TenantId == tenantId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
    }
}
