using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.View.Books;
using Bibliotech.View.Lectors;
using System;
using System.Collections.Generic;
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

namespace Bibliotech.View.Lendings
{
    /// <summary>
    /// Lógica interna para LendingWindow.xaml
    /// </summary>
    public partial class LendingWindow : Window
    {
        private Lector lector { get; set; } = new Lector();

        private Book book { get; set; } = new Book();

        private Exemplary exemplary { get; set; } = new Exemplary();

        DAOLending daoLending = new DAOLending();

        List<Book> books = new List<Book>();

        List<Exemplary> exemplaries = new List<Exemplary>();
        
        public LendingWindow()
        {
            InitializeComponent();
            book = null;
        }

        private void UpdateGrid()
        {
            if (book == null)
            {
                return;
            }
            if (exemplary.IdIndex <= 0)
            {
                return;
            }
            book.idExemplary = exemplary.IdIndex;
            books.Add(book);
            exemplaries.Add(exemplary);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = books;
        }

        private bool ValidateFields()
        {
            if(string.IsNullOrEmpty(tfLectorRegister.Text))
            {
                return false;
            }

            if(string.IsNullOrEmpty(tfNameLector.Text))
            {
                return false;
            }

            if(dataGrid.Items == null)
            {
                return false;
            }

            return true;
        }

        private void OnOffControls(bool validate)
        {
            loading.Awaiting = !validate;
            addButton.IsEnabled = validate;
            dataGrid.IsEnabled = validate;
            btnSearchBook.IsEnabled = validate;
            btnSearchLector.IsEnabled = validate;
        }

        private void btnSearchLector_Click(object sender, RoutedEventArgs e)
        {
            SearchLectorWindow searchLector = new SearchLectorWindow();
            searchLector.ShowDialog();
            lector = searchLector.lector;
            if(lector.Name == null)
            {
                return;
            }
            tfLectorRegister.Text = lector.IdLector.ToString();
            tfNameLector.Text = lector.Name.ToString();
        }

        private void btnSearchBook_Click(object sender, RoutedEventArgs e)
        {
            SearchBookWindow searchBook = new SearchBookWindow();
            searchBook.ShowDialog();
            book = searchBook.book;
            exemplary = searchBook.exemplary;
            UpdateGrid();
        }

        private async void addButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(!ValidateFields())
            {
                return;
            }

            DateTime begin = DateTime.Parse(dtpBegin.date.Text);
            DateTime end = DateTime.Parse(dtpEnd.date.Text);

            OnOffControls(false);
            for (int i = 0; i < books.Count; i++)
            {
                 await daoLending.Insert(books[i], exemplaries[i], lector, begin, end);    
            }
            dataGrid.ItemsSource = null;
            OnOffControls(true);
        }

        private void GridCellDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
