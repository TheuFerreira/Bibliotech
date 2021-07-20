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
            double result = 0;

            if((tfName.Text == "") || (double.TryParse(tfName.Text, out result)))
            {
                MessageBox.Show("Escreva um nome válido! \nEle deve ser composto por letras. \nNúmeros apenas não são aceitos.");
                tfName.Focus();
                
            }
            else
            {
                control++;
            }

            if((tfCity.Text == "") || (double.TryParse(tfCity.Text, out result)))
            {
                MessageBox.Show("Escreva um nome de cidade válido! \nEle deve ser composto por letras. \nNúmeros apenas não são aceitos.");
                tfCity.Focus();
            }
            else
            {
                control++;
            }

            if(tfDistrict.Text == "")
            {
                MessageBox.Show("Escreva o nome de um bairro! \nEste campo é obrigatório, portanto preencha-o para continuar.");
                tfDistrict.Focus();
            }
            else
            {
                control++;
            }

            if(((tfPhone.Text.Length >= 8) || (tfPhone.Text == "")) && (double.TryParse(tfPhone.Text, out result)))
            {
                control++;
                
            }
            else
            {
                MessageBox.Show("Insira um número de telefone válido! \n É recomendável que se digite junto o DDD. \nOu também pode deixar este campo sem preencher");
                tfPhone.Focus();
            }

            if(((double.TryParse(tfNumber.Text, out result)) && result>=0) || (tfNumber.Text == ""))
            {
                control++;
                
            }
            else
            {
                MessageBox.Show("Insira um número válido! \nOu deixe este campo em branco.");
                tfNumber.Focus();
            }


            if(control < 5)
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
            if (ValidateFields())
            {
                MessageBox.Show("enoix");
            }
        }

 


    }
}
