using NetTopologySuite.Geometries;
using System;

namespace Practice.Entities.Entities
{
    public class Location : IEntity
    {
        public Guid Id { get; set; }
        public Point Coordinates { get; set; }
    }
}
