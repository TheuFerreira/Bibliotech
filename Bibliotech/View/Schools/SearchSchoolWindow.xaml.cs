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
        public Branch Branch { get; set; }

        private readonly DAOBranch daoSchool;

        public SearchSchoolWindow()
        {
            InitializeComponent();

            daoSchool = new DAOBranch();
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

        private int GetIdInSelectedRow()
        {
            DataRowView row = dataGrid.SelectedItem as DataRowView;
            return int.Parse(row["id_branch"].ToString());
        }
        
        private async void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            int idBranch = GetIdInSelectedRow();

            Branch = await daoSchool.GetById(idBranch);
            DialogResult = true;

            Close();
        }
    }
}
