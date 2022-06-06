using Practice.Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Practice.Entities.Entities
{
    public class Venue : IEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public User Creator { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime Date { get; set; }
        public Location Location { get; set; }
        public Guid LocationId { get; set; }
        public List<User> Attendees { get; set; }

        public double GetDistance(Location location)
        {
            var distance = Location.Coordinates.ProjectTo(2855).Distance(location.Coordinates.ProjectTo(2855));
            return distance;
        }
    }
}
