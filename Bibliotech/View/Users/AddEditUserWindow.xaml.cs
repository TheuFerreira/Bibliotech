using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using Bibliotech.UserControls;
using Bibliotech.UserControls.CustomDialog;
using Bibliotech.View.Schools;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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

        private void FillScreenWithDatas()
        {
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

            FillScreenWithDatas();
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

        private async Task<bool> ValidatedFields()
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
                return false;
            }

            if (Branch.Id_branch == -1)
            {
                dialogService.ShowError("Selecione uma escola para o Usuário!!!");
                return false;
            }

            if (user.UserName == tfUserName.Text)
            {
                return true;
            }

            if (await daoUser.UserNameExists(tfUserName.Text))
            {
                dialogService.ShowError("Nome de Usuário já existe!!!");
                return false;
            }

            return true;
        }

        private bool ValidatedBirthDate()
        {
            user.BirthDate = null;
            if (string.IsNullOrEmpty(tfBirthDate.Text))
            {
                return true;
            }

            bool result = DateTime.TryParseExact(tfBirthDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datetime);
            if (result == false)
            {
                dialogService.ShowError("Data de Nascimento Inválida!!!");
                return false;
            }

            user.BirthDate = datetime;
            return true;
        }

        private bool ValidatedTelephone()
        {
            user.Telephone = null;
            if (string.IsNullOrEmpty(tfPhone.Text))
            {
                return true;
            }

            if (tfPhone.Text.Length < 10)
            {
                dialogService.ShowError("O telefone precisa de pelo menos 10 dígitos!!!");
                return false;
            }

            user.Telephone = long.Parse(tfPhone.Text);
            return true;
        }

        private bool ValidatedPassword()
        {
            TextFieldDialog fieldDialog = new TextFieldDialog();
            bool? result = fieldDialog.ShowDialog();
            if (result == false)
            {
                return false;
            }

            if (tfPassword.Text != fieldDialog.Text)
            {
                dialogService.ShowError("As Senhas não coincidem!!!");
                return false;
            }

            return true;
        }

        private TypeUser ComboBoxToEnum()
        {
            return Enums.Parse<TypeUser>(cbTypeUser.SelectedItem.ToString(), true, EnumFormat.Description);
        }

        private Address SetAddress()
        {
            Address address = user.Address;
            address.City = tfCity.Text;
            address.Street = tfStreet.Text;
            address.Neighborhood = tfDistrict.Text;
            address.Number = tfNumber.Text;
            address.Complement = tfComplement.Text;

            return address;
        }

        private async void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (await ValidatedFields() == false)
            {
                return;
            }

            if (ValidatedBirthDate() == false)
            {
                return;
            }

            if (ValidatedTelephone() == false)
            {
                return;
            }

            if (ValidatedPassword() == false)
            {
                return;
            }

            user.Name = tfName.Text;
            user.TypeUser = ComboBoxToEnum();
            user.UserName = tfUserName.Text;
            user.Password = tfPassword.Text;
            user.Branch = Branch;
            user.Address = SetAddress();

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
