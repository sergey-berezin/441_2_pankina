using ClassLibrary;
using System.Runtime.InteropServices;


int POPULATION_SIZE = 10;
//int MAX_GENERATIONS = 10;


// 1 -> 3 -> 5 -> 2 -> 4 -> 1
// 15
double[,] distances =
{
    {0, 6, 1, 5, 10 },
    {6, 0, 7, 4, 3 },
    {1, 7, 0, 9, 2 },
    {5, 4, 9, 0, 8 },
    {10, 3, 2, 8, 0 },
};

// 1 -> 4 -> 2 -> 6 -> 3 -> 5 -> 1
// 21
double[,] distances_1 =
{
    {0, 8, 11, 1, 6, 7 },
    {8, 0, 9, 2, 12, 3 },
    {11, 9, 0, 10, 5, 4 },
    {1, 2, 10, 0, 11, 9 },
    {6, 12, 5, 11, 0, 12 },
    {7, 3, 4, 9, 12, 0 },
};

Population myPopulation = new Population(POPULATION_SIZE, distances_1);


//Console.CancelKeyPress += (sender, args) =>
//{
//    args.Cancel = true;
//    //Console.WriteLine(args.SpecialKey);
//    flag = false;
//};


ConsoleKeyInfo cki;

int generationsCount = 1;

//for (int i = 0; i < MAX_GENERATIONS; i++)
while (true)
{
    Console.WriteLine($"Generation: {generationsCount++}\n");

    //Console.Write(myPopulation.populationToString());

    Console.WriteLine($"Best distance: {myPopulation.getBestDistance()}");
    Console.WriteLine($"Mean distance : {myPopulation.getMeanDistance()}\n");

    Console.WriteLine($"Best routes:\n");

    Console.Write(myPopulation.bestRoutesToString());

    Console.WriteLine($"---------------------------------------------------------------");

    //System.Threading.Thread.Sleep(1000);

    cki = Console.ReadKey(true);
    if (cki.Key == ConsoleKey.Spacebar) break;

    myPopulation.evolution(distances_1);
}