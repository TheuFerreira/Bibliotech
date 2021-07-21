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

namespace Bibliotech.View.Users
{
    /// <summary>
    /// Lógica interna para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    { 
        public static int IdBranch { get; set; }
        public DAOUser DaoUser;
        public User User;

        public LoginWindow()
        {
            InitializeComponent();
            DaoUser = new DAOUser();
        }

        private async void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            String user = tfUser.Text;
            String password = tfPassword.Text;
            User = await DaoUser.IsValidUser(user, password);
            if(User != null)
            {
                MessageBox.Show("Hello World");
            }
            
         
            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
