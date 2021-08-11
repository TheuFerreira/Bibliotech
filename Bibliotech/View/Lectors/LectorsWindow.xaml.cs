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
using EnumsNET;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using Bibliotech.Singletons;

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

        DialogService dialogServices = new DialogService();

        private TypeSearch typeSearch;

        //mudar depois, este dado deverá vir a partir do usário atual
        int idBranch = Session.Instance.User.Branch.IdBranch;

        public LectorsWindow()
        {
            InitializeComponent();
            

            List<string> typesSearch = Enum.GetValues(typeof(TypeSearch))
              .Cast<TypeSearch>()
              .Select(x => x.AsString(EnumFormat.Description))
              .ToList();
            searchField.ItemsSource = typesSearch;

            typeSearch = TypeSearch.Current;
            searchField.SelectedItem = typeSearch.AsString(EnumFormat.Description);

            UpdateGrid();

        }

        private void DisableButtons()
        {
            btnEdit.IsEnabled = false;
            btnAdd.IsEnabled = false;
            btnDelete.IsEnabled = false;
            searchField.IsEnabled = false;
        }

        private void EnableButtons()
        {
            btnEdit.IsEnabled = true;
            btnAdd.IsEnabled = true;
            btnDelete.IsEnabled = true;
            searchField.IsEnabled = true;
        }

        private async void UpdateGrid()
        {
            
            if (searchField.SelectedIndex == ((int)TypeSearch.All))
            {

                typeSearch = TypeSearch.All;
                dataGrid.Columns[4].Visibility = Visibility.Visible;
            }
            else
            {
                typeSearch = TypeSearch.Current;
                dataGrid.Columns[4].Visibility = Visibility.Collapsed;
            }

            DisableButtons();
            DataTable dataTable = await daoLector.FillDataGrid(searchField.Text, idBranch , typeSearch);
            if (dataTable != null)
            {
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            EnableButtons();
        }

        private void SplitAddress(string temp)
        {
            string[] result = temp.Split(',');

            address.City = result[0];
            address.City = address.City.Trim();

            address.Neighborhood = result[1];
            address.Neighborhood = address.Neighborhood.Trim();

            address.Street = result[2];
            address.Street = address.Street.Trim();

            address.Number = result[3];
            address.Number = address.Number.Trim();

            try
            {
                address.Complement = result[4];
                address.Complement = address.Complement.Trim();
            }
            catch (System.IndexOutOfRangeException)
            {
            }

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;

            if (row_selected != null)
            {
                branch.IdBranch = Convert.ToInt32(row_selected["id_branch"].ToString());

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

                address.IdAddress = Convert.ToInt32(row_selected["id_address"].ToString());
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
            AddEditLectorWindow addEditLector = new AddEditLectorWindow(idBranch, true, address.IdAddress);

            addEditLector.tfName.Text = lector.Name;

            addEditLector.tfUserRegistration.Text = lector.IdLector.ToString();

            addEditLector.tfResponsible.Text = lector.Responsible;

            addEditLector.tfBirthDate.Text = lector.BirthDate.ToString();

            addEditLector.tfCity.Text = address.City;

            addEditLector.tfDistrict.Text = address.Neighborhood;

            addEditLector.tfStreet.Text = address.Street;

            addEditLector.tfNumber.Text = address.Number;

            addEditLector.tfComplement.Text = address.Complement;

            addEditLector.tfPhone.Text = lector.Phone.ToString();

            addEditLector.ShowDialog();
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (lector.IdLector > 0)
            {
                if (branch.IdBranch != idBranch)
                {
                    dialogServices.ShowError("Você não pode editar usuários de outra escola.");
                    return;
                }
                FillFieldsToUpdate();
                UpdateGrid();
            }
            
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            AddEditLectorWindow addEditLectorWindow = new AddEditLectorWindow(idBranch, false, address.IdAddress);

            _ = addEditLectorWindow.ShowDialog();
            UpdateGrid();
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            /* if (dataGrid.SelectedItem != null)
             {
                 if((dialogServices.ShowQuestion("Tem certeza que deseja excluir este leitor?", "Não é possível desfazer esta ação.")))
                 {
                     if(await daoLector.Delete(lector.IdLector))
                     {
                         dialogServices.ShowSuccess("Leitor excluído com sucesso!");
                         UpdateGrid();
                         return;
                     }
                     dialogServices.ShowError("Algo deu errado!\nTente novamente.");
                     return;
                 }
             }
             dialogServices.ShowError("Escolha algo primeiro.");*/


            if (dataGrid.SelectedItem == null)
            {
                dialogServices.ShowError("Escolha algo primeiro.");
                return;
            }

            bool result = dialogServices.ShowQuestion("Tem certeza que deseja excluir este leitor?", "Não é possível desfazer esta ação.");
            if (result == false)
            {
                return;
            }

            result = await daoLector.Delete(lector.IdLector);
            if (result == false)
            {
                dialogServices.ShowError("Algo deu errado!\nTente novamente.");
                return;
            }

            dialogServices.ShowSuccess("Leitor excluído com sucesso!");
            UpdateGrid();

        }
    }
}
