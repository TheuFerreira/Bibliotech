using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using Bibliotech.Singletons;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Bibliotech.View.Lectors
{
    public partial class LectorsWindow : Window
    {
        private readonly DAOLector daoLector = new DAOLector();
        private readonly DialogService dialogServices = new DialogService();

        private TypeSearch typeSearch;

        private readonly int idBranch = Session.Instance.User.Branch.IdBranch;

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
        }

        private void SetButtons(bool value)
        {
            btnEdit.IsEnabled = value;
            btnAdd.IsEnabled = value;
            btnDelete.IsEnabled = value;
            searchField.IsEnabled = value;
        }

        private void SetLoadingAndButtons(bool value)
        {
            SetButtons(value);

            loading.Awaiting = !value;
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

            SetLoadingAndButtons(false);

            DataTable dataTable = await daoLector.FillDataGrid(searchField.Text, idBranch, typeSearch);
            dataGrid.ItemsSource = dataTable.DefaultView;

            SetLoadingAndButtons(true);
        }

        private async Task<Lector> GetLector()
        {
            DataRowView row_selected = dataGrid.SelectedItem as DataRowView;
            if (row_selected == null)
            {
                return new Lector();
            }

            SetButtons(false);

            int idLector = Convert.ToInt32(row_selected["id_lector"].ToString());
            Lector lector = await daoLector.GetById(idLector);

            SetButtons(true);

            return lector;
        }

        private void SearchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void FillFieldsToUpdate(Lector lector)
        {
            _ = new AddEditLectorWindow(lector).ShowDialog();
        }

        private async void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            Lector lector = await GetLector();

            if (lector.IdLector <= 0)
            {
                return;
            }

            if (lector.IdBranch != idBranch)
            {
                dialogServices.ShowError("Você não pode editar usuários de outra escola.");
                return;
            }

            FillFieldsToUpdate(lector);
            UpdateGrid();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new AddEditLectorWindow(new Lector()).ShowDialog();

            UpdateGrid();
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                dialogServices.ShowError("Escolha algo primeiro.");
                return;
            }

            Lector lector = await GetLector();

            bool result = dialogServices.ShowQuestion("Tem certeza que deseja excluir este leitor?", "Não é possível desfazer esta ação.");
            if (result == false)
            {
                return;
            }

            loading.Awaiting = true;
            SetLoadingAndButtons(false);

            result = await daoLector.Delete(lector.IdLector);
            if (result == false)
            {
                dialogServices.ShowError("Algo deu errado!\nTente novamente.");
                loading.Awaiting = false;

                SetLoadingAndButtons(true);
                return;
            }

            dialogServices.ShowSuccess("Leitor excluído com sucesso!");
            UpdateGrid();
        }
    }
}
