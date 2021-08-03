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
        private DAOAuthor DAOAuthor = new DAOAuthor();
        private DAOBook DAOBook = new DAOBook();
        public AddEditBookWindow()
        {
            InitializeComponent();
            Title = "Adicionar Livro";
            tbInfo.Text = "Adicionar Livro";

           

        }
        private void ShowMessage(string title, string contents, TypeDialog typeDialog)
        {
            InformationDialog dialog = new InformationDialog(title, contents, typeDialog);
            dialog.ShowDialog();
        }
        
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tfTitle.Text)
               || string.IsNullOrWhiteSpace(tfSubtitle.Text)
               || string.IsNullOrWhiteSpace(tfPublishingCompany.Text)
               || string.IsNullOrWhiteSpace(tfAuthor.Text))
            {
                ShowMessage("Atenção", "Preencha os espaços com * !!!", TypeDialog.Error);
                return;
            }

            book.Title = tfTitle.Text;
            book.Subtitle = tfSubtitle.Text;
            book.PublishingCompany = tfPublishingCompany.Text;
            author.Name = tfAuthor.Text;
            book.Gender = tfGender.Text;
            book.Edition = tfEdition.Text;
            if (string.IsNullOrWhiteSpace(tfNumberPages.Text))
            {
                book.Pages = 0;
            }
            else
            {
                book.Pages = Convert.ToInt32(tfNumberPages.Text);
            }

            if (string.IsNullOrWhiteSpace(tfYear.Text))
            {
                book.YearPublication = 0;
            }
            else
            {
                book.YearPublication = Convert.ToInt32(tfYear.Text);
            }
            book.Language = tfLanguage.Text;
            book.Volume = tfVolume.Text;
            book.Collection = tfColletion.Text;

            DAOBook.InsertBook(book, author);

            ShowMessage(" ", "Livro cadastrado com sucesso", TypeDialog.Success);
        }
    }
}
