using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;


namespace ViewModel
{
    public interface ICityMapRenderer
    {
        //Point[] createVertexPositions(int citiesCount);
        Task RenderCities(int citiesCount, List<int> route);
        Task RenderRoads(int citiesCount, List<int> route, int[,] distances);
    }
}
