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
        void RenderCities(int citiesCount, List<int> route);
        void RenderRoads(int citiesCount, List<int> route, int[,] distances);
    }
}
