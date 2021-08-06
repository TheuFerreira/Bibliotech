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
        private readonly User loggedUser;
        private readonly Branch currentBranch;
        private TypeSearch typeSearch;
        private List<User> users;

        public UsersWindow()
        {
            InitializeComponent();

            daoUser = new DAOUser();
            dialogService = new DialogService();

            loggedUser = Session.Instance.User;
            currentBranch = loggedUser.Branch;

            List<string> typesSearch = Enum.GetValues(typeof(TypeSearch))
                .Cast<TypeSearch>()
                .Where(x => !loggedUser.IsUser() || x != TypeSearch.All)
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();
            searchField.ItemsSource = typesSearch;

            typeSearch = TypeSearch.Current;
            searchField.SelectedItem = typeSearch.AsString(EnumFormat.Description);
        }

        private async void SearchUsers()
        {
            string text = searchField.Text;
            typeSearch = Enums.Parse<TypeSearch>(searchField.SelectedItem.ToString(), false, EnumFormat.Description);

            users = await daoUser.SearchByText(typeSearch, text, currentBranch);
            dataGrid.ItemsSource = users;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SearchUsers();
        }

        private void SearchField_Click(object sender, RoutedEventArgs e)
        {
            SearchUsers();
        }

        private User GetUserInSelectedRow()
        {
            int selectedRow = dataGrid.SelectedIndex;
            return users[selectedRow];
        }

        private async void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            User selectedUser = GetUserInSelectedRow();

            if (loggedUser.Equals(selectedUser) == false)
            {
                dialogService.ShowError("Você não pode editar outro usuário!!!");
                return;
            }

            int idUser = selectedUser.IdUser;
            selectedUser = await daoUser.GetUserById(idUser);

            _ = new AddEditUserWindow(selectedUser).ShowDialog();

            SearchUsers();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            User newUser = new User();

            if (loggedUser.IsController() == false)
            {
                newUser.Branch = currentBranch;
            }

            _ = new AddEditUserWindow(newUser).ShowDialog();

            SearchUsers();
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            User selectedUser = GetUserInSelectedRow();

            if (loggedUser.IsUser()
                && selectedUser.IsController())
            {
                dialogService.ShowError($"Você não pode excluir o usuário {selectedUser.Name}!!!");
                return;
            }

            if (loggedUser.Equals(selectedUser))
            {
                dialogService.ShowError("Você não pode se excluir do BIBLIOTECH!!!");
                return;
            }

            string nameUser = selectedUser.Name;

            bool result = dialogService.ShowQuestion("EXCLUSÃO", $"Tem certeza de que deseja excluir o Usuário {nameUser}?");
            if (result == false)
            {
                return;
            }

            int idUser = selectedUser.IdUser;
            await daoUser.Delete(idUser);

            dialogService.ShowSuccess($"Usuário {nameUser}, excluído com sucesso!!!");
            SearchUsers();
        }
    }
}
