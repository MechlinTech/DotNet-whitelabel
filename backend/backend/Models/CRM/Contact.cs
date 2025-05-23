using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.CRM
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        // Navigation property
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}
