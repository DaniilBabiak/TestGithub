using System;

namespace Practice.Service
{
    public class ChallengeDto
    {
        public Guid? Id { get; set; }
        public string TypeId { get; set; }
        public string TypeName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? Reward { get; set; }

        public DateTime AvailableFrom { get; set; }

        public DateTime AvailableTo { get; set; }

        public string CreatorId { get; set; }

        public string CreatorName { get; set; }

        public string Status { get; set; }

        public string ImagePath { get; set; }
        public string ThumbnailPath { get; set; }
    }
}
