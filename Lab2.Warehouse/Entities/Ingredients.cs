using System;
using System.Linq.Expressions;
using Lab2.Warehouse.Entities.Abstractions;

namespace Lab2.Warehouse.Entities {
    public class Ingredient : Entity {
        public string Name     { get; set; }
        public float  Calories { get; set; }
        public float  Fats     { get; set; }
        public float  Carbs    { get; set; }
        public float  Protein  { get; set; }
    }

    public class IngredientMapping : Cassandra.Mapping.Mappings {
        public IngredientMapping() {
            For<Ingredient>()
                .TableName("ingredients")
                .PartitionKey(i => i.Id)
                .ClusteringKey(i => i.Id);
        }

        public static Expression<Func<Ingredient, Ingredient>> UpdateExpression(Ingredient ingredient) =>
            i => new Ingredient {
                Calories = ingredient.Calories,
                Carbs    = ingredient.Carbs,
                Fats     = ingredient.Fats,
                Name     = ingredient.Name,
                Protein  = ingredient.Protein
            };
    }
}
