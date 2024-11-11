using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using ClassLibrary;
using OxyPlot.Series;
using OxyPlot.Legends;

namespace ViewModel
{
    public class OxyPlotDistance
    {
        public PlotModel plotModel { get; private set; }

        public List<double> bestDistanceScorer { get; private set; }

        public List<double> meanDistanceScorer { get; private set; }

        public OxyPlotDistance(List<double> bestDistanceScorer, List<double> meanDistanceScorer)
        {
            this.bestDistanceScorer = bestDistanceScorer;
            this.meanDistanceScorer = meanDistanceScorer;
            this.plotModel = new PlotModel { Title = "Значение метрики приспособленности" };
            this.distanceScorerPlot();
        }

        public void distanceScorerPlot()
        {
            LineSeries lineSeries = new LineSeries();

            for (int i=0; i<bestDistanceScorer.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i+1, bestDistanceScorer[i]));
            }

            lineSeries.Title = "Длина лучшего маршрута";
            lineSeries.Color = OxyColors.Purple;
            lineSeries.MarkerType = MarkerType.Circle;
            lineSeries.MarkerSize = 2;

            Legend leg = new Legend();
            this.plotModel.Legends.Add(leg);

            this.plotModel.Series.Add(lineSeries);

            lineSeries = new LineSeries();

            for (int i = 0; i < meanDistanceScorer.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i + 1, meanDistanceScorer[i]));
            }

            lineSeries.Title = "Средняя длина маршрута";
            lineSeries.Color = OxyColors.DarkGreen;
            lineSeries.MarkerType = MarkerType.Circle;
            lineSeries.MarkerSize = 2;

            this.plotModel.Series.Add(lineSeries);
        }
    }
}
