using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using Bibliotech.Singletons;
using Bibliotech.View.Books;
using Bibliotech.View.Devolutions;
using Bibliotech.View.Lectors;
using Bibliotech.View.Lendings;
using Bibliotech.View.Reports;
using Bibliotech.View.Schools;
using Bibliotech.View.Users;
using System.Windows;

namespace Bibliotech.View
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private User User;
        private readonly DialogService dialogService;

        private async void FirstLogin()
        {
            Server serverSettings = Session.Instance.Server;
            if (string.IsNullOrEmpty(serverSettings.ServerName))
            {
                bool? result = new ServerWindow().ShowDialog();
                if (result.Value != true)
                {
                    return;
                }
            }

            DAOBranch daoBranch = new DAOBranch();
            int totalBranches = await daoBranch.Total();
            if (totalBranches == 0)
            {
                dialogService.ShowInformation("Você será redirecionado para a tela de Escolas, para cadastrar a primeira escola!!!");

                AddEditSchoolWindow addEditSchool = new AddEditSchoolWindow(false, true);
                bool? result = addEditSchool.ShowDialog();

                if (result != true)
                {
                    return;
                }
            }

            DAOUser daoUser = new DAOUser();
            int totalUsers = await daoUser.Total();
            if (totalUsers == 0)
            {
                AddEditUserWindow addEditUser = new AddEditUserWindow(new User(), true);
                bool? result = addEditUser.ShowDialog();

                if (result != true)
                {
                    return;
                }
            }

            return;
        }

        public MainWindow()
        {
            InitializeComponent();
            dialogService = new DialogService();

            FirstLogin();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            User = loginWindow.User;

            if (User == null)
            {
                return;
            }

            WindowState = WindowState.Maximized;
        }

        private void BtnOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            User = loginWindow.User;

            if (User != null)
            {
                Show();
                MainWindow_Loaded(sender, e);
            }
            else
            {
                Close();
            }

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (User == null)
            {
                Close();
                return;
            }

            tbText.Text = "BEM VINDO, " + User.Name;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (User == null) return;

            bool result = dialogService.ShowQuestion("ATENÇÃO", "Deseja sair do programa?");

            if (!result)
            {
                e.Cancel = true;
            }

        }

        private void BtnLectors_OnClick(object sender, RoutedEventArgs e)
        {
            LectorsWindow lectorsWindow = new LectorsWindow();
            lectorsWindow.Show();
        }

        private void BtnLendings_OnClick(object sender, RoutedEventArgs e)
        {
            LendingWindow lendingWindow = new LendingWindow();
            lendingWindow.Show();
        }

        private void BtnDevolutions_OnClick(object sender, RoutedEventArgs e)
        {
            DevolutionWindow devolutionWindow = new DevolutionWindow();
            devolutionWindow.Show();
        }

        private void BtnBooks_OnClick(object sender, RoutedEventArgs e)
        {
            BooksWindow booksWindow = new BooksWindow();
            booksWindow.Show();
        }

        private void BtnUsers_OnClick(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow();
            usersWindow.Show();
        }

        private void BtnReports_OnClick(object sender, RoutedEventArgs e)
        {
            ReportsWindow reportsWindow = new ReportsWindow();
            reportsWindow.Show();
        }

        private void BtnSchools_OnClick(object sender, RoutedEventArgs e)
        {
            SchoolsWindow schoolsWindow = new SchoolsWindow();
            schoolsWindow.Show();
        }
    }
}
