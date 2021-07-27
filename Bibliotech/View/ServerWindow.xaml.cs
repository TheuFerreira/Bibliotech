using Bibliotech.Model.DAO;
using Bibliotech.Services;
using Bibliotech.Singletons;
using System.Windows;

namespace Bibliotech.View
{
    /// <summary>
    /// Lógica interna para ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        private readonly DAOServer daoServer;
        private readonly DialogService dialogService;

        public ServerWindow()
        {
            InitializeComponent();

            daoServer = new DAOServer();
            dialogService = new DialogService();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tfServer.Text = Session.Instance.Server.ServerName;
            tfPort.Text = Session.Instance.Server.Port.ToString();
            tfDatabase.Text = Session.Instance.Server.Database;
            tfUser.Text = Session.Instance.Server.UserName;
            tfPassword.Text = Session.Instance.Server.Password;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidatedFields()
        {
            if (string.IsNullOrEmpty(tfServer.Text)
                || string.IsNullOrEmpty(tfPort.Text)
                || string.IsNullOrEmpty(tfDatabase.Text)
                || string.IsNullOrEmpty(tfUser.Text)
                || string.IsNullOrEmpty(tfPassword.Text))
            {
                return false;
            }

            return true;
        }

        private async void BtnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            if (ValidatedFields() == false)
            {
                dialogService.ShowError("Preencha todos os campos com *!!!");
                return;
            }

            btnTestConnection.IsEnabled = false;
            btnConfirm.IsEnabled = false;

            bool result = await daoServer.TestConnection(tfServer.Text, tfPort.Text, tfDatabase.Text, tfUser.Text, tfPassword.Text);

            btnTestConnection.IsEnabled = true;
            btnConfirm.IsEnabled = true;

            if (result)
                dialogService.ShowSuccess("Conexão bem sucedida!!!");
            else
                dialogService.ShowError("Conexão mal sucedida!!!");
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (ValidatedFields() == false)
            {
                dialogService.ShowError("Preencha todos os campos com *!!!");
                return;
            }

            Session.Instance.Server.ServerName = tfServer.Text;
            Session.Instance.Server.Port = int.Parse(tfPort.Text);
            Session.Instance.Server.Database = tfDatabase.Text;
            Session.Instance.Server.UserName = tfUser.Text;
            Session.Instance.Server.Password = tfPassword.Text;
            Session.Instance.Server.Save();

            dialogService.ShowSuccess("Configurações Salvas!!!");

            DialogResult = true;
            Close();
        }
    }
}
