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

namespace Bibliotech.View.Schools
{
    /// <summary>
    /// Lógica interna para AddEditSchoolWindow.xaml
    /// </summary>
    public partial class AddEditSchoolWindow : Window
    {
        public AddEditSchoolWindow()
        {
            InitializeComponent();
        }

        public bool ValidateFields()
        {
            int control = 0;

            if(tfName.Text == "")
            {
                tfName.Focus();
                
            }
            else
            {
                control++;
            }

            if(tfCity.Text == "")
            {
                tfCity.Focus();
            }
            else
            {
                control++;
            }

            if(tfDistrict.Text == "")
            {
                tfDistrict.Focus();
            }
            else
            {
                control++;
            }

            if(tfPhone.Text.Length < 10)
            {
                tfPhone.Focus();
            }
            else
            {
                control++;
            }

            if(tfStreet.Text == "")
            {
                tfStreet.Focus();
            }
            else
            {
                control++;
            }

            if(!(double.TryParse(tfNumber.Text, out double result)))
            {
                tfNumber.Focus();
            }
            else
            {
                control++;
            }


            if(control < 6)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        private void btnSave_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("certo");
        }
    }
}
