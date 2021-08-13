using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.UserControls.CustomDialog;
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

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para BooksWindow.xaml
    /// </summary>
    public partial class BooksWindow : Window
    {
        public readonly DAOBook DAOBook;
        Book book = new Book();
        List<Book> books;
        public BooksWindow()
        {
            InitializeComponent();
            DAOBook = new DAOBook();
            books = new List<Book>();

        }
        private async Task SearchBooks()
        {
            books = await DAOBook.GetBook();
            dataGrid.ItemsSource = books;
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
        private void ShowMessage(string title, string contents, TypeDialog typeDialog)
        {
            InformationDialog dialog = new InformationDialog(title, contents, typeDialog);
            dialog.ShowDialog();
        }
        private async void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (dataGrid.SelectedItem == null)
            {
                return;
            }

            int index = dataGrid.SelectedIndex;
            Book book = books[index];

            new AddEditBookWindow(book).ShowDialog();
            await SearchBooks();
        }

        private void BtnExemplary_OnClick(object sender, RoutedEventArgs e)
        {
            ExemplaryWindow exemplaryWindow = new ExemplaryWindow();
            exemplaryWindow.ShowDialog();
            
        }
    }
}
