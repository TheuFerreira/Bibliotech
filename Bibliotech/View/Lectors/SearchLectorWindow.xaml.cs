using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Singletons;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.View.Lectors
{
    /// <summary>
    /// Lógica interna para SearchLectorWindow.xaml
    /// </summary>
    public partial class SearchLectorWindow : Window
    {
        public Lector Lector { get; private set; }
        public bool IsConfirmed { get => isConfirmed; set => isConfirmed = value; }

        private readonly DAOLector daoLector = new DAOLector();

        private readonly int idBranch = Session.Instance.User.Branch.IdBranch;

        private bool isConfirmed = false;

        public SearchLectorWindow()
        {
            InitializeComponent();
            UpdateGrid();

            Lector = new Lector();
        }

        private void OnOffControls(bool awaiting)
        {
            addButton.IsEnabled = awaiting;
            selectButton.IsEnabled = awaiting;
            searchField.IsEnabled = awaiting;
        }

        private async void UpdateGrid()
        {
            loading.Awaiting = true;
            OnOffControls(false);
            DataTable dataTable = await daoLector.FillDataGrid(searchField.Text, idBranch, Model.Entities.Enums.TypeSearch.Current);
            if (dataTable != null)
            {
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            loading.Awaiting = false;
            OnOffControls(true);
        }

        private void searchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            _ = new AddEditLectorWindow(new Lector()).ShowDialog();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }
            isConfirmed = true;
            Close();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dataGrid.Items.Count < 1)
            {
                return;
            }
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;

            Lector.IdLector = Convert.ToInt32(row_selected["id_lector"].ToString());
            Lector.Name = row_selected["name"].ToString();

            OnOffControls(false);
            OnOffControls(true);
        }
    }
}
