using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practice.Entities.Entities;

namespace Practice.Data.ContextConfigurations
{
    public class UserChallengeConfig : IEntityTypeConfiguration<UserChallenge>
    {
        public void Configure(EntityTypeBuilder<UserChallenge> builder)
        {
            builder.HasKey(u => u.Id);

            builder
                .HasOne(u => u.Challenge)
                .WithMany()
                .HasForeignKey(u => u.ChallengeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(u => u.UserId).IsRequired();
            builder.Property(u => u.ChallengeId).IsRequired();
            builder.Property(u => u.StartedAt).IsRequired();
            builder.Property(u => u.EndedAt).IsRequired(false);
            builder.Property(u => u.ApproverId).IsRequired(false);
            builder.Property(u => u.ApprovedAt).IsRequired(false);
        }
    }
}
