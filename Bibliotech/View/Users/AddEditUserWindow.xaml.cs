using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using Bibliotech.Singletons;
using Bibliotech.UserControls;
using Bibliotech.UserControls.CustomDialog;
using Bibliotech.View.Schools;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
        public Branch Branch { get; set; }

        private readonly DialogService dialogService;
        private readonly DAOUser daoUser;
        private readonly bool isFirstUser = false;
        private User user;

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

        public AddEditUserWindow(User user, bool isFirstUser = false)
        {
            InitializeComponent();

            dialogService = new DialogService();
            daoUser = new DAOUser();
            Branch = user.Branch;
            this.isFirstUser = isFirstUser;

            User loggedUser = Session.Instance.User;
            List<string> typesUser = Enum.GetValues(typeof(TypeUser))
                .Cast<TypeUser>()
                .Where(x =>
                {
                    if (isFirstUser && x == TypeUser.User)
                    {
                        return false;
                    }
                    else if (isFirstUser && x == TypeUser.Controller)
                    {
                        return true;
                    }
                    else if (loggedUser.IsUser() && x == TypeUser.Controller)
                    {
                        return false;
                    }

                    return true;
                })
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();
            cbTypeUser.ItemsSource = typesUser;
            cbTypeUser.SelectedIndex = 0;

            this.user = user;
            Title = "Adicionar Usuário";
            tbInfo.Text = "Adicionar Usuário";

            if (loggedUser == null)
            {
                return;
            }

            btnSearchSchools.IsEnabled = loggedUser.IsController();

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

            Branch.IdBranch = searchSchool.Branch.IdBranch;
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

            if (Branch.IdBranch == -1)
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
            string text = dialogService.ShowPasswordDialog("Para salvar, é necessário verificar sua identidade!!!");

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (tfPassword.Text != text)
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

        private void SetUser()
        {
            user.Name = tfName.Text;
            user.TypeUser = ComboBoxToEnum();
            user.UserName = tfUserName.Text;
            user.Password = tfPassword.Text;
            user.Branch = Branch;
            user.Address = SetAddress();
        }

        private void SetButtons(bool isEnabled)
        {
            save.IsEnabled = isEnabled;
            btnSearchSchools.IsEnabled = isEnabled;
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

            SetUser();
            SetButtons(false);

            await daoUser.Save(user);

            SetButtons(true);
            dialogService.ShowSuccess("Usuário Salvo com Sucesso!!!");

            if (user.IdUser == -1)
            {
                ClearControls(gridFields.Children);

                user = new User();

                Branch.IdBranch = -1;
                Branch.Name = string.Empty;

                if (isFirstUser)
                {
                    dialogService.ShowInformation("Agora você poderá fazer login no BIBLIOTECH, com o usuário cadastrado!!!");

                    Close();
                }
            }
            else
            {
                Close();
            }
        }
    }
}
