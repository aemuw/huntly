using Huntly.Domain.Entities;
using Huntly.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huntly.Infrastructure.Persistence.Configurations
{
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.Result)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.ScheduledAt)
                .IsRequired();
        }
    }
}
