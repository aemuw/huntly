using Huntly.Domain.Enums;

namespace Huntly.Domain.Entities
{
    public class JobApplication : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? JobUrl { get; private set; }
        public ApplicationStatus Status { get; private set; }
        public Priority Priority { get; private set; }
        public decimal? SalaryFrom { get; private set; }
        public decimal? SalaryTo { get; private set; }
        public DateTime? AppliedDate { get; private set; }
        public DateTime? DueDate { get; private set; }
        public string? Notes { get; private set; }

        public Guid UserId { get; private set; }
        public Guid CompanyId { get; private set; }

        public User? User { get; private set; }
        public Company? Company { get; private set; }

        public ICollection<Technology> Technologies { get; private set; } = new List<Technology>();

        public ICollection<Interview> Interview { get; private set; } = new List<Interview>();
        public JobApplication(string title, Guid userId, Guid companyId, Priority priority)
        {
            Title = title;
            UserId = userId;
            CompanyId = companyId;
            Priority = priority;
            Status = ApplicationStatus.Watchlist;
        }

        public void ChangeStatus(ApplicationStatus newStatus)
        {
            Status = newStatus;

            if (newStatus == ApplicationStatus.Applied && AppliedDate == null)
                AppliedDate = DateTime.UtcNow;

            UpdateTimestamp();
        }

        public void Update(string title, string? description, string? jobUrl, Priority priority, decimal? salaryFrom, decimal? salaryTo, DateTime? dueDate, string? notes)
        {
            Title = title;
            Description = description;
            JobUrl = jobUrl;
            Priority = priority;
            SalaryFrom = salaryFrom;
            SalaryTo = salaryTo;
            DueDate = dueDate;
            Notes = notes;
            UpdateTimestamp();
        }

        public void AddTechonology(Technology technology)
        {
            if (!Technologies.Contains(technology))
                Technologies.Add(technology);
            UpdateTimestamp();
        }
    }
}
