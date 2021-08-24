using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.UserControls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para BooksWindow.xaml
    /// </summary>
    public partial class BooksWindow : Window
    {
        private readonly DAOBook DAOBook;
        private Book book = new Book();
        private List<Book> books;
        
        public BooksWindow()
        {
            InitializeComponent();
            DAOBook = new DAOBook();
            books = new List<Book>();
        }
        private void DisableControls(UIElementCollection childs)
        {
            foreach (UserControl element in childs)
            {
                if (element is ButtonImage)
                {
                    (element).IsEnabled = false;
                }
                
            }
        }
        private void EnableControls(UIElementCollection childs)
        {
            foreach (UserControl element in childs)
            {
                if (element is ButtonImage)
                {
                    (element).IsEnabled = true;
                }
            }
        }
        private void IsEnabled(bool result)
        {
            loading.Awaiting = result;
            searchField.IsEnabled = !result;
        }
        private async Task SearchBooks()
        {
            DisableControls(gridPanel.Children);
            IsEnabled(true);
            string text = searchField.Text;
            books = await DAOBook.GetBook(text);
            dataGrid.ItemsSource = books;
            IsEnabled(false);
            EnableControls(gridPanel.Children);
        }
        private Book GetIdBook()
        {
            int index = dataGrid.SelectedIndex;
            Book book = books[index];
            return book;
        }
        private void ShowExemplaries()
        {
            book = GetIdBook();
            ExemplaryWindow exemplary = new ExemplaryWindow(book);
            exemplary.Show();
        }
        private async void BooksWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await SearchBooks();
        }
        private async void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            AddEditBookWindow addEditBookWindow = new AddEditBookWindow(book);
            addEditBookWindow.ShowDialog();
            await SearchBooks();
        }
        private async void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            book = GetIdBook();
            new AddEditBookWindow(book).ShowDialog();
            await SearchBooks();
        }
        private void BtnExemplary_OnClick(object sender, RoutedEventArgs e)
        {
            ShowExemplaries();
        }
        private async void SearchField_Click(object sender, RoutedEventArgs e)
        {
            await SearchBooks();
        }
    }
}
