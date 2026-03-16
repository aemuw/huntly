using Huntly.Domain.Entities;
using Huntly.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huntly.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(c => c.Size)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(c => c.Website)
                .HasMaxLength(500);

            builder.Property(c => c.LinkedIn)
                .HasMaxLength(500);

            builder.HasMany(c => c.Technologies)
                .WithMany()
                .UsingEntity("CompanyTechnologies");
        }
    }
}
