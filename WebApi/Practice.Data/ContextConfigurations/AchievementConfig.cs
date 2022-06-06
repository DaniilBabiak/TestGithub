using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practice.Entities.Entities;

namespace Practice.Data.ContextConfigurations
{
    public class AchievementConfig : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.HasKey(c => c.Id);

            builder
                .HasOne(c => c.ChallengeType)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(c => c.Streak).IsRequired();
            builder.Property(c => c.ChallengeTypeId).IsRequired();
        }
    }
}
