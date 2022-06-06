using System;

namespace Practice.Entities.Entities
{
    public class Image : IEntity
    {
        public Guid Id { get; set; }

        public string Path { get; set; }
        public string ThumbnailPath { get; set; }

        public Guid EntityId { get; set; }
    }
}
