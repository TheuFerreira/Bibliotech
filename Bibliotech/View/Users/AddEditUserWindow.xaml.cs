using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using Bibliotech.UserControls;
using Bibliotech.View.Schools;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.View.Users
{
    /// <summary>
    /// Lógica interna para AddEditUserWindow.xaml
    /// </summary>
    public partial class AddEditUserWindow : Window
    {
        public School Branch { get; set; }

        private User user;
        private readonly DialogService dialogService;
        private readonly DAOUser daoUser;

        public AddEditUserWindow(User user)
        {
            InitializeComponent();

            dialogService = new DialogService();
            daoUser = new DAOUser();
            Branch = user.Branch;

            List<string> typesUser = Enum.GetValues(typeof(TypeUser))
                .Cast<TypeUser>()
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();
            cbTypeUser.ItemsSource = typesUser;
            cbTypeUser.SelectedIndex = 0;

            this.user = user;
            Title = "Adicionar Usuário";
            tbInfo.Text = "Adicionar Usuário";

            if (user.IdUser == -1)
            {
                return;
            }

            Title = "Editar Usuário";
            tbInfo.Text = "Editar Usuário";

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

        private void ButtonSearchSchool_Click(object sender, RoutedEventArgs e)
        {
            SearchSchoolWindow searchSchool = new SearchSchoolWindow();
            bool? result = searchSchool.ShowDialog();

            if (result == false)
            {
                return;
            }

            Branch.Id_branch = searchSchool.Branch.Id_branch;
            Branch.Name = searchSchool.Branch.Name;
        }

        private void ClearControls(UIElementCollection childs)
        {
            foreach (UIElement element in childs)
            {
                if (element is TextField)
                {
                    (element as TextField).Text = string.Empty;
                }
                else if (element is ComboBox)
                {
                    (element as ComboBox).SelectedIndex = 0;
                }
                else if (element is Grid)
                {
                    Grid grid = element as Grid;
                    ClearControls(grid.Children);
                }
            }
        }

        private async void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tfName.Text)
                || string.IsNullOrEmpty(tfUserName.Text)
                || string.IsNullOrEmpty(tfPassword.Text)
                || string.IsNullOrEmpty(tfCity.Text)
                || string.IsNullOrEmpty(tfStreet.Text)
                || string.IsNullOrEmpty(tfDistrict.Text)
                || string.IsNullOrEmpty(tfNumber.Text))
            {
                dialogService.ShowError("Preencha todos os campos com *!!!");
                return;
            }

            if (Branch.Id_branch == -1)
            {
                dialogService.ShowError("Selecione uma escola para o Usuário!!!");
                return;
            }

            if (user.UserName != tfUserName.Text)
            {
                if (await daoUser.UserNameExists(tfUserName.Text))
                {
                    dialogService.ShowError("Nome de Usuário já existe!!!");
                    return;
                }
            }

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
            user.Branch = Branch;

            Address address = user.Address;
            address.City = tfCity.Text;
            address.Street = tfStreet.Text;
            address.Neighborhood = tfDistrict.Text;
            address.Number = tfNumber.Text;
            address.Complement = tfComplement.Text;
            user.Address = address;

            await daoUser.Save(user);
            dialogService.ShowSuccess("Usuário Salvo com Sucesso!!!");

            if (user.IdUser == -1)
            {
                ClearControls(gridFields.Children);

                user = new User();

                Branch.Id_branch = -1;
                Branch.Name = string.Empty;
            }
            else
            {
                Close();
            }
        }
    }
}
