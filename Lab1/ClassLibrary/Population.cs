using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Population
    {
        public List<Chromosome> routes { get; set; }

        public int citiesCount { get; set; }

        public int populationSize { get; set; }

        public Population(int populationSize, double[,] distances)
        {
            this.populationSize = populationSize;

            citiesCount = distances.GetLength(0);

            this.routes = new List<Chromosome> ();
            createPopulation(populationSize, distances);

            sortPopulation(routes);
        }

        public string populationToString()
        {
            string res = "";
            foreach (var item in routes)
            {
                res+=item.chromosomeToString();
            }
            return res;
        }

        public string bestRoutesToString()
        {
            string res = "";
            foreach (var item in getBestRoutes())
            {
                item.route.ForEach(elem => res += $"{elem} -> ");
                res += $"{item.route[0]}\n";
            }
            return res;
        }

        public void evolution(double[,] distances)
        {
            Random random = new Random();

            List<Chromosome> newRoutes = routes.ToList();

            foreach (var mutant in routes)
                if (random.Next(101) <= 30)
                    newRoutes.Add(new Chromosome(mutant.route, citiesCount, distances));

            int parent1Index = random.Next(populationSize); ;
            int parent2Index = random.Next(populationSize); ;
            while (parent1Index == parent2Index)
            {
                parent1Index = random.Next(populationSize);
                parent2Index = random.Next(populationSize);
            }

            newRoutes.Add(new Chromosome(routes[parent1Index].route, routes[parent2Index].route, citiesCount, distances));

            sortPopulation(newRoutes);
            newRoutes.RemoveRange(populationSize, newRoutes.Count - populationSize);

            routes = newRoutes;
        }

        public void createPopulation(int populationSize, double[,] distances)
        {
            for (int i = 0; i < populationSize; i++)
            {
                routes.Add(new Chromosome(citiesCount, distances));
            }
        }

        public void sortPopulation(List<Chromosome> routes)
        {
            routes.Sort((b1, b2) => b1.fitness.CompareTo(b2.fitness));
        }

        public double getBestDistance()
        {
            return routes.Min(elem => elem.fitness);
        }

        public double getMeanDistance()
        {
            return routes.Average(elem => elem.fitness);
        }

        public List<Chromosome> getBestRoutes()
        {
;           double bestFitness = getBestDistance();
            List<Chromosome> bestRoutes = new List<Chromosome>();
            foreach (var item in routes)
                if (item.fitness == bestFitness)
                    bestRoutes.Add(item);
            return bestRoutes;
        }       

    }
}


