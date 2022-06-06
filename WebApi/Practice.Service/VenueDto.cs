using System;
using System.Collections.Generic;

namespace Practice.Service
{
    public class VenueDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CreatorId { get; set; }
        public UserDto Creator { get; set; }
        public DateTime Date { get; set; }
        public LocationDto Location { get; set; }
        public int Distance { get; set; }
        public IEnumerable<UserDto> Attendees { get; set; }
    }
}
