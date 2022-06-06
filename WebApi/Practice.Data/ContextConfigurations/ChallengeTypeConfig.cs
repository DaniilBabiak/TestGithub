using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practice.Entities.Entities;

namespace Practice.Data.ContextConfigurations
{
    public class ChallengeTypeConfig : IEntityTypeConfiguration<ChallengeType>
    {
        public void Configure(EntityTypeBuilder<ChallengeType> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired();
        }
    }
}
