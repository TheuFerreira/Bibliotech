using Bibliotech.Model.Entities;
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

namespace Bibliotech.View.Lectors
{
    /// <summary>
    /// Lógica interna para AddEditLectorWindow.xaml
    /// </summary>
    public partial class AddEditLectorWindow : Window
    {
        DialogService dialogService = new DialogService();
        bool isUpdate;

        public bool IsUpdate { get => isUpdate; set => isUpdate = value; }

        public AddEditLectorWindow()
        {
            InitializeComponent();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty((tfName.Text)) || (double.TryParse(tfName.Text, out _)))
            {
                dialogService.ShowError("Escreva um nome válido! \nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }

            if (double.TryParse(tfResponsible.Text, out _))
            {
                dialogService.ShowError("Escreva um nome válido! \nOu deixe este campo sem preencher.");
                return false;
            }

            if (string.IsNullOrEmpty(tfCity.Text))
            {
                dialogService.ShowError("Escreva um nome de cidade!\nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }

            if (string.IsNullOrEmpty(tfDistrict.Text))
            {
                dialogService.ShowError("Escreva o nome de um bairro!\nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }

            if (!string.IsNullOrEmpty(tfPhone.Text))
            {
                if (tfPhone.Text.Length <= 8)
                {
                    dialogService.ShowError("Insira um número de telefone válido!\nOu deixe este campo sem preencher.");
                    return false;
                }
            }

            if (string.IsNullOrEmpty(tfStreet.Text))
            {
                dialogService.ShowError("Escreva o nome de uma rua!\nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }

            if (string.IsNullOrEmpty(tfNumber.Text))
            {
                dialogService.ShowError("Escreva um número!\nEste campo é obrigatório, portanto preencha-o.");
                return false;
            }

            if (!string.IsNullOrEmpty(tfBirthDate.Text))
            {
                if (tfBirthDate.Text.Length < 10)
                {
                    dialogService.ShowError("Escreva uma data de nascimento válida!\nOu deixe este campo sem preencher.");
                    return false;
                }
            }

            return true;
        }


        private void save_OnClick(object sender, RoutedEventArgs e)
        {
            ValidateFields();
        }
    }
}
