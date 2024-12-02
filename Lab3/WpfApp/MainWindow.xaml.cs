using ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ViewModel;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using static WpfApp.MainWindow;
using System.Xml.Linq;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            this.DataContext = new Evolution(new CanvasRoute(graphCanvas), new SaveExperimentDialog(), new MessageBoxErrorSender());
        }


        public class SaveExperimentDialog: IWindowDialog
        {
            public string openWindowDialog()
            {

                SaveExperimentDialogWindow dialogWindow = new SaveExperimentDialogWindow();

                if (dialogWindow.ShowDialog() == true)
                {
                    return dialogWindow.expеrimentName;
                }
                else
                {
                    MessageBox.Show("Эксперимент не сохранен");
                    return null;
                }
            }
        }


        public class MessageBoxErrorSender : IErrorSender
        {
            public void SendError(string message) => MessageBox.Show(message);
        }


        public class CanvasRoute: ICityMapRenderer
        {
            public Canvas graphCanvas { get; private set; }

            private Point[] vertexPositions; // Позиции вершин на плоскости

            public CanvasRoute(Canvas graphCanvas)
            {
                this.graphCanvas = graphCanvas;
            }

            public async Task RenderRoads(int citiesCount, List<int> route, int[,] distances)
            {
                await graphCanvas.Dispatcher.InvokeAsync(() =>
                {
                    graphCanvas.Children.Clear();
                    vertexPositions = createVertexPositions(citiesCount);

                    //Рисуем ребра
                    for (int i = 0; i < citiesCount; i++)
                    {
                        int ind1 = route[i] - 1;
                        int ind2;

                        if (i == citiesCount - 1)
                            ind2 = route[0] - 1;
                        else
                            ind2 = route[i + 1] - 1;

                        var line = new Line
                        {
                            X1 = vertexPositions[ind1].X,
                            Y1 = vertexPositions[ind1].Y,
                            X2 = vertexPositions[ind2].X,
                            Y2 = vertexPositions[ind2].Y,
                            Stroke = Brushes.Orange,
                            StrokeThickness = 1
                        };
                        graphCanvas.Children.Add(line);

                        // Рисуем вес ребра
                        var midPoint = new Point((vertexPositions[ind1].X + vertexPositions[ind2].X) / 2,
                                                  (vertexPositions[ind1].Y + vertexPositions[ind2].Y) / 2);
                        var textBlock = new System.Windows.Controls.TextBlock
                        {
                            Text = distances[ind1, ind2].ToString(),
                            Foreground = Brushes.Black,
                            FontSize = 12
                        };
                        Canvas.SetLeft(textBlock, midPoint.X);
                        Canvas.SetTop(textBlock, midPoint.Y);
                        graphCanvas.Children.Add(textBlock);

                    }
                });

                
            }

            public async Task RenderCities(int citiesCount, List<int> route)
            {
                await graphCanvas.Dispatcher.InvokeAsync(() =>
                {
                    //vertexPositions = createVertexPositions(citiesCount);

                    //Рисуем вершины
                    foreach (var vertex in route)
                    {
                        var ellipse = new Ellipse
                        {
                            Width = 24,
                            Height = 24,
                            Fill = Brushes.LightBlue
                        };

                        Canvas.SetLeft(ellipse, vertexPositions[vertex - 1].X - 5);
                        Canvas.SetTop(ellipse, vertexPositions[vertex - 1].Y - 7);

                        graphCanvas.Children.Add(ellipse);

                        // Рисуем номер вершины
                        var vertexText = new System.Windows.Controls.TextBlock
                        {
                            Text = vertex.ToString(),
                            Foreground = Brushes.Black,
                            FontSize = 15
                        };
                        Canvas.SetLeft(vertexText, vertexPositions[vertex - 1].X);
                        Canvas.SetTop(vertexText, vertexPositions[vertex - 1].Y - 5);
                        graphCanvas.Children.Add(vertexText);
                    }
                });
                
            }


            private Point[] createVertexPositions(int citiesCount)
            {
                Point[] vertexPositions = new Point[citiesCount];

                const double Radius = 130; // Радиус многоугольника

                double angleStep = 2 * Math.PI / citiesCount;

                for (int i = 0; i < citiesCount; i++)
                {
                    double angle = i * angleStep;
                    double x = Radius * Math.Cos(angle);
                    double y = Radius * Math.Sin(angle);
                    vertexPositions[i] = new Point(x, y);
                }

                return vertexPositions;
            }
        }

    }
}
