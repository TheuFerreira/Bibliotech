using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
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

        DialogService dialogService = new DialogService();

        DAOLending daoLending = new DAOLending();

        List<Book> books = new List<Book>();

        List<Exemplary> exemplaries = new List<Exemplary>();
        
        public LendingWindow()
        {
            InitializeComponent();
            book = null;

            dtpBegin.date.Text = DateTime.Now.Date.ToShortDateString();
           
            dtpEnd.date.Text = DateTime.Now.AddDays(7).Date.ToShortDateString();
            
        }

        private void UpdateGrid(bool isDelete)
        {
            if (book == null)
            {
                return;
            }

            if (exemplary.IdIndex <= 0)
            {
                return;
            }
            if (!isDelete)
            {
                book.idExemplary = exemplary.IdIndex;
                books.Add(book);
                exemplaries.Add(exemplary);
            }
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = books;
        }

        private bool ValidateFields()
        {
            if(string.IsNullOrEmpty(tfLectorRegister.Text))
            {
                dialogService.ShowError("Escolha um Leitor.");
                return false;
            }

            if(string.IsNullOrEmpty(tfNameLector.Text))
            {
                dialogService.ShowError("Escolha um Leitor.");
                return false;
            }

            if(books.Count < 1)
            {
                dialogService.ShowError("Escolha um Livro.");
                return false;
            }

            if(string.IsNullOrEmpty(dtpBegin.date.Text))
            {
                dialogService.ShowError("Escolha uma data de empréstimo.");
                return false;
            }

            if (string.IsNullOrEmpty(dtpEnd.date.Text))
            {
                dialogService.ShowError("Escolha uma data de devolução.");
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

        private void ClearFields()
        {
            dataGrid.ItemsSource = null;
            tfLectorRegister.Text = "";
            tfNameLector.Text = "";
            dtpBegin.date.Text = "";
            dtpEnd.date.Text = "";
        }

        private void DeleteFromList(int index)
        {
            if (!dialogService.ShowQuestion("Tem certeza que deseja\ndeletar este livro?", ""))
            {
                return;
            }
            if (index < 0)
            {
                return;
            }

            if (index > books.Count)
            {
                return;
            }

            if (books.Count < 1 || exemplaries.Count < 1)
            {
                return;
            }

            books.RemoveAt(index);
            exemplaries.RemoveAt(index);

            UpdateGrid(true);
        }

        private void GridCellDelete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int temp = dataGrid.SelectedIndex;
            DeleteFromList(temp);
        }

        private void btnSearchLector_Click(object sender, RoutedEventArgs e)
        {
            SearchLectorWindow searchLector = new SearchLectorWindow();
            _ = searchLector.ShowDialog();
            lector = searchLector.Lector;

            bool isConfirmed = searchLector.IsConfirmed;
            if (!isConfirmed)
            {
                return;
            }

            if (lector.Name == null)
            {
                return;
            }

            tfLectorRegister.Text = lector.IdLector.ToString();
            tfNameLector.Text = lector.Name.ToString();
        }

        private void btnSearchBook_Click(object sender, RoutedEventArgs e)
        {
            SearchBookWindow searchBook = new SearchBookWindow();  
            searchBook.Exemplaries = exemplaries;
            _ = searchBook.ShowDialog();

            bool isConfirmed = searchBook.IsConfirmed;

            if (isConfirmed)
            {
                book = searchBook.book;
                exemplary = searchBook.exemplary;
                UpdateGrid(false);
            }

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

            if (await daoLending.Insert(exemplaries, lector, begin, end))
            {
                dialogService.ShowSuccess("Empréstimo realizado com sucesso!");
            }

            ClearFields();
            OnOffControls(true);
        }

    }
}
