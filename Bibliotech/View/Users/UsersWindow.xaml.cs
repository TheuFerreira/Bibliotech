using Bibliotech.Model.DAO;
using System.Windows;

namespace Bibliotech.View.Users
{
    /// <summary>
    /// Lógica interna para UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window
    {
        private readonly DAOUser daoUser;

        public UsersWindow()
        {
            InitializeComponent();

            daoUser = new DAOUser();
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
    }
}
