using Lab2.Warehouse.Domain.Entities.Abstractions;

namespace Lab2.Warehouse.Domain.Entities {
    public class Ingredient : Entity {
        public string Name     { get; set; }
        public float  Calories { get; set; }
        public float  Fats     { get; set; }
        public float  Carbs    { get; set; }
        public float  Protein  { get; set; }
    }
}
