using Lab2.Warehouse.Entities.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Lab2.Warehouse.Entities {
    [BsonIgnoreExtraElements]
    public class Ingredient : Entity {
        public string Name     { get; set; }
        public float  Calories { get; set; }
        public float  Fats     { get; set; }
        public float  Carbs    { get; set; }
        public float  Protein  { get; set; }
    }
}
