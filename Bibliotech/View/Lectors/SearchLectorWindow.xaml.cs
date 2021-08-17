using Bibliotech.Model.DAO;
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
using System.Windows.Shapes;

namespace Bibliotech.View.Lectors
{
    /// <summary>
    /// Lógica interna para SearchLectorWindow.xaml
    /// </summary>
    public partial class SearchLectorWindow : Window
    {
        DAOLector daoLector = new DAOLector();

        private int idBranch = 1;
        public SearchLectorWindow()
        {
            InitializeComponent();
            UpdateGrid();
        }

        private async void UpdateGrid()
        {
            DataTable dataTable = await daoLector.FillDataGrid(searchField.Text, idBranch, 0);
            if (dataTable != null)
            {
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
        }
    }
}
