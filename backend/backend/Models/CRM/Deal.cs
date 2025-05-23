using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.CRM
{
    public class Deal
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        public DealStage Stage { get; set; }

        public DateTime ExpectedCloseDate { get; set; }

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

    public enum DealStage
    {
        Lead,
        Qualification,
        Proposal,
        Negotiation,
        ClosedWon,
        ClosedLost,
    }
}
