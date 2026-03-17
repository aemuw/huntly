using Huntly.Domain.Enums;

namespace Huntly.Application.DTOs.Interview
{
    public class InterviewResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string? Notes { get; set; }
    }
}
