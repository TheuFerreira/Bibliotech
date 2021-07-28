using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using System;
using System.Data;
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

        public UsersWindow()
        {
            InitializeComponent();

            daoUser = new DAOUser();
            dialogService = new DialogService();
        }

        private async void LoadUsers()
        {
            string text = searchField.Text;

            dataGrid.ItemsSource = await daoUser.SearchByText(text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void SearchField_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private async void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            DataRowView row = dataGrid.SelectedItem as DataRowView;
            int idUser = int.Parse(row["id_user"].ToString());

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

            int idUser = Convert.ToInt32(row["id_user"].ToString());
            await daoUser.Delete(idUser);

            dialogService.ShowSuccess($"Usuário {name}, excluído com sucesso!!!");
            LoadUsers();
        }
    }
}
