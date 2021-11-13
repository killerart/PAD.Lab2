using System;
using System.Linq.Expressions;
using Lab2.Warehouse.Domain.Entities;

namespace Lab2.Warehouse.Infrastructure.Mappings {
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
