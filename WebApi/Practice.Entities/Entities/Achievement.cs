using System;
using System.Collections.Generic;

namespace Practice.Entities.Entities
{
    public class Achievement : IEntity
    {
        public Guid Id { get; set; }
        public Guid ChallengeTypeId { get; set; }
        public string Name { get; set; }
        public ChallengeType ChallengeType { get; set; }
        public uint Streak { get; set; }
        public ICollection<User> Users { get; set; }
        public Achievement(Guid id, Guid challengeTypeId, string name, uint streak)
        {
            Id = id;
            ChallengeTypeId = challengeTypeId;
            Name = name;
            Streak = streak;
        }

        public Achievement(Guid challengeTypeId, string name, uint streak)
            : this(Guid.NewGuid(), challengeTypeId, name, streak)
        {
        }
    }
}
