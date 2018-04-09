using System;

namespace QuotingBot.Models.Motor
{
    [Serializable]
    public class Vehicle
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string BodyType { get; set; }
        public int NumberOfDoors { get; set; }
        public int YearOfFirstManufacture { get; set; }
        public int EngineCapacity { get; set; }
        public string FuelType { get; set; }
        public string Description { get; set; }
        public string AbiCode { get; set; }
    }
}