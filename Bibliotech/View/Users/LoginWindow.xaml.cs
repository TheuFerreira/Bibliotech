using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Singletons;
using Bibliotech.UserControls.CustomDialog;
using System.Windows;

namespace Bibliotech.View.Users
{
    /// <summary>
    /// Lógica interna para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly DAOUser DaoUser;

        public LoginWindow()
        {
            InitializeComponent();
            DaoUser = new DAOUser();
        }

        private void ShowMessage(string title, string contents, TypeDialog typeDialog)
        {
            _ = new InformationDialog(title, contents, typeDialog).ShowDialog();
        }

        private async void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            btnEnter.IsEnabled = false;

            if (string.IsNullOrWhiteSpace(tfUser.Text)
                || string.IsNullOrWhiteSpace(tfPassword.Text))
            {
                ShowMessage("Atenção", "Usuário ou Senha inválida. Tente novamente", TypeDialog.Error);
                return;
            }
           
            string userName = tfUser.Text;
            string password = tfPassword.Text;

            User user = await DaoUser.IsValidUser(userName, password);
            btnEnter.IsEnabled = true;

            if (user == null)
            {
                ShowMessage("Atenção", "Usuário não encontrado", TypeDialog.Error);

                tfUser.Text = string.Empty;
                tfPassword.Text = string.Empty;

                return;
            }

            Session.Instance.User = user;

            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Session.Instance.User = null;

            Close();
        }

        private void BtnDatabase_Click(object sender, RoutedEventArgs e)
        {
            _ = new ServerWindow().ShowDialog();
        }
    }
}
