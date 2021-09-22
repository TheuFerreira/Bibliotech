using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using Bibliotech.Singletons;
using Bibliotech.UserControls.CustomDialog;
using Bibliotech.View.Authors;
using System;
using System.Windows;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para AddEditBookWindow.xaml
    /// </summary>
    public partial class AddEditBookWindow : Window
    {
        private readonly Book book;
        private readonly DAOBook daoBook;
        private readonly Branch currentBranch = Session.Instance.User.Branch;

        public AddEditBookWindow(Book book)
        {
            InitializeComponent();

            daoBook = new DAOBook();

            this.book = book;

            Title = "Adicionar Livro";
            tbInfo.Text = "Adicionar Livro";
            btnExemplaries.IsEnabled = false;

            if (book.IdBook == -1)
            {
                return;
            }

            EditBook();
        }

        private void EditBook()
        {
            Title = "Editar Livros";
            tbInfo.Text = "Editar Livros";
            tfTitle.Text = book.Title;
            tfSubtitle.Text = book.Subtitle;
            tfPublishingCompany.Text = book.PublishingCompany;
            tfAuthor.Text = book.GetAuthorsToString();
            tfGender.Text = book.Gender;
            tfEdition.Text = book.Edition;
            tfNumberPages.Text = book.Pages.ToString();
            tfYear.Text = book.YearPublication.ToString();
            tfLanguage.Text = book.Language;
            tfVolume.Text = book.Volume;
            tfColletion.Text = book.Collection;
            btnExemplaries.IsEnabled = true;
        }

        private void ShowMessage(string title, string contents, TypeDialog typeDialog)
        {
            _ = new InformationDialog(title, contents, typeDialog).ShowDialog();
        }

        private bool ShowQuestion(string title, string contents)
        {
            DialogService dialogService = new DialogService();
            return dialogService.ShowQuestion(title, contents);
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

            if (book.Authors.Count == 0)
            {
                ShowMessage("Atenção", "Adicione Autores ao livro!!!", TypeDialog.Error);
                return false;
            }

            book.Pages = string.IsNullOrWhiteSpace(tfNumberPages.Text) ? 0 : Convert.ToInt32(tfNumberPages.Text);
            book.YearPublication = string.IsNullOrWhiteSpace(tfYear.Text) ? 0 : Convert.ToInt32(tfYear.Text);
            return true;
        }

        private void BtnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            SearchAuthorWindow searchAuthor = new SearchAuthorWindow(book);
            _ = searchAuthor.ShowDialog();

            tfAuthor.Text = book.GetAuthorsToString();
            
        }

        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = false;

            if (!VerifyFields()) return;

            book.Title = tfTitle.Text;
            book.Subtitle = tfSubtitle.Text;
            book.PublishingCompany = tfPublishingCompany.Text;
            book.Gender = tfGender.Text;
            book.Edition = tfEdition.Text;
            book.Language = tfLanguage.Text;
            book.Volume = tfVolume.Text;
            book.Collection = tfColletion.Text;

            if (book.IdBook == -1)
            {
                await daoBook.Insert(book);
                if (ShowQuestion(" ", "Livro inserido com sucesso! Deseja adicionar um exemplar?"))
                {
                    _ = new ExemplaryWindow(book).ShowDialog();
                }
                else return;
            }
            else
            {
                await daoBook.Update(book);

                ShowMessage(" ", "Livro alterado com sucesso", TypeDialog.Success);

                Close();
            }

            btnSave.IsEnabled = true;
        }

        private void ShowExemplaries()
        {
            _ = new ExemplaryWindow(book).ShowDialog();
        }

        private void BtnExemplaryBook_Click(object sender, RoutedEventArgs e)
        {
            ShowExemplaries();
        }
    }
}
