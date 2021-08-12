using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using System.Collections.Generic;
using System.Windows;

namespace Bibliotech.View.Schools
{
    /// <summary>
    /// Lógica interna para SchoolsWindow.xaml
    /// </summary>
    public partial class SchoolsWindow : Window
    {
        private readonly DialogService dialogService;
        private readonly DAOBranch daoSchool;
        private List<Branch> branches;

        public SchoolsWindow()
        {
            InitializeComponent();

            dialogService = new DialogService();
            daoSchool = new DAOBranch();
        }

        private void DisableButtons()
        {
            btnEdit.IsEnabled = false;
            btnAdd.IsEnabled = false;
            btnDelete.IsEnabled = false;
            searchField.IsEnabled = false;
            loading.Awaiting = true;
        }

        private void EnableButtons()
        {
            btnEdit.IsEnabled = true;
            btnAdd.IsEnabled = true;
            btnDelete.IsEnabled = true;
            searchField.IsEnabled = true;
            loading.Awaiting = false;
        }

        private async void UpdateGrid()
        {
            string text = searchField.Text;
            DisableButtons();
            branches = await daoSchool.FillDataGrid(text);
            dataGrid.ItemsSource = branches;
            EnableButtons();
        }

        private async void ButtonOnOff_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            int selectedIndex = dataGrid.SelectedIndex;
            Branch branch = branches[selectedIndex];
            DisableButtons();
            loading.Awaiting = true;
            if (branch.IsActive())
            {
                if (dialogService.ShowQuestion("Tem certeza que deseja\ndesativar esta escola?", ""))
                {
                    await daoSchool.OnOff(Status.Inactive, branch);
                    dialogService.ShowSuccess("Desativado com sucesso!");
                }
            }
            else
            {
                if (dialogService.ShowQuestion("Tem certeza que deseja\nativar esta escola?", ""))
                {
                    await daoSchool.OnOff(Status.Active, branch);
                    dialogService.ShowSuccess("Ativado com sucesso!");
                }
            }
            loading.Awaiting = false;
            EnableButtons();

            UpdateGrid();
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            int selectedIndex = dataGrid.SelectedIndex;
            Branch branch = branches[selectedIndex];

            _ = new AddEditSchoolWindow(branch).ShowDialog();

            UpdateGrid();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            _ = new AddEditSchoolWindow(new Branch()).ShowDialog();

            UpdateGrid();
        }

        public void SchoolGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void SearchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }
    }
}
