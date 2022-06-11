using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module2_HomeWork2
{
    public abstract class Vegetable
    {
        public double Calories { get; init; }
        public double Proteins { get; init; }
        public double Fats { get; init; }
        public double Carbohydrates { get; init; }
        public double Weight { get; init; }

        public Vegetable(double calories, double proteins, double fats, double carbohydrates, double weight)
        {
            Calories = calories;
            Proteins = proteins;
            Fats = fats;
            Carbohydrates = carbohydrates;
            Weight = weight;
        }

        public double CountCalories()
        {
            return Calories * Weight / 100;
        }
    }

}
