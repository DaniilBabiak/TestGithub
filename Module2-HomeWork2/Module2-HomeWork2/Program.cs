namespace Module2_HomeWork2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var salad = new Salad();
            var tomato = new Tomato(10, 2, 1, 4, 60);
            var cucumber = new Cucumber(13, 3, 2, 4, 55);
            salad.AddVegetable(tomato);
            salad.AddVegetable(cucumber);
            Console.WriteLine($"Всего калорий в салате: {salad.GetCalories()}");
            var test = salad.GetVegetablesWithCaloriesLessThan(8);
        }
    }
}