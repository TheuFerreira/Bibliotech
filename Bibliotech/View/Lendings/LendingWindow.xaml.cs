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
        public Lector Lector { get; set; } = new Lector();
        public Book book { get; set; } = new Book();
        public Exemplary exemplary { get; set; } = new Exemplary();
        public static bool isLector { get; set; }

        List<Book> books = new List<Book>();
        

        public LendingWindow()
        {
            InitializeComponent();
            isLector = false;
        }

        private void btnSearchLector_Click(object sender, RoutedEventArgs e)
        {
            SearchLectorWindow searchLector = new SearchLectorWindow();
            searchLector.ShowDialog();
            Lector = searchLector.lector;

            tfLectorRegister.Text = Lector.IdLector.ToString();
            tfNameLector.Text = Lector.Name.ToString();
        }

        private void btnSearchBook_Click(object sender, RoutedEventArgs e)
        {
            SearchBookWindow searchBook = new SearchBookWindow();
            searchBook.ShowDialog();
            book = searchBook.book;
            exemplary = searchBook.exemplary;
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            if (book != null)
            {
               //book.idExemplary = exemplary.IdIndex;
                books.Add(book);
                dataGrid.ItemsSource = books;
            }
        }
    }
}
