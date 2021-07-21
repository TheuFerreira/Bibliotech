using Bibliotech.Model.DAO;
using Bibliotech.Services;
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
using System.Data;
using MySqlConnector;

namespace Bibliotech.View.Schools

{
    /// <summary>
    /// Lógica interna para SchoolsWindow.xaml
    /// </summary>
    public partial class SchoolsWindow : Window
    {
        DialogService dialogService = new DialogService();
        DAOSchool ds = new DAOSchool();

        public SchoolsWindow()
        {
            InitializeComponent();
        }

        private void ButtonImage_OnClick(object sender, RoutedEventArgs e)
        {
           
        }

        private async void SchoolGrid_Loaded(object sender, RoutedEventArgs e)
        {
           await ds.FillDataGrid(schoolGrid);
            
        }

        private async void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await ds.FillDataGrid(schoolGrid, "y");
        }
    }
}
