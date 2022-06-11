using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module2_HomeWork2
{
    public class Salad
    {
        public Vegetable[] Vegetables { get; private set; } = new Vegetable[0];

        public Salad() { }
        public Salad(params Vegetable[] vegetables)
        {
            Vegetables = vegetables;
        }
        public void AddVegetable(Vegetable vegetable)
        {
            var oldArray = Vegetables;
            Vegetables = new Vegetable[oldArray.Length + 1];
            for (int i = 0; i < oldArray.Length; i++)
            {
                Vegetables[i] = oldArray[i];
            }
            Vegetables[Vegetables.Length - 1] = vegetable;
        }
        public double GetCalories()
        {
            double result = 0;
            foreach (var vegetable in Vegetables)
            {
                result += vegetable.CountCalories();
            }
            return result;
        }
        public Vegetable[] GetVegetablesWithCaloriesLessThan(double calories)
        {
            var result = new Vegetable[Vegetables.Length];
            int i = 0;
            foreach (var vegetable in Vegetables)
            {
                if (vegetable.CountCalories() < calories)
                {
                    result[i] = vegetable;
                    i++;
                }
            }
            return result;
        }
    }
}
