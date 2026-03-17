using Huntly.Domain.Enums;

namespace Huntly.Application.DTOs.JobApplication
{
    public class UpdateJobApplicationRequest
    {
        public string Title { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public string? Description { get; set; }
        public string? JobUrl { get; set; }
        public string? Notes { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
