using System;
using backend.Models.CRM;

namespace backend.DTOs.CRM
{
    public class DealDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Value { get; set; }
        public DealStage Stage { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class CreateDealDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Value { get; set; }
        public DealStage Stage { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class UpdateDealDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Value { get; set; }
        public DealStage Stage { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
    }
}
