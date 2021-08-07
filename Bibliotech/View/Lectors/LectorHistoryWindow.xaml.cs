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
    /// Lógica interna para LectorHistoryWindow.xaml
    /// </summary>
    public partial class LectorHistoryWindow : Window
    {
        private int idLector;
        DAOLector daoLector = new DAOLector();
        public LectorHistoryWindow(int idLector)
        {
            InitializeComponent();
            this.idLector = idLector;
            UpdateGrid();
        }

        private async void UpdateGrid()
        {
            DataTable dataTable = await daoLector.FillDataGrid(searchField.Text, idLector);
            if (dataTable != null)
            {
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
        }
    }
}
