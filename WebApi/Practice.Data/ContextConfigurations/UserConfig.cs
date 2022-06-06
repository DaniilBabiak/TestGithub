using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practice.Entities.Entities;

namespace Practice.Data.ContextConfigurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(u => u.Achievements).WithMany(a => a.Users);
            builder.HasMany(u => u.Venues).WithMany(v => v.Attendees);
        }
    }
}
