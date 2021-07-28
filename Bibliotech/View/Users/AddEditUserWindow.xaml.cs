using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Bibliotech.View.Users
{
    /// <summary>
    /// Lógica interna para AddEditUserWindow.xaml
    /// </summary>
    public partial class AddEditUserWindow : Window
    {
        private readonly User user;
        private readonly DialogService dialogService;
        private readonly DAOUser daoUser;

        public AddEditUserWindow(User user)
        {
            InitializeComponent();

            dialogService = new DialogService();
            daoUser = new DAOUser();

            List<string> typesUser = Enum.GetValues(typeof(TypeUser))
                .Cast<TypeUser>()
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();
            cbTypeUser.ItemsSource = typesUser;
            cbTypeUser.SelectedIndex = 0;

            this.user = user;

            if (user.IdUser == -1)
                return;

            tfName.Text = user.Name;
            cbTypeUser.SelectedItem = user.TypeUser.AsString(EnumFormat.Description);
            tfBirthDate.Text = user.GetBirthDate();
            tfUserName.Text = user.UserName;
            tfPassword.Text = user.Password;
            tfPhone.Text = user.GetTelephone();
            tfCity.Text = user.Address.City;
            tfStreet.Text = user.Address.Street;
            tfDistrict.Text = user.Address.Neighborhood;
            tfNumber.Text = user.Address.Number;
            tfComplement.Text = user.Address.Complement;
        }

        private async void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            DateTime? birthDate = null;
            if (string.IsNullOrEmpty(tfBirthDate.Text) == false)
            {
                bool result = DateTime.TryParseExact(tfBirthDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datetime);
                if (result == false)
                {
                    dialogService.ShowError("Data de Nascimento Inválida!!!");
                    return;
                }

                birthDate = datetime;
            }

            long? telephone = null;
            if (string.IsNullOrEmpty(tfPhone.Text) == false)
            {
                if (tfPhone.Text.Length < 10)
                {
                    dialogService.ShowError("O telefone precisa de pelo menos 10 dígitos!!!");
                    return;
                }

                telephone = long.Parse(tfPhone.Text);
            }

            user.Name = tfName.Text;
            user.TypeUser = Enums.Parse<TypeUser>(cbTypeUser.SelectedItem.ToString(), true, EnumFormat.Description);
            user.BirthDate = birthDate;
            user.UserName = tfUserName.Text;
            user.Password = tfPassword.Text;
            user.Telephone = telephone;

            School school = new School();
            school.Id_branch = 1;
            user.Branch = school;

            Address address = user.Address;
            address.City = tfCity.Text;
            address.Street = tfStreet.Text;
            address.Neighborhood = tfDistrict.Text;
            address.Number = tfNumber.Text;
            address.Complement = tfComplement.Text;
            user.Address = address;

            await daoUser.Save(user);
            dialogService.ShowSuccess("Usuário Inserido com Sucesso!!!");
        }
    }
}
