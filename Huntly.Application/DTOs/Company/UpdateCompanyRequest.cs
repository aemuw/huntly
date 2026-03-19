using Huntly.Domain.Enums;

namespace Huntly.Application.DTOs.Company
{
    public class UpdateCompanyRequest
    {
        public string Name { get; set; } = string.Empty;
        public CompanyType Type { get; set; }
        public CompanySize Size { get; set; }
        public string? Website { get; set; }
        public string? LinkedIn { get; set; }
        public string? Notes { get; set; }
    }
}
