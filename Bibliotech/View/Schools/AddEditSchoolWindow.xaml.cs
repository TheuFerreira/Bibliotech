using Bibliotech.Model.DAO;
using Bibliotech.Services;
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
using Bibliotech.Model.Entities;
namespace Bibliotech.View.Schools
{
    /// <summary>
    /// Lógica interna para AddEditSchoolWindow.xaml
    /// </summary>
    public partial class AddEditSchoolWindow : Window
    {
        DialogService dialogService = new DialogService();
        DAOSchool ds = new DAOSchool();
        SchoolsWindow schoolsWindow = new SchoolsWindow();

        public AddEditSchoolWindow()
        {
            InitializeComponent();
        }

        public bool ValidateFields()
        {
            
            if((String.IsNullOrEmpty(tfName.Text)))
            {
                dialogService.ShowError("Escreva um nome válido! \nEste campo é obrigatório, portanto preencha-o para continuar.");
                return false;
                
            }


            if((String.IsNullOrEmpty(tfCity.Text)))
            {
                dialogService.ShowError("Escreva um nome de cidade válido! \nEste campo é obrigatório, portanto preencha-o para continuar.");
                return false;
            }


            if(String.IsNullOrEmpty(tfDistrict.Text))
            {
                
                dialogService.ShowError("Escreva o nome de um bairro válido! \nEste campo é obrigatório, portanto preencha-o para continuar.");
                return false;
            }


            if (!String.IsNullOrEmpty(tfPhone.Text))
            {


                if (tfPhone.Text.Length <= 8)
                {
                    dialogService.ShowError("Insira um número de telefone válido! \n É recomendável que se digite junto o DDD. \nOu também pode deixar este campo sem preencher");
                    return false;
                }
            }


            return true;
            
        }

        private async void btnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                if (!schoolsWindow.isUpdate)
                {
                    await ds.InsertSchool(tfName.Text, tfCity.Text, tfDistrict.Text, tfPhone.Text, tfStreet.Text, tfNumber.Text);

                    tfName.Text = "";
                    tfCity.Text = "";
                    tfDistrict.Text = "";
                    tfPhone.Text = "";
                    tfStreet.Text = "";
                    tfNumber.Text = "";
                }

            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int aux = await ds.UserCount();
            tfUsers.Text = aux.ToString();
            
        }
    }
}
