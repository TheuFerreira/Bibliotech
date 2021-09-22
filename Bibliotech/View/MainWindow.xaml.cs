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
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Bibliotech.View
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DialogService dialogService;
        private User loggedUser;

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

                AddEditSchoolWindow addEditSchool = new AddEditSchoolWindow(new Branch(), true);
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

            new LoginWindow().ShowDialog();

            txtVersion.Text = "Versão " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            loggedUser = Session.Instance.User;
            if (loggedUser == null)
            {
                return;
            }
        }

        private void BtnOut_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            _ = new LoginWindow().ShowDialog();

            loggedUser = Session.Instance.User;

            if (loggedUser != null)
            {
                Show();
                MainWindow_Loaded(sender, e);
            }
            else
            {
                Close();
            }
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/TheuFerreira/Bibliotech/tree/main");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Session.Instance.User == null)
            {
                Close();
                return;
            }

            tbText.Text = "BEM VINDO, " + Session.Instance.User.Name;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Session.Instance.User == null)
            {
                return;
            }

            bool result = dialogService.ShowQuestion("ATENÇÃO", "Deseja sair do programa?");

            if (!result)
            {
                e.Cancel = true;
            }

        }

        private void BtnLectors_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new LectorsWindow().ShowDialog();
        }

        private void BtnLendings_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new LendingWindow().ShowDialog();
        }

        private void BtnDevolutions_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new DevolutionWindow().ShowDialog();
        }

        private void BtnBooks_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new BooksWindow().ShowDialog();
        }

        private void BtnUsers_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new UsersWindow().ShowDialog();
        }

        private void BtnReports_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new ReportsWindow().ShowDialog();
        }

        private async void BtnSchools_OnClick(object sender, RoutedEventArgs e)
        {
            if (loggedUser.IsController())
            {
                _ = new SchoolsWindow().ShowDialog();
                return;
            }

            int idBranch = loggedUser.Branch.IdBranch;
            Branch branch = await new DAOBranch().GetById(idBranch);

            _ = new AddEditSchoolWindow(branch).ShowDialog();
        }

        private void BtnManual_OnClick(object sender, RoutedEventArgs e)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"Manual\manual.pdf";

            FileService fileService = new FileService();
            if (fileService.FileExists(filePath) == false)
            {
                dialogService.ShowError("O Manual não foi encontrado!!!");
                return;
            }

            if (fileService.IsFileOpen(filePath))
            {
                dialogService.ShowError("O Manual, já está aberto!!!");
                return;
            }

            Process.Start(filePath);
            dialogService.ShowInformation("Abrindo Manual!!!");
        }
    }
}
