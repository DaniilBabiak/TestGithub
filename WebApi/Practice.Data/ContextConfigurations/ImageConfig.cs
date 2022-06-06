using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practice.Entities.Entities;

namespace Practice.Data.ContextConfigurations
{
    public class ImageConfig : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(c => c.Id);


            builder.Property(c => c.EntityId).IsRequired();
            builder.Property(c => c.Path).IsRequired();
            builder.Property(c => c.ThumbnailPath).IsRequired();
        }
    }
}
