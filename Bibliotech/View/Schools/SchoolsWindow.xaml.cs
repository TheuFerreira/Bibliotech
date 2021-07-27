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
        
        DialogService dialogService = new DialogService();
        DAOSchool ds = new DAOSchool();
        Address address = new Address();
        School school = new School();
        private bool canUpdateGrid = false;

        public bool CanUpdateGrid { get => canUpdateGrid; set => canUpdateGrid = value; }

        public SchoolsWindow()
        {
            InitializeComponent();
        }

        private async void ButtonImage_OnClick(object sender, RoutedEventArgs e)
        {
            if (school.Status == 1)
            {
                if(dialogService.ShowQuestion("Tem certeza que deseja\ndesativar esta escola?", ""))
                {
                    await ds.OnOff(0, school.Id_branch);
                }
               
                
            }
            else
            {
                if (dialogService.ShowQuestion("Tem certeza que deseja ativar esta escola?", ""))
                {
                    await ds.OnOff(1, school.Id_branch);
                }
            }
            await ds.FillDataGrid(schoolGrid);
        }

        public async void SchoolGrid_Loaded(object sender, RoutedEventArgs e)
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
             
                school.Id_branch = Convert.ToInt32(row_selected["id_branch"].ToString());
                school.Id_address = Convert.ToInt32(row_selected["id_address"].ToString());
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

        private async void ButtonImage_OnClick_1(object sender, RoutedEventArgs e)
        {
            AddEditSchoolWindow addEdit = new AddEditSchoolWindow();
            addEdit.Id = school.Id_branch;
            addEdit.Id_address = school.Id_address;
            addEdit.tfName.Text = school.Name;
            addEdit.tfCity.Text = address.City;
            addEdit.tfDistrict.Text = address.Neighborhood;
            addEdit.tfPhone.Text = school.Telephone;
            addEdit.tfStreet.Text = address.Street;
            addEdit.tfNumber.Text = address.Number;
            addEdit.IsUpdate = true;
            addEdit.ShowDialog();
            await ds.FillDataGrid(schoolGrid);
        }

        private async void ButtonImage_OnClick_2(object sender, RoutedEventArgs e)
        {
            AddEditSchoolWindow addEdit = new AddEditSchoolWindow();

            addEdit.IsUpdate = false;
            addEdit.ShowDialog();
            await ds.FillDataGrid(schoolGrid);
        }

        private async void searchField_LostFocus(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(searchField.Text))
            {
                await ds.FillDataGrid(schoolGrid);
            }
        }

        private async void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            if(canUpdateGrid)
            {
                await ds.FillDataGrid(schoolGrid);
                canUpdateGrid = false;
            }
        }
    }
}
