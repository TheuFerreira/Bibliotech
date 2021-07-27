using Bibliotech.Model.DAO;
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
            string text = string.Empty;

            dataGrid.ItemsSource = await daoUser.SearchByText(text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;

            DataRowView row = dataGrid.SelectedItem as DataRowView;
            string name = row["name"].ToString();

            if (dialogService.ShowQuestion("EXCLUSÃO", $"Tem certeza de que deseja excluir o Usuário {name}?") == false)
                return;

            int idUser = Convert.ToInt32(row["id_user"].ToString());

            bool result = await daoUser.Delete(idUser);
        }
    }
}
