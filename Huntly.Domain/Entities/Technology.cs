namespace Huntly.Domain.Entities
{
    public class Technology : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Category { get; private set; } = string.Empty;

        public Technology(string name, string category)
        {
            Name = name;
            Category = category;
        }

        public void Update(string name, string category)
        {
            Name = name;
            Category = category;
            UpdateTimestamp();
        }
    }
}
