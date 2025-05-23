using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.CRM
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? Company { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public CustomerStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        // Navigation properties
        public virtual ICollection<Contact>? Contacts { get; set; }
        public virtual ICollection<Deal>? Deals { get; set; }
    }

    public enum CustomerStatus
    {
        Lead,
        Prospect,
        Customer,
        Inactive,
    }
}
