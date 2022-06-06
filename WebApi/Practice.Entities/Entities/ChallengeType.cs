using System;

namespace Practice.Entities.Entities
{
    public class ChallengeType : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
