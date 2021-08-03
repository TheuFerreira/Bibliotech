﻿using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using System.Windows;

namespace Bibliotech.View.Schools
{
    /// <summary>
    /// Lógica interna para AddEditSchoolWindow.xaml
    /// </summary>
    public partial class AddEditSchoolWindow : Window
    {
        private readonly DialogService dialogService = new DialogService();
        private readonly DAOBranch daoSchool = new DAOBranch();
        private readonly bool isFirstBranch;

        public int Id { get; set; }
        public bool IsUpdate { get; set; }
        public int Id_address { get; set; }

        public AddEditSchoolWindow(bool isUpdate = false, bool isFirstBranch = false)
        {
            InitializeComponent();

            IsUpdate = isUpdate;

            this.isFirstBranch = isFirstBranch;
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

        private void ClearFields()
        {
            tfName.Text = "";
            tfCity.Text = "";
            tfDistrict.Text = "";
            tfPhone.Text = "";
            tfStreet.Text = "";
            tfNumber.Text = "";
        }

        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            long? phone = null;
            if (long.TryParse(tfPhone.Text, out long temp))
            {
                phone = temp;
            }

            Address address = new Address()
            {
                IdAddress = Id_address,
                City = tfCity.Text,
                Neighborhood = tfDistrict.Text,
                Street = tfStreet.Text,
                Number = tfNumber.Text,
            };

            Branch school = new Branch()
            {
                Name = tfName.Text,
                Telephone = phone,
                Address = address,
            };

            if (!IsUpdate)
            {
                if (await daoSchool.Insert(school))
                {
                    dialogService.ShowSuccess("Escola salva com sucesso");
                    ClearFields();

                    if (isFirstBranch)
                    {
                        dialogService.ShowInformation("Agora você será redicionado para a tela de usuários, para cadastrar o primeiro usuário!!!");
                        DialogResult = true;
                        Close();
                    }
                    return;
                }
            }
            else
            {
                if (await daoSchool.Update(Id, school))
                {
                    dialogService.ShowSuccess("Escola salva com sucesso");
                    Close();
                    return;
                }
            }

            dialogService.ShowError("Algo deu errado\nTente novamente");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int aux = await daoSchool.UsersCount();
            tfUsers.Text = aux.ToString();
        }
    }
}
