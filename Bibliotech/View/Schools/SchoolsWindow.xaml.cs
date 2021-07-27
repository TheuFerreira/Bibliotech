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
using Bibliotech.Model.Entities.Enums;

namespace Bibliotech.View.Schools

{
    /// <summary>
    /// Lógica interna para SchoolsWindow.xaml
    /// </summary>
    public partial class SchoolsWindow : Window
    {
        
        DialogService dialogService = new DialogService();
        DAOSchool daoSchool = new DAOSchool();
        Address address = new Address();
        School school = new School();
    
        public SchoolsWindow()
        {
            InitializeComponent();
        }

        private async void UpdateGrid()
        {
            DataTable dataTable = new DataTable();
            dataTable = await daoSchool.FillDataGrid(searchField.Text);
            schoolGrid.ItemsSource = dataTable.DefaultView;
        }

        private async void ButtonOnOff_OnClick(object sender, RoutedEventArgs e)
        {
            if (school.Id_branch >= 1)
            {
                if (school.Status == Status.Active)
                {
                    if (dialogService.ShowQuestion("Tem certeza que deseja\ndesativar esta escola?", ""))
                    {
                        await daoSchool.OnOff(0, school.Id_branch);
                        dialogService.ShowSuccess("Desativado com sucesso!");
                    }

                    UpdateGrid();
                    return;
                }

                if (dialogService.ShowQuestion("Tem certeza que deseja\nativar esta escola?", ""))
                {
                    await daoSchool.OnOff(1, school.Id_branch);
                    dialogService.ShowSuccess("Ativado com sucesso!");
                }

                UpdateGrid();
            }
            
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            AddEditSchoolWindow addEdit = new AddEditSchoolWindow();
            addEdit.Id = school.Id_branch;
            addEdit.Id_address = school.Id_address;
            addEdit.tfName.Text = school.Name;
            addEdit.tfCity.Text = address.City;
            addEdit.tfDistrict.Text = address.Neighborhood;
            addEdit.tfPhone.Text = school.Telephone.ToString();
            addEdit.tfStreet.Text = address.Street;
            addEdit.tfNumber.Text = address.Number;
            addEdit.IsUpdate = true;
            addEdit.tbInfo.Text = "Editar Escola";
            if(school.Id_branch >= 1)
            addEdit.ShowDialog();
            UpdateGrid();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            AddEditSchoolWindow addEdit = new AddEditSchoolWindow();

            addEdit.IsUpdate = false;
            addEdit.tbInfo.Text = "Adicionar Escola";
            addEdit.ShowDialog();
            UpdateGrid();
        }

        public void SchoolGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
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
                long.TryParse(row_selected["telephone"].ToString(), out long temp);
                school.Telephone = temp;

                if(row_selected["description"].ToString() == "Ativo")
                {
                    school.Status = Status.Active;
                }
                else
                {
                    school.Status = Status.Inactive;
                }

                address.City = row_selected["city"].ToString();
                address.Neighborhood = row_selected["neighborhood"].ToString();
                address.Street = row_selected["street"].ToString();
                address.Number = row_selected["number"].ToString();
             

            }
        }

        private void searchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void searchField_LostFocus(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(searchField.Text))
            {
                UpdateGrid();
            }
        }

    }
}
