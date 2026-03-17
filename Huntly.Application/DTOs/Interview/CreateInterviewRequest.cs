using Huntly.Domain.Enums;

namespace Huntly.Application.DTOs.Interview
{
    public class CreateInterviewRequest
    {
        public InterviewType Type { get; set; }
        public DateTime ScheduledAt { get; set; }
    }
}
