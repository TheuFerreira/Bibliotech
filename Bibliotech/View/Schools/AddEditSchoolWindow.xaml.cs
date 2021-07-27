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

        private int id;
        private bool isUpdate = false;
        private int id_address;

        public AddEditSchoolWindow()
        {
            InitializeComponent();
        }

        public int Id { get => id; set => id = value; }
        public bool IsUpdate { get => isUpdate; set => isUpdate = value; }
        public int Id_address { get => id_address; set => id_address = value; }

        public bool ValidateFields()
        {
            
            if((String.IsNullOrEmpty(tfName.Text)))
            {
                dialogService.ShowError("Escreva um nome válido! \nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }


            if((String.IsNullOrEmpty(tfCity.Text)))
            {
                dialogService.ShowError("Escreva um nome de cidade!\nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }


            if(String.IsNullOrEmpty(tfDistrict.Text))
            {
                dialogService.ShowError("Escreva o nome de um bairro!\nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }


            if (!String.IsNullOrEmpty(tfPhone.Text))
            {
                if (tfPhone.Text.Length <= 8)
                {
                    dialogService.ShowError("Insira um número de telefone válido!\nOu deixe este campo sem preencher.");
                    return false;
                }
            }


            if (String.IsNullOrEmpty(tfStreet.Text))
            {
                dialogService.ShowError("Escreva o nome de uma rua!\nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }


            if (String.IsNullOrEmpty(tfNumber.Text))
            {
                dialogService.ShowError("Escreva um número!\nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }

            return true;
            
        }

        private void ClearFields()
        {
            tfName.Text = "";
            tfCity.Text = "";
            tfDistrict.Text = "";
            tfPhone.Text = "";
            tfStreet.Text = "";
            tfNumber.Text = "";
        }

        private async void btnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
              
                if (!isUpdate)
                {
                    long? phone = null;
                    if (long.TryParse(tfPhone.Text, out long temp))
                    {
                        phone = temp;
                    }

                    if(await ds.Insert(tfName.Text, tfCity.Text, tfDistrict.Text, phone, tfStreet.Text, tfNumber.Text))
                    {
                        dialogService.ShowSuccess("Usuário salvo com sucesso");
                        ClearFields();
                        return;
                    }
                    
                     dialogService.ShowError("Algo deu errado\nTente novamente");

                }
                else
                {
                    long? phone = null;
                    if(long.TryParse(tfPhone.Text, out long temp))
                    {
                        phone = temp;
                    }

                    if(await ds.Update(Id, tfName.Text, tfCity.Text, tfDistrict.Text, phone, tfStreet.Text, tfNumber.Text, Id_address))
                    {
                        dialogService.ShowSuccess("Usuário salvo com sucesso");
                        Close();
                        return;
                    }

                    dialogService.ShowError("Algo deu errado\nTente novamente");
                }

            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int aux = await ds.Count();
            tfUsers.Text = aux.ToString();
        }
    }
}
