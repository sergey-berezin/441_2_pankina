using ClassLibrary;
using OxyPlot;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    public class Evolution: INotifyPropertyChanged
    {
        public int citiesCount { get; set; } = 10;

        public int maxDistance { get; set; } = 100;

        public int[,]? distances { get; set; }

        public int populationSize { get; set; } = 70;

        public Population? population { get; set; }

        public int generationsCounter { get; set; }

        public double bestDistance { get; set; }

        public double meanDistance { get; set; }

        public List<double> bestDistanceScorer { get; set; }

        public List<double> meanDistanceScorer { get; set; }

        public PlotModel? plotModel { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand createDistancesCommand { get; private set; }
        public ICommand createPopulationCommand { get; private set; }
        public ICommand evolutionCommand { get; private set; }


        public Evolution()
        {
            createDistancesCommand = new Commands(o => { createRandomDistances_Execute(); });
            createPopulationCommand = new Commands(o => { createPopulation_Execute(); }, o => createPopulation_CanExecute());
            evolutionCommand = new Commands(o => { evolution_Execute(); }, o => evolution_CanExecute());
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void createRandomDistances_Execute()
        {
            createRandomDistances();
        }

        bool createPopulation_CanExecute()
        {
            return distances != null;
        }

        void createPopulation_Execute()
        {
            createPopulation();
            RaisePropertyChanged("generationsCounter");
            RaisePropertyChanged("bestDistance");
            RaisePropertyChanged("meanDistance");
        }

        bool evolution_CanExecute()
        {
            return distances != null && population != null;
        }

        void evolution_Execute()
        {
            evolution();
            RaisePropertyChanged("generationsCounter");
            RaisePropertyChanged("bestDistance");
            RaisePropertyChanged("meanDistance");
            drawDistanceScorer();
            RaisePropertyChanged("plotModel");
        }

        void evolution()
        {
            population.evolution(distances);
            generationsCounter++;
            bestDistance = population.getBestDistance();
            meanDistance = population.getMeanDistance();
            bestDistanceScorer.Add(bestDistance);
            meanDistanceScorer.Add(meanDistance);
        }

        void createPopulation()
        {
            population = new Population(populationSize, distances);
            generationsCounter=1;
            bestDistanceScorer = new List<double> ();
            meanDistanceScorer = new List<double>();
            bestDistance = population.getBestDistance();
            meanDistance = population.getMeanDistance();
            bestDistanceScorer.Add(bestDistance);
            meanDistanceScorer.Add(meanDistance);
        }

        void createRandomDistances()
        {
            int minDistance = maxDistance%2 == 0 ? maxDistance / 2: maxDistance / 2 + 1;

            Random random = new Random();

            distances = new int[citiesCount, citiesCount];

            for (int i = 0; i < citiesCount - 1; i++)
            {
                for (int j = i + 1; j < citiesCount; j++)
                {
                    distances[i, j] = random.Next(minDistance, maxDistance + 1);
                    distances[j, i] = distances[i, j];
                }
            }

            for (int i = 0; i < citiesCount - 1; i++)
            {
                for (int j = i + 1; j < citiesCount; j++)
                {
                    int currentMaxDistance = 2 * maxDistance;
                    int currentMinDistance = 0;
                    for (int k = 0; k < citiesCount; k++)
                    {
                        if (k != i && k != j)
                        {
                            int currentDistance = distances[i, k] + distances[k, j];
                            int currentDifference = Math.Abs(distances[i, k] - distances[k, j]);
                            currentMaxDistance = currentDistance < currentMaxDistance ? currentDistance : currentMaxDistance;
                            currentMinDistance = currentDifference > currentMinDistance ? currentDifference : currentMinDistance;
                        }
                    }
                    distances[i, j] = random.Next(currentMinDistance, currentMaxDistance + 1);
                    distances[j, i] = distances[i, j];
                }
            }
        }

        private void drawDistanceScorer()
        {
            try
            {
                var oxyPlotModel = new OxyPlotDistance(bestDistanceScorer, meanDistanceScorer);
                this.plotModel = oxyPlotModel.plotModel;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка в построении графика:\n" + ex.Message);
            }
        }

    }
}