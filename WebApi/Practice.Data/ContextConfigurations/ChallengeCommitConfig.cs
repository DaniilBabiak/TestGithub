using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practice.Entities.Entities;

namespace Practice.Data.ContextConfigurations
{
    public class ChallengeCommitConfig : IEntityTypeConfiguration<ChallengeCommit>
    {
        public void Configure(EntityTypeBuilder<ChallengeCommit> builder)
        {
            builder.HasKey(c => c.Id);

            builder
                .HasOne(c => c.UserChallenge)
                .WithMany()
                .HasForeignKey(c => c.UserChallengeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            builder
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();


            builder.Property(c => c.UserId).IsRequired();
            builder.Property(c => c.UserChallengeId).IsRequired();
            builder.Property(c => c.ScreenshotPath).IsRequired();
            builder.Property(c => c.CommitDateTime).IsRequired();
            builder.Property(c => c.Status).IsRequired();
        }
    }
}
