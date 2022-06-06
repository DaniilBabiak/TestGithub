using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practice.Entities.Entities;

namespace Practice.Data.ContextConfigurations
{
    public class VenueConfig : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.HasKey(v => v.Id);

            builder.HasOne(v => v.Location);
            builder.Property(v => v.LocationId).IsRequired();

            builder.HasOne(v => v.Creator);
            builder.Property(v => v.CreatorId).IsRequired();

            builder.Property(v => v.Description).IsRequired();
        }
    }
}
