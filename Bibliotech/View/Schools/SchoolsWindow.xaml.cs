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
using Bibliotech.Model.Entities;

namespace Bibliotech.View.Schools

{
    /// <summary>
    /// Lógica interna para SchoolsWindow.xaml
    /// </summary>
    public partial class SchoolsWindow : Window
    {
        AddEditSchoolWindow addEdit = new AddEditSchoolWindow();
        DialogService dialogService = new DialogService();
        DAOSchool ds = new DAOSchool();
        Address address = new Address();
        School school = new School();
        public bool isUpdate = false;

        public SchoolsWindow()
        {
            InitializeComponent();
        }

        private void ButtonImage_OnClick(object sender, RoutedEventArgs e)
        {
            object obj = new object();
            List<DataGridCellInfo> gridCells = new List<DataGridCellInfo>();
            //obj = schoolGrid.SelectedCells.ElementAt(1).Item ;
          
            

        }

        private async void SchoolGrid_Loaded(object sender, RoutedEventArgs e)
        {
           await ds.FillDataGrid(schoolGrid);
            
        }



        private async void searchField_Click(object sender, RoutedEventArgs e)
        {
            await ds.FillDataGrid(schoolGrid, searchField.Text);
        }

        private void schoolGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if(row_selected !=null)
            {
             

                school.Id_address = Convert.ToInt32(row_selected["id_branch"].ToString());
                school.Name = row_selected["name"].ToString();
                school.Telephone = row_selected["telephone"].ToString();
                school.Status = Convert.ToInt32(row_selected["status"].ToString());

                address.City = row_selected["city"].ToString();
                address.Neighborhood = row_selected["neighborhood"].ToString();
                address.Street = row_selected["street"].ToString();
                address.Number = row_selected["number"].ToString();
                address.Complement = row_selected["complement"].ToString();


            }
        }

        private void ButtonImage_OnClick_1(object sender, RoutedEventArgs e)
        {
            addEdit.tfName.Text = school.Name;
            addEdit.tfCity.Text = address.City;
            addEdit.tfDistrict.Text = address.Neighborhood;
            addEdit.tfPhone.Text = school.Telephone;
            addEdit.tfStreet.Text = address.Street;
            addEdit.tfNumber.Text = address.Number;
            isUpdate = true;
            addEdit.ShowDialog();

        }
    }
}
