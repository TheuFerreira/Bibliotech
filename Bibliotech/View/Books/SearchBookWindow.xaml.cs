using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Singletons;
using Bibliotech.View.Lendings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para SearchBookWindow.xaml
    /// </summary>
    public partial class SearchBookWindow : Window
    {
        private DAOBook daoBook = new DAOBook();

        public Book book = new Book();

        public Exemplary exemplary = new Exemplary();
        //trocar pro krai do session
        private int idBranch = Session.Instance.User.Branch.IdBranch;

        public SearchBookWindow()
        {
            InitializeComponent();
            UpdateGrid();
        }

        private void OnOffControls(bool awaiting)
        {
            selectButton.IsEnabled = awaiting;
            searchField.IsEnabled = awaiting;
        }

        private async void UpdateGrid()
        {
            loading.Awaiting = true;
            OnOffControls(false);
            DataTable dataTable = await daoBook.FillSearchDataGrid(searchField.Text, idBranch);
            if (dataTable != null)
            {
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            loading.Awaiting = false;
            OnOffControls(true);
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }
            Close();
        }

       private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;

            if (row_selected != null)
            {
                book.IdBook = Convert.ToInt32(row_selected["id_book"].ToString());
                book.Title = row_selected["title"].ToString();
                book.Subtitle = row_selected["subtitle"].ToString();
                book.Author.Name = row_selected["autores"].ToString();
                book.PublishingCompany = row_selected["publishing_company"].ToString();

                exemplary.IdIndex = Convert.ToInt32(row_selected["id_index"].ToString());
                exemplary.IdExemplary = Convert.ToInt32(row_selected["id_exemplary"].ToString());
            }
        
        }

        private void searchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }
    }
}
