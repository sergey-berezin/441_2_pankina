using System;
using System.Collections;
using System.Diagnostics;

namespace ClassLibrary
{
    public class Chromosome
    {
        public List<int> route { get; set; }

        public int citiesCount { get; set; }

        public double fitness { get; set; }

        public Chromosome()
        {
        }

        public Chromosome (int citiesCount, int[,] distances)
        {
            this.citiesCount = citiesCount;
            route = Enumerable.Range(1, citiesCount).ToList();
            createRandomRoute(citiesCount);
            fitness = getDistance(distances);
        }

        public Chromosome (List<int> route, int citiesCount, int[,] distances)
        {
            this.citiesCount = citiesCount;
            this.route = route.ToList();
            mutate();
            fitness = getDistance(distances);
        }

        public Chromosome(List<int> route1, List<int> route2, int citiesCount, int[,] distances)
        {
            this.citiesCount = citiesCount;
            route = new List<int>();
            cross(route1, route2);
            fitness = getDistance(distances);
        }

        public string chromosomeToString()
        {
            string res = "";
            res += "Route: ";
            foreach (var item in route) res+=$"{item} -> ";
            res += $"{route[0]}\n";
            res += $"Fitness: {fitness}\n\n";
            return res;
        }

        public void createRandomRoute(int citiesCount)
        {
            Random random = new Random();
            for (int i = citiesCount - 1; i >= 0; --i)
            {
                int j = random.Next(i + 1);
                (route[i], route[j]) = (route[j], route[i]);
            }
        }

        public double getDistance(int[,] distances)
        {
            double routeDistance = 0;
            for (int i = 0 ; i < citiesCount - 1 ; i++)
            {
                routeDistance += distances[route[i] - 1, route[i+1] - 1];
            }
            routeDistance += distances[route[0]-1, route[citiesCount - 1] - 1];
            return routeDistance;
        }

        private void mutate()
        {
            Random random = new Random();
            int i = random.Next(citiesCount);
            int j = random.Next(citiesCount);
            int tmp = route[i];
            route[i] = route[j];
            route[j] = tmp;
        }

        private void cross(List<int> route1, List<int> route2)
        {
            Random random = new Random();
            int section = random.Next(1, citiesCount-1);
            route = route1.Take(section).ToList();

            foreach (var item in route2)
                if (!route.Contains(item))
                    route.Add(item);
        }
    }
}