using System;

namespace Practice.Entities.Entities
{
    public class Challenge : IEntity
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public ChallengeType Type { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public int Reward { get; set; }

        public DateTime AvailableFrom { get; set; }

        public DateTime AvailableTo { get; set; }

        public Guid CreatorId { get; set; }

        public User Creator { get; set; }

        public ChallengeStatuses Status { get; set; }

    }
}
