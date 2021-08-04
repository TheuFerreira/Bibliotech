using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
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
    /// Lógica interna para LectorsWindow.xaml
    /// </summary>
    public partial class LectorsWindow : Window
    {
        DAOLector daoLector = new DAOLector();
        Lector lector = new Lector();
        Address address = new Address();
        Branch branch = new Branch();
        public LectorsWindow()
        {
            InitializeComponent();
            UpdateGrid();
         
        }

        private async void UpdateGrid()
        {
            DataTable dataTable = await daoLector.FillDataGrid(searchField.Text, 22);
            dataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void SplitAddress(string temp)
        {
            string[] result = temp.Split(',');

            address.City = result[0];
            address.City.Trim();

            address.Neighborhood = result[1];
            address.Neighborhood.Trim();

            address.Street = result[2];
            address.Street.Trim();

            address.Number = result[3];
            address.Number.Trim();
            

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                int a = Convert.ToInt32(row_selected["id_branch"].ToString());

                lector.IdLector = Convert.ToInt32(row_selected["id_lector"].ToString());

                lector.Name = row_selected["name"].ToString();

                lector.Responsible = row_selected["responsible"].ToString();

                DateTime? birth = null;
                if(DateTime.TryParse(row_selected["birth_date"].ToString(), out DateTime date))
                {
                    birth = date;
                }
                lector.BirthDate = birth;

                long? telephone = null;
                if(long.TryParse(row_selected["telephone"].ToString(), out long result))
                {
                    telephone = result;
                }
                lector.Phone = telephone; 

                string temp = row_selected["endereco"].ToString();
                SplitAddress(temp);
            }
        }

        private void searchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void searchField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchField.Text))
            {
                UpdateGrid();
            }
        }

        private void FillFieldsToUpdate()
        {
            AddEditLectorWindow addEditLector = new AddEditLectorWindow();

            addEditLector.tfName.Text = lector.Name;

            addEditLector.tfUserRegistration.Text = lector.IdLector.ToString();

            addEditLector.tfResponsible.Text = lector.Responsible;

            addEditLector.tfBirthDate.Text = lector.BirthDate.ToString();

            addEditLector.tfCity.Text = address.City;

            addEditLector.tfDistrict.Text = address.Neighborhood;

            addEditLector.tfStreet.Text = address.Street;

            addEditLector.tfNumber.Text = address.Number;

            addEditLector.tfPhone.Text = lector.Phone.ToString();

            addEditLector.ShowDialog();
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (lector.IdLector > 0)
            {
                FillFieldsToUpdate();
            }
        }
    }
}
