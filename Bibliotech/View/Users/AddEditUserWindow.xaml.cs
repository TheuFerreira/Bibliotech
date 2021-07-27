using Bibliotech.Model.Entities;
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

namespace Bibliotech.View.Users
{
    /// <summary>
    /// Lógica interna para AddEditUserWindow.xaml
    /// </summary>
    public partial class AddEditUserWindow : Window
    {
        public AddEditUserWindow(User user)
        {
            InitializeComponent();

            if (user.IdUser != -1)
            {
                tfName.Text = user.Name;
                tfBirthDate.Text = user.GetBirthDate();
                tfUserName.Text = user.UserName;
                tfPassword.Text = user.Password;
                tfPhone.Text = user.GetTelephone();
                tfCity.Text = user.Address.City;
                tfStreet.Text = user.Address.Street;
                tfDistrict.Text = user.Address.Neighborhood;
                tfNumber.Text = user.Address.Neighborhood;
                tfComplement.Text = user.Address.Neighborhood;
            }
        }
    }
}
