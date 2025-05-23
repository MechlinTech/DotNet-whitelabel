using System;
using backend.Models.CRM;

namespace backend.DTOs.CRM
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public CustomerStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCustomerDto
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public CustomerStatus Status { get; set; }
    }

    public class UpdateCustomerDto
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public CustomerStatus Status { get; set; }
    }
}
