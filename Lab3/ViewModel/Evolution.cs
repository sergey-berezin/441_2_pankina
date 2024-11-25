using ClassLibrary;
using OxyPlot;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;
using System.Numerics;

namespace ViewModel
{
    public class Evolution: INotifyPropertyChanged
    {
        private readonly ICityMapRenderer cityMapRenderer;

        private readonly IWindowDialog windowDialog;

        private CancellationTokenSource tokenSource;

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
        public ICommand startEvolutionCommand { get; private set; }
        public ICommand finishEvolutionCommand { get; private set; }

        public ICommand saveEvolutionCommand { get; private set; }

        public ICommand loadPopulationCommand { get; private set; }


        public Evolution(ICityMapRenderer cityMapRenderer, IWindowDialog windowDialog)
        {
            this.cityMapRenderer = cityMapRenderer;
            this.windowDialog = windowDialog;
            this.tokenSource = new CancellationTokenSource();
            createDistancesCommand = new Commands(o => { createRandomDistances_Execute(); });
            createPopulationCommand = new Commands(o => { createPopulation_Execute(); }, o => createPopulation_CanExecute());
            evolutionCommand = new Commands(o => { evolution_Execute(); }, o => evolution_CanExecute());
            startEvolutionCommand = new AsyncCommands(async o => { await startEvolution_Execute(); }, o => evolution_CanExecute());
            finishEvolutionCommand = new Commands(o => { finishEvolution_Execute(); }, o => finishEvolution_CanExecute());
            saveEvolutionCommand = new Commands(o => { saveEvolution_Execute(); }, o => saveEvolution_CanExecute());
            loadPopulationCommand = new Commands(o => { loadPopulation_Execute(); });
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void createRandomDistances_Execute()
        {
            createRandomDistances();
            //cityMapRenderer.createVertexPositions(citiesCount);
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
            //return distances != null && population != null;
            return population != null;
        }

        async Task startEvolution_Execute()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    await evolution_Execute();
                    Thread.Sleep(1000);
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        async Task evolution_Execute()
        {
            evolution();
            RaisePropertyChanged("generationsCounter");
            RaisePropertyChanged("bestDistance");
            RaisePropertyChanged("meanDistance");
            drawDistanceScorer();
            RaisePropertyChanged("plotModel");
            await drawBestRoute();
        }

        bool finishEvolution_CanExecute()
        {
            return distances != null && population != null;
        }

        void finishEvolution_Execute()
        {
            tokenSource.Cancel();
            //tokenSource = new CancellationTokenSource();
        }

        bool saveEvolution_CanExecute()
        {
            return distances != null && population != null && tokenSource.Token.IsCancellationRequested;
        }

        void saveEvolution_Execute()
        {
            //string populationJson = JsonConvert.SerializeObject(population);
            //string path = "C:\\Users\\simal\\Documents\\C#\\441_2_pankina\\Lab3\\test.json";
            //File.WriteAllText(path, populationJson);
            //PasswordWindow passwordWindow = new PasswordWindow();

            windowDialog.SaveExperiment();
        }

        void loadPopulation_Execute()
        {
            string path = "C:\\Users\\simal\\Documents\\C#\\441_2_pankina\\Lab3\\test.json";
            string populationJson = File.ReadAllText(path);
            population = JsonConvert.DeserializeObject<Population>(populationJson);
            generationsCounter = population.generationsCounter;
            distances = population.distances;
            bestDistance = population.getBestDistance();
            meanDistance = population.getMeanDistance();
            bestDistanceScorer = new List<double>();
            meanDistanceScorer = new List<double>();
            RaisePropertyChanged("generationsCounter");
            RaisePropertyChanged("bestDistance");
            RaisePropertyChanged("meanDistance");
        }

        void evolution()
        {
            population.evolution();
            generationsCounter = population.generationsCounter;
            bestDistance = population.getBestDistance();
            meanDistance = population.getMeanDistance();
            bestDistanceScorer.Add(bestDistance);
            meanDistanceScorer.Add(meanDistance);
        }

        void createPopulation()
        {
            population = new Population(populationSize, distances);
            generationsCounter = population.generationsCounter;
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

        private async Task drawBestRoute()
        {
            //try
            //{
            List<int> bestRoute = population.getBestRoutes()[0].route;
            await cityMapRenderer.RenderRoads(citiesCount, bestRoute, distances);
            await cityMapRenderer.RenderCities(citiesCount, bestRoute);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка в построении графика:\n" + ex.Message);
            //}
        }

    }
}