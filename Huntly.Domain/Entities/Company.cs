using Huntly.Domain.Enums;

namespace Huntly.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public CompanyType Type { get; private set; }
        public CompanySize Size { get; private set; }
        public string? Website { get; private set; }
        public string? LinkedIn { get; private set; }
        public string? Notes { get; private set; }
        public ICollection<Technology> Technologies { get; private set; } = new List<Technology>();
        public Company(string name, CompanyType type, CompanySize size)
        {
            Name = name;
            Type = type;
            Size = size;
        }

        private void AddTechnology(Technology technology)
        {
            if (!Technologies.Contains(technology))
                Technologies.Add(technology);
            UpdateTimestamp();
        }
        
        public void RemoveTechnology(Technology technology)
        {
            Technologies.Remove(technology);
            UpdateTimestamp();
        }

        public void Update(string name, CompanyType type, CompanySize size, string? website, string? linkedin, string? notes)
        {
            Name = name;
            Type = type;
            Size = size;
            Website = website;
            LinkedIn = linkedin;
            Notes = notes;
            UpdateTimestamp();
        }
    }
}

