using System;
using System.Linq.Expressions;

namespace Lab2.Warehouse.Entities.Mappings {
    public class IngredientMapping : Cassandra.Mapping.Mappings {
        public IngredientMapping() {
            For<Ingredient>().TableName("ingredients").PartitionKey(i => i.Id);
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
