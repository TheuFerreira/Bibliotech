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
using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.UserControls;
using Bibliotech.UserControls.CustomDialog;

namespace Bibliotech.View.Users
{
    /// <summary>
    /// Lógica interna para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    { 
        public static string NameBranch { get; set; }
        private DAOUser DaoUser;
        public User User;
        private readonly UserControl control;

        public LoginWindow()
        {
            InitializeComponent();
            DaoUser = new DAOUser();
        }
        private void ShowMessage(string title, string contents, TypeDialog typeDialog)
        {
            InformationDialog dialog = new InformationDialog(title, contents, typeDialog);
            dialog.Show();
        }
        private void ClearControl(TextField textBox)
        {
            textBox.Text = string.Empty;
        }
        private async void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tfUser.Text) || string.IsNullOrWhiteSpace(tfPassword.Text))
            {
                ShowMessage("Atenção", "Usuário ou Senha inválida. Tente novamente", TypeDialog.Error);
            }

            btnEnter.IsEnabled = false;
            String user = tfUser.Text;
            String password = tfPassword.Text;
            User = await DaoUser.IsValidUser(user, password);
            btnEnter.IsEnabled = true;

            if(User != null)
            {
                this.Close();
            }
            else
            {
                
                ShowMessage("Atenção", "Usuário não encontrado", TypeDialog.Error);
                ClearControl(tfUser);
                ClearControl(tfPassword);
                return;
            }
            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnDatabase_Click(object sender, RoutedEventArgs e)
        {
            ServerWindow server = new ServerWindow();
            server.ShowDialog();
        }
    }
}
