using Bibliotech.BarCode;
using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using System.Collections.Generic;
using System.Windows;

namespace Bibliotech.View.Schools
{
    /// <summary>
    /// Lógica interna para AddEditSchoolWindow.xaml
    /// </summary>
    public partial class AddEditSchoolWindow : Window
    {
        private readonly DialogService dialogService = new DialogService();
        private readonly FileService fileService = new FileService();
        private readonly DAOBranch daoSchool = new DAOBranch();
        private readonly bool isFirstBranch;
        private readonly Branch branch;

        private void FillFieldWithBranchInfo()
        {
            if (branch.IdBranch == -1)
            {
                return;
            }

            tfName.Text = branch.Name;
            tfCity.Text = branch.Address.City;
            tfDistrict.Text = branch.Address.Neighborhood;
            tfPhone.Text = branch.Telephone.ToString();
            tfStreet.Text = branch.Address.Street;
            tfNumber.Text = branch.Address.Number;

            tbInfo.Text = "Editar Escola";
            Title = tbInfo.Text;
        }

        public AddEditSchoolWindow(Branch branch, bool isFirstBranch = false)
        {
            InitializeComponent();

            this.isFirstBranch = isFirstBranch;
            this.branch = branch;

            tbInfo.Text = "Adicionar Escola";
            Title = tbInfo.Text;

            FillFieldWithBranchInfo();
        }

        public bool ValidateFields()
        {
            if (string.IsNullOrEmpty(tfName.Text))
            {
                dialogService.ShowError("Escreva um nome válido! \nEste campo é obrigatório, portanto preencha-o.");
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

            return true;
        }

        private long? ValidatePhone()
        {
            long? phone = null;
            if (long.TryParse(tfPhone.Text, out long temp))
            {
                phone = temp;
            }

            return phone;
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

        private void SetButtons(bool value)
        {
            btnSave.IsEnabled = value;
            btnGeneratePDF.IsEnabled = value;
        }

        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            long? phone = ValidatePhone();

            Address address = branch.Address;
            address.City = tfCity.Text;
            address.Neighborhood = tfDistrict.Text;
            address.Street = tfStreet.Text;
            address.Number = tfNumber.Text;

            branch.Address = address;
            branch.Name = tfName.Text;
            branch.Telephone = phone;
            branch.Address = address;

            SetButtons(false);
            bool result = await daoSchool.Save(branch);
            SetButtons(true);

            if (result == false)
            {
                dialogService.ShowError("Algo deu errado\nTente novamente");
                return;
            }

            dialogService.ShowSuccess("Escola salva com sucesso");
            ClearFields();

            if (isFirstBranch)
            {
                dialogService.ShowInformation("Agora você será redicionado para a tela de usuários, para cadastrar o primeiro usuário!!!");
                DialogResult = true;
            }

            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int aux = await daoSchool.UsersCount(branch);
            tfUsers.Text = aux.ToString();

            btnGeneratePDF.Visibility = branch.IsNew() ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void ButtonGeneratePDF_Click(object sender, RoutedEventArgs e)
        {
            SetButtons(false);

            List<Exemplary> exemplaries = await new DAOExamplary().GetAllExemplariesByBranch(branch);
            string path = dialogService.SaveFileDialg("PDF Files|*.pdf", "pdf", "Código de Barras");
            if (fileService.IsFileOpen(path))
            {
                dialogService.ShowError("O arquivo já está aberto em outro programa. \\ Por favor, feche-o");
            }

            new GenerateAndPrintBarCorde().BaseDocument(exemplaries, branch, path);
            dialogService.ShowInformation("PDF gerado com sucesso!!!");

            SetButtons(true);
        }
    }
}
