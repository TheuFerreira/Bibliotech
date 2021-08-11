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
using Bibliotech.Model.DAO;
using Bibliotech.UserControls;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para AddEditBookWindow.xaml
    /// </summary>
    public partial class AddEditBookWindow : Window
    {
        private Book book = new Book();
        private Author author = new Author();
        private readonly DAOAuthor DAOAuthor;
        private readonly DAOBook DAOBook;
        private  List<Book> returnBooksOnDataBase;
        public AddEditBookWindow(Book book)
        {
            InitializeComponent();
            DAOAuthor = new DAOAuthor();
            DAOBook = new DAOBook();

            Title = "Adicionar Livro";
            tbInfo.Text = "Adicionar Livro";

            if (book.IdBook == -1)
            {
                return;
            }
            
            EditBook();

        } 
        
        private async void EditBook()
        {
            Title = "Editar Livros";
            tbInfo.Text = "Editar Livros";

            tfBarCode.Text = book.IdBook.ToString();
            tfTitle.Text = book.Title;
            tfSubtitle.Text = book.Subtitle;
            tfAuthor.Text = book.Author.Name;
            tfGender.Text = book.Gender;
            tfEdition.Text = book.Edition;
            tfNumberPages.Text = book.Pages.ToString();
            tfYear.Text = book.YearPublication.ToString();
            tfLanguage.Text = book.Language;
            tfVolume.Text = book.Volume;
            tfColletion.Text = book.Collection;

        }
        private async Task<bool> CompareBook()
        {
            // consertar
            List<Book> returnDataBase = new List<Book>();

           // returnDataBase = await DAOBook.ListBooks();

            var emComum = returnDataBase.Intersect(returnBooksOnDataBase);
            if (emComum.Count() > 1) return true;
            else return false;
           
        }
        private void ShowMessage(string title, string contents, TypeDialog typeDialog)
        {
            InformationDialog dialog = new InformationDialog(title, contents, typeDialog);
            dialog.ShowDialog();
        }
        private bool VerifyFields()
        {
            if (string.IsNullOrWhiteSpace(tfTitle.Text)
               || string.IsNullOrWhiteSpace(tfSubtitle.Text)
               || string.IsNullOrWhiteSpace(tfPublishingCompany.Text)
               || string.IsNullOrWhiteSpace(tfAuthor.Text))
            {
                ShowMessage("Atenção", "Preencha os espaços com * !!!", TypeDialog.Error);
                return false;
            }

            book.Pages = (string.IsNullOrWhiteSpace(tfNumberPages.Text) ? 0 : Convert.ToInt32(tfNumberPages.Text));
            book.YearPublication = (string.IsNullOrWhiteSpace(tfYear.Text) ? 0 : Convert.ToInt32(tfYear.Text));
            return true;
        }
       

        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            returnBooksOnDataBase = new List<Book>();
            if (!VerifyFields()) return;
            book.Title = tfTitle.Text;
            book.Subtitle = tfSubtitle.Text;
            book.PublishingCompany = tfPublishingCompany.Text;
            author.Name = tfAuthor.Text;
            book.Gender = tfGender.Text;
            book.Edition = tfEdition.Text;
            book.Language = tfLanguage.Text;
            book.Volume = tfVolume.Text;
            book.Collection = tfColletion.Text;

            returnBooksOnDataBase.Add(book);
            /*if (await CompareBook())
            {
                ShowMessage(" ", "Livro já inserido", TypeDialog.Error);
            }*/

           await DAOAuthor.InsertAuthor(author);
           await DAOBook.InsertBook(book);
           await DAOBook.BookHasAuthor();

            ShowMessage(" ", "Livro cadastrado com sucesso", TypeDialog.Success);
        }
    }
}
