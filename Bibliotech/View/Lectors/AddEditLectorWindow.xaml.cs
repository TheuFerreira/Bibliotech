using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using Bibliotech.Singletons;
using System;
using System.Windows;

namespace Bibliotech.View.Lectors
{
    /// <summary>
    /// Lógica interna para AddEditLectorWindow.xaml
    /// </summary>
    public partial class AddEditLectorWindow : Window
    {
        private readonly DialogService dialogService = new DialogService();
        private readonly DAOLector daoLector = new DAOLector();
        private readonly Lector lector;
        private readonly Address address;
        private readonly int idBranch;

        public AddEditLectorWindow(Lector lector)
        {
            InitializeComponent();

            idBranch = Session.Instance.User.Branch.IdBranch;
            tbInfo.Text = "Adicionar Leitor";
            Title = tbInfo.Text;

            this.lector = lector;
            address = lector.Address;
            HistoryButton.IsEnabled = lector.IdLector != -1;

            if (lector.IdLector == -1)
            {
                return;
            }

            tbInfo.Text = "Editar Leitor";
            Title = tbInfo.Text;

            tfName.Text = lector.Name;
            tfUserRegistration.Text = lector.IdLector.ToString("D6");
            tfResponsible.Text = lector.Responsible;
            tfBirthDate.Text = lector.BirthDate.ToString();
            tfCity.Text = address.City;
            tfDistrict.Text = address.Neighborhood;
            tfStreet.Text = address.Street;
            tfNumber.Text = address.Number;
            tfComplement.Text = address.Complement;
            tfPhone.Text = lector.Phone.ToString();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(tfName.Text) || double.TryParse(tfName.Text, out _))
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
            lector.Name = tfName.Text;
            lector.Responsible = tfResponsible.Text;

            lector.BirthDate = null;
            if (DateTime.TryParse(tfBirthDate.Text, out DateTime temp))
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

        private void ClearFields()
        {
            tfName.Text = "";
            tfBirthDate.Text = "";
            tfCity.Text = "";
            tfResponsible.Text = "";
            tfStreet.Text = "";
            tfNumber.Text = "";
            tfUserRegistration.Text = "";
            tfComplement.Text = "";
            tfDistrict.Text = "";
            tfNumber.Text = "";
        }

        private async void Save_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            GroupArguments();

            if (lector.IdLector == -1)
            {
                if (!dialogService.ShowQuestion("Tem certeza que deseja adicionar este usuário?", ""))
                {
                    return;
                }

                btnSave.IsEnabled = false;
                if (!await daoLector.Insert(idBranch, lector, address))
                {
                    dialogService.ShowError("Algo deu errado!\nTente novamente.");
                    return;
                }

                dialogService.ShowSuccess("Leitor adicionado com sucesso!");
                btnSave.IsEnabled = true;
                ClearFields();

                return;
            }

            if (await daoLector.Update(lector, address))
            {
                if (!dialogService.ShowQuestion("Confirmação", "Tem certeza que deseja alterar este leitor?"))
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

            _ = new LectorHistoryWindow(lector.IdLector).ShowDialog();
        }
    }
}
