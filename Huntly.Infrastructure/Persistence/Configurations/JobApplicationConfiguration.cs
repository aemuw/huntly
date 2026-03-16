using Microsoft.EntityFrameworkCore;
using Huntly.Domain.Entities;
using Huntly.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huntly.Infrastructure.Persistence.Configurations
{
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(j => j.Priority)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(j => j.SalaryFrom)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.SalaryTo)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.JobUrl)
                .HasMaxLength(1000);

            builder.HasOne(j => j.Company)
                .WithMany()
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(j => j.Technologies)
                .WithMany()
                .UsingEntity("JobApplicationTechnologies");

            builder.HasMany(j => j.Interview)
                .WithOne(i => i.JobApplication)
                .HasForeignKey(i => i.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
