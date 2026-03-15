using Huntly.Domain.Enums;

namespace Huntly.Domain.Entities
{
    public class Interview : BaseEntity
    {
        public InterviewType Type { get; private set; }
        public InterviewResult Result { get; private set; }
        public DateTime ScheduledAt { get; private set; }
        public string? Notes { get; private set; }
        public Guid JobApplicationId { get; private set; }
        public JobApplication? JobApplication { get; private set; }

        public Interview(Guid jobApplicationId, InterviewType type, DateTime scheduledAt)
        {
            JobApplicationId = jobApplicationId;
            Type = type;
            ScheduledAt = scheduledAt;
            Result = InterviewResult.Pending;
        }

        public void SetResult(InterviewResult result, string? notes)
        {
            Result = result;
            Notes = notes;
            UpdateTimestamp();
        }

        public void Reschedule(DateTime newDate)
        {
            ScheduledAt = newDate;
            UpdateTimestamp();
        }
    }
}
