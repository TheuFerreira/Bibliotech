using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using System.Data;
using System.Windows;

namespace Bibliotech.View.Schools
{
    /// <summary>
    /// Lógica interna para SearchSchoolWindow.xaml
    /// </summary>
    public partial class SearchSchoolWindow : Window
    {
        public School Branch { get; set; }

        private readonly DAOSchool daoSchool;

        public SearchSchoolWindow()
        {
            InitializeComponent();

            daoSchool = new DAOSchool();
        }

        private async void LoadSchools()
        {
            string text = searchField.Text;

            dataGrid.ItemsSource = (await daoSchool.FillDataGrid(text)).DefaultView;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSchools();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadSchools();
        }

        private async void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            DataRowView row = dataGrid.SelectedItem as DataRowView;
            int idBranch = int.Parse(row["id_branch"].ToString());

            Branch = await daoSchool.GetById(idBranch);
            DialogResult = true;

            Close();
        }
    }
}
