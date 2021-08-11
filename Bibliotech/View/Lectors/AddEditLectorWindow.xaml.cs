using Bibliotech.Model.DAO;
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
        //mudar depois
        private int idBranch = 1;

        private DialogService dialogService = new DialogService();

        private DAOLector daoLector = new DAOLector();

        private Lector lector = new Lector();

        private Address address = new Address();

        public bool IsUpdate { get; set; }

        public AddEditLectorWindow(int id_branch, bool is_update, int idAddress)
        {
            InitializeComponent();

            idBranch = id_branch;
            IsUpdate = is_update;
            address.IdAddress = idAddress;
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

        private void GroupArguments()
        {
            if (IsUpdate)
            {
                lector.IdLector = Convert.ToInt32(tfUserRegistration.Text);
            }

            lector.Name = tfName.Text;

            lector.Responsible = tfResponsible.Text;

            lector.BirthDate = null;
            if(DateTime.TryParse(tfBirthDate.Text, out DateTime temp))
            {
                lector.BirthDate = temp;
            }

            lector.Phone = null;
            if (!string.IsNullOrEmpty(tfPhone.Text))
            {
                lector.Phone = Convert.ToInt64(tfPhone.Text);
            }

            address.City = tfCity.Text;

            address.Neighborhood = tfDistrict.Text;

            address.Street = tfStreet.Text;

            address.Number = tfNumber.Text;

            address.Complement = tfComplement.Text;
        }

        private async void save_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            GroupArguments();
            if (!IsUpdate)
            {
                if (!dialogService.ShowQuestion("Tem certeza que deseja adicionar este usuário?", ""))
                {
                    return;
                }
                if (!await daoLector.Insert(idBranch, lector, address))
                {
                    dialogService.ShowError("Algo deu errado!\nTente novamente.");
                    return;
                }
                dialogService.ShowSuccess("Leitor adicionado com sucesso!");
                return;
            }

            if(await daoLector.Update(lector, address))
            {
                if (!dialogService.ShowQuestion("Tem certeza que deseja alterar este usuário?", ""))
                {
                    return;
                }
                dialogService.ShowSuccess("Leitor alterado coom sucesso!");
                Close();
                return;
            }
            dialogService.ShowError("Algo deu errado!\nTente novamente.");
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            GroupArguments();

            LectorHistoryWindow historyWindow = new LectorHistoryWindow(lector.IdLector);

            if (IsUpdate)
            {
                _ = historyWindow.ShowDialog();
            }
        }
    }
}
