using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practice.Entities.Entities;

namespace Practice.Data.ContextConfigurations
{
    public class ChallengeConfig : IEntityTypeConfiguration<Challenge>
    {
        public void Configure(EntityTypeBuilder<Challenge> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.TypeId).IsRequired();
            builder.Property(c => c.Description).IsRequired();
            builder.Property(c => c.Reward).IsRequired();
            builder.Property(c => c.AvailableFrom).IsRequired();
            builder.Property(c => c.Status).IsRequired();
        }
    }
}
