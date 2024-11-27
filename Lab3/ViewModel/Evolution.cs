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
using Newtonsoft.Json.Linq;

namespace ViewModel
{
    public class Evolution: INotifyPropertyChanged
    {
        private readonly ICityMapRenderer cityMapRenderer;

        private readonly IWindowDialog windowDialog;

        private CancellationTokenSource tokenSource;

        public int citiesCount { get; set; } = 10;

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

        public ICommand createPopulationCommand { get; private set; }

        public ICommand loadPopulationCommand { get; private set; }

        public ICommand startEvolutionCommand { get; private set; }

        public ICommand evolutionCommand { get; private set; }

        public ICommand stopEvolutionCommand { get; private set; }

        public ICommand finishEvolutionCommand { get; private set; }

        public ICommand saveEvolutionCommand { get; private set; }


        public Evolution(ICityMapRenderer cityMapRenderer, IWindowDialog windowDialog)
        {
            this.cityMapRenderer = cityMapRenderer;
            this.windowDialog = windowDialog;
            this.tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();

            createPopulationCommand = new Commands(o => { createPopulation_Execute(); }, o => newPopulation_CanExecute());
            loadPopulationCommand = new Commands(o => { loadPopulation_Execute(); }, o => newPopulation_CanExecute());
            startEvolutionCommand = new AsyncCommands(async o => { await evolution_Execute(); }, o => startEvolution_CanExecute());
            evolutionCommand = new AsyncCommands(async o => { await evolution_Execute(); }, o => evolution_CanExecute());
            stopEvolutionCommand = new Commands(o => { stopEvolution_Execute(); }, o => stopEvolution_CanExecute());
            finishEvolutionCommand = new Commands(o => { finishEvolution_Execute(); }, o => finishEvolution_CanExecute());
            saveEvolutionCommand = new Commands(o => { saveEvolution_Execute(); }, o => saveEvolution_CanExecute());
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool newPopulation_CanExecute()
        {
            return distances == null && population == null ;
        }

        void createPopulation_Execute()
        {
            createRandomDistances();
            createPopulation();
            RaisePropertyChanged("citiesCount");
            RaisePropertyChanged("populationSize");
            RaisePropertyChanged("generationsCounter");
            RaisePropertyChanged("bestDistance");
            RaisePropertyChanged("meanDistance");
        }

        void loadPopulation_Execute()
        {
            string path = "C:\\Users\\simal\\Documents\\C#\\441_2_pankina\\Lab3\\test.json";
            string populationJson = File.ReadAllText(path);
            population = JsonConvert.DeserializeObject<Population>(populationJson);

            citiesCount = population.citiesCount;
            populationSize = population.populationSize;
            generationsCounter = population.generationsCounter;
            distances = population.distances;
            bestDistance = population.getBestDistance();
            meanDistance = population.getMeanDistance();
            bestDistanceScorer = new List<double>();
            meanDistanceScorer = new List<double>();
            bestDistanceScorer.Add(bestDistance);
            meanDistanceScorer.Add(meanDistance);

            RaisePropertyChanged("citiesCount");
            RaisePropertyChanged("populationSize");
            RaisePropertyChanged("generationsCounter");
            RaisePropertyChanged("bestDistance");
            RaisePropertyChanged("meanDistance");
        }

        bool startEvolution_CanExecute()
        {
            return distances != null && population != null && generationsCounter == 1;
        }

        bool evolution_CanExecute()
        {
            return distances != null && population != null && generationsCounter > 1 && tokenSource.Token.IsCancellationRequested;
        }

        async Task evolution_Execute()
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
                    evolution();
                    RaisePropertyChanged("generationsCounter");
                    RaisePropertyChanged("bestDistance");
                    RaisePropertyChanged("meanDistance");
                    drawDistanceScorer();
                    RaisePropertyChanged("plotModel");
                    await drawBestRoute();
                    Thread.Sleep(300);
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        bool stopEvolution_CanExecute()
        {
            return distances != null && population != null && generationsCounter > 1 && !tokenSource.Token.IsCancellationRequested; ;
        }

        void stopEvolution_Execute()
        {
            tokenSource.Cancel();
        }

        bool finishEvolution_CanExecute()
        {
            return distances != null && population != null && tokenSource.Token.IsCancellationRequested;
        }

        void finishEvolution_Execute()
        {
            tokenSource.Cancel();
            distances = null;
            population = null;
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

            int maxDistance = 100;
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