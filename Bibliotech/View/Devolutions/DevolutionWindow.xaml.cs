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

namespace Bibliotech.View.Devolutions
{
    /// <summary>
    /// Lógica interna para DevolutionWindow.xaml
    /// </summary>
    public partial class DevolutionWindow : Window
    {
        public DevolutionWindow()
        {
            InitializeComponent();
        }
        private void SearchExemplaries()
        {

        }
        private void BtnSearhLector_Click(object sender, RoutedEventArgs e)
        {
            SearchLectorWindow lectorWindow = new SearchLectorWindow();
            lectorWindow.ShowDialog();
        }
    }
}
