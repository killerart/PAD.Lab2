﻿using System.ComponentModel.DataAnnotations;

namespace Lab2.Warehouse.Models {
    public class CreateIngredientModel {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Calories { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Fats { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Carbs { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Protein { get; set; }
    }
}
