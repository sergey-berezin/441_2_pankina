using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ViewModel
{
    public interface ICityMapRenderer
    {
        Task RenderCities(int citiesCount, List<int> route);
        Task RenderRoads(int citiesCount, List<int> route, int[,] distances);
    }
}
