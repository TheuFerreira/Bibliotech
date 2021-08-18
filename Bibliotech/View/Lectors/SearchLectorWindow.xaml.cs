using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Singletons;
using Bibliotech.UserControls;
using Bibliotech.View.Lendings;
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

        public Lector lector = new Lector();

        Address address = new Address();

       // Branch branch = new Branch();
        //trocar para o krai la de session
        private int idBranch = Session.Instance.User.Branch.IdBranch;

        public SearchLectorWindow()
        {
            InitializeComponent();
            UpdateGrid();
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
            AddEditLectorWindow addEditLector = new AddEditLectorWindow(idBranch, false, address.IdAddress);
            addEditLector.ShowDialog();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if(dataGrid.SelectedItem == null)
            {
                return;
            }
            Close();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;

            if (row_selected != null)
            {
               // branch.IdBranch = Convert.ToInt32(row_selected["id_branch"].ToString());

                lector.IdLector = Convert.ToInt32(row_selected["id_lector"].ToString());

                lector.Name = row_selected["name"].ToString();

                lector.Responsible = row_selected["responsible"].ToString();

                DateTime? birth = null;
                if (DateTime.TryParse(row_selected["birth_date"].ToString(), out DateTime date))
                {
                    birth = date;
                }
                lector.BirthDate = birth;

                long? telephone = null;
                if (long.TryParse(row_selected["telephone"].ToString(), out long result))
                {
                    telephone = result;
                }
                lector.Phone = telephone;

                address.IdAddress = Convert.ToInt32(row_selected["id_address"].ToString());
                string temp = row_selected["endereco"].ToString();

            }
        }
    }
}
