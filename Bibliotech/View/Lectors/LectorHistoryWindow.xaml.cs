using Bibliotech.Model.DAO;
using System.Data;
using System.Windows;

namespace Bibliotech.View.Lectors
{
    /// <summary>
    /// Lógica interna para LectorHistoryWindow.xaml
    /// </summary>
    public partial class LectorHistoryWindow : Window
    {
        private readonly int idLector;
        private readonly DAOLector daoLector = new DAOLector();

        public LectorHistoryWindow(int idLector)
        {
            InitializeComponent();

            this.idLector = idLector;
            
            UpdateGrid();
        }

        private async void UpdateGrid()
        {
            searchField.IsEnabled = false;
            loading.Awaiting = true;
            DataTable dataTable = await daoLector.FillDataGrid(searchField.Text, idLector);
            if (dataTable != null)
            {
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            loading.Awaiting = false;
            searchField.IsEnabled = true;
        }

        private void searchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void searchField_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }
    }
}
