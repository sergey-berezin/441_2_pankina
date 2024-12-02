using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class Run
    {
        public string experimentName { get; set; }

        public string fileName { get; set; }

        public Run(string experimentName, string fileName)
        {
            this.experimentName = experimentName;
            this.fileName = fileName;
        }
    }
}
