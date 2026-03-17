namespace Huntly.Application.DTOs.Company
{
    public class CompanyResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? LinkedIn { get; set; }
        public string? Notes { get; set; }
        public List<string> Technologies { get; set; } = new();
    }
}
