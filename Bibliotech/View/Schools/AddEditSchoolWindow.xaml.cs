using Bibliotech.Model.DAO;
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
            
            if((String.IsNullOrEmpty(tfName.Text)))
            {
                MessageBox.Show("Escreva um nome válido! \n");
                return false;
                
            }


            if((String.IsNullOrEmpty(tfCity.Text)))
            {
                MessageBox.Show("Escreva um nome de cidade válido! \n");
                return false;
            }


            if(String.IsNullOrEmpty(tfDistrict.Text))
            {
                MessageBox.Show("Escreva o nome de um bairro! \nEste campo é obrigatório, portanto preencha-o para continuar.");
                return false;
            }


            if (!String.IsNullOrEmpty(tfPhone.Text))
            {


                if (tfPhone.Text.Length <= 8)
                {
                    MessageBox.Show("Insira um número de telefone válido! \n É recomendável que se digite junto o DDD. \nOu também pode deixar este campo sem preencher");
                    return false;
                }
            }


            return true;
            
        }

        private async void btnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                MessageBox.Show("enoix");
                DAOSchool ds = new DAOSchool();

                await ds.InsertSchool(tfName.Text, tfCity.Text, tfDistrict.Text, tfPhone.Text, tfStreet.Text, tfNumber.Text);
            }
        }

 


    }
}
