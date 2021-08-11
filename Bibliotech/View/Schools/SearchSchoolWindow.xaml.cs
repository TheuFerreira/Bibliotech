using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Bibliotech.View.Schools
{
    /// <summary>
    /// Lógica interna para SearchSchoolWindow.xaml
    /// </summary>
    public partial class SearchSchoolWindow : Window
    {
        public Branch Branch { get; set; }

        private readonly DAOBranch daoSchool;
        private List<Branch> branches;

        public SearchSchoolWindow()
        {
            InitializeComponent();

            daoSchool = new DAOBranch();
        }

        private void DisableButtons()
        {
            searchField.IsEnabled = false;
            btnSelect.IsEnabled = false;
        }

        private void EnableButtons()
        {
            searchField.IsEnabled = true;
            btnSelect.IsEnabled = true;
        }
        private async void LoadSchools()
        {
            string text = searchField.Text;
            DisableButtons();
            branches = await daoSchool.FillDataGrid(text);
            dataGrid.ItemsSource = branches;
            EnableButtons();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSchools();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadSchools();
        }

        private Branch GetBranchInSelectedRow()
        {
            int selectedIndex = dataGrid.SelectedIndex;
            return branches[selectedIndex];
        }

        private async void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            Branch branch = GetBranchInSelectedRow();
            int idBranch = branch.IdBranch;

            Branch = await daoSchool.GetById(idBranch);
            DialogResult = true;

            Close();
        }
    }
}
