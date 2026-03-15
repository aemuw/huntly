namespace Huntly.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;

        public ICollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();

        public User(string firstName, string lastName, string email, string passwordHash)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            UpdateTimestamp();
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdateTimestamp();
        }
    }
}
