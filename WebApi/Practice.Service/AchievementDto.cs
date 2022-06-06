using System;

namespace Practice.Service
{
    public class AchievementDto
    {
        public Guid? Id { get; init; }
        public string Name { get; init; }
        public string ImagePath { get; set; }
        public string ThumbnailPath { get; set; }
        public Guid TypeId { get; set; }
        public string TypeName { get; set; }
        public uint Streak { get; set; }
    }
}
