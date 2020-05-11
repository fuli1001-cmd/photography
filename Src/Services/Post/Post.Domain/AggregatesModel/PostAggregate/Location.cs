using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class Location : ValueObject
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }

        private Location() { }

        public Location(string name, string address, double latitude, double longitude)
        {
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Address;
            yield return Latitude;
            yield return Longitude;
        }
    }
}
