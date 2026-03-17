using Huntly.Domain.Enums;

namespace Huntly.Application.DTOs.JobApplication
{
    public class ChangeStatusRequest
    {
        public ApplicationStatus Status { get; set; }
    }
}
