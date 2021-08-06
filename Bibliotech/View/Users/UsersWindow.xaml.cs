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
using System.Windows;

namespace Bibliotech.View.Users
{
    /// <summary>
    /// Lógica interna para UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window
    {
        private readonly DAOUser daoUser;
        private readonly DialogService dialogService;
        private TypeSearch typeSearch;
        private readonly Branch currentBranch;

        public UsersWindow()
        {
            InitializeComponent();

            daoUser = new DAOUser();
            dialogService = new DialogService();

            List<string> typesSearch = Enum.GetValues(typeof(TypeSearch))
                .Cast<TypeSearch>()
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();
            searchField.ItemsSource = typesSearch;

            typeSearch = TypeSearch.Current;
            searchField.SelectedItem = typeSearch.AsString(EnumFormat.Description);

            currentBranch = Session.Instance.User.Branch;
        }

        private async void LoadUsers()
        {
            string text = searchField.Text;
            typeSearch = Enums.Parse<TypeSearch>(searchField.SelectedItem.ToString(), false, EnumFormat.Description);

            dataGrid.ItemsSource = await daoUser.SearchByText(typeSearch, text, currentBranch);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void SearchField_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private int GetIdUserInSelectedRow()
        {
            DataRowView row = dataGrid.SelectedItem as DataRowView;
            return int.Parse(row["id_user"].ToString());
        }

        private async void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            int idUser = GetIdUserInSelectedRow();

            User user = await daoUser.GetUserById(idUser);
            _ = new AddEditUserWindow(user).ShowDialog();

            LoadUsers();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new AddEditUserWindow(new User()).ShowDialog();

            LoadUsers();
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            DataRowView row = dataGrid.SelectedItem as DataRowView;
            string name = row["name"].ToString();

            bool result = dialogService.ShowQuestion("EXCLUSÃO", $"Tem certeza de que deseja excluir o Usuário {name}?");
            if (result == false)
            {
                return;
            }

            int idUser = GetIdUserInSelectedRow();
            await daoUser.Delete(idUser);

            dialogService.ShowSuccess($"Usuário {name}, excluído com sucesso!!!");
            LoadUsers();
        }
    }
}
