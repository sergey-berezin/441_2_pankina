using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для SaveExperimentDialogWindow.xaml
    /// </summary>
    public partial class SaveExperimentDialogWindow : Window
    {
        public string? expеrimentName { get; set; } = string.Empty;

        public SaveExperimentDialogWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            expеrimentName = ExpNameTextBox.Text;
            //MessageBox.Show(expеrimentName);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
