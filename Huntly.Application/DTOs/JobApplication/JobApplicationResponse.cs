using Huntly.Domain.Enums;

namespace Huntly.Application.DTOs.JobApplication
{
    public class JobApplicationResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ? JobUrl { get; set; }
        public ApplicationStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public DateTime? AppliedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
}
