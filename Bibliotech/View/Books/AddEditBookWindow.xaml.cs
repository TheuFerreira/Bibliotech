﻿using Bibliotech.Model.Entities;
using Bibliotech.UserControls.CustomDialog;
using System;
using System.Windows;
using Bibliotech.Model.DAO;
using Bibliotech.UserControls;
using Bibliotech.Services;
using System.Threading.Tasks;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para AddEditBookWindow.xaml
    /// </summary>
    public partial class AddEditBookWindow : Window
    {
        private readonly Book Book = new Book();
        private readonly Author Author = new Author();
        private readonly DAOAuthor DAOAuthor;
        private readonly DAOBook DAOBook;
        private readonly Loading loading = new Loading();
        public AddEditBookWindow(Book book)
        {
            InitializeComponent();
            DAOAuthor = new DAOAuthor();
            DAOBook = new DAOBook();
            Book = book;
            Title = "Adicionar Livro";
            tbInfo.Text = "Adicionar Livro";

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

            tfBarCode.Text = ($" SME-VGP- {Book.IdBook}");
            tfTitle.Text = Book.Title;
            tfSubtitle.Text = Book.Subtitle;
            tfPublishingCompany.Text = Book.PublishingCompany;
           // tfAuthor.Text = Book.Author.Name;
            tfGender.Text = Book.Gender;
            tfEdition.Text = Book.Edition;
            tfNumberPages.Text = Book.Pages.ToString();
            tfYear.Text = Book.YearPublication.ToString();
            tfLanguage.Text = Book.Language;
            tfVolume.Text = Book.Volume;
            tfColletion.Text = Book.Collection;
        }
        private void ShowMessage(string title, string contents, TypeDialog typeDialog)
        {
            InformationDialog dialog = new InformationDialog(title, contents, typeDialog);
            dialog.ShowDialog();
        }
        private bool ShowQuestion(string title, string contents)
        {
            DialogService dialogService = new DialogService();
            bool result = dialogService.ShowQuestion(title, contents);
            return result;
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
            
            Book.Pages = (string.IsNullOrWhiteSpace(tfNumberPages.Text) ? 0 : Convert.ToInt32(tfNumberPages.Text));
            Book.YearPublication = (string.IsNullOrWhiteSpace(tfYear.Text) ? 0 : Convert.ToInt32(tfYear.Text));
            return true;
        }
        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            loading.Awaiting = true;
            btnSave.IsEnabled = false;

            if (!VerifyFields()) return;
            Book.Title = tfTitle.Text;
            Book.Subtitle = tfSubtitle.Text;
            Book.PublishingCompany = tfPublishingCompany.Text;
            Author.Name = tfAuthor.Text;
            Book.Gender = tfGender.Text;
            Book.Edition = tfEdition.Text;
            Book.Language = tfLanguage.Text;
            Book.Volume = tfVolume.Text;
            Book.Collection = tfColletion.Text;
            
            if(Book.IdBook == -1)
            {
                await DAOAuthor.InsertAuthor(Author);
                await DAOBook.InsertBook(Book);
                await DAOBook.BookHasAuthor();

                if (ShowQuestion(" ", "Livro inserido com sucesso! Deseja adicionar um exemplar?"))
                {
                    ExemplaryWindow exemplaryWindow = new ExemplaryWindow(Book);
                    exemplaryWindow.ShowDialog();
                }
                else return;
            }
            else
            {
                await DAOBook.UpdateBook(Book);
                await DAOAuthor.UpdateAuthor(Author);
                ShowMessage(" ", "Livro alterado com sucesso", TypeDialog.Success);
                this.Close();
            }
            loading.Awaiting = false;
            btnSave.IsEnabled = true;

        }
        private void ShowExemplaries()
        {
            ExemplaryWindow exemplary = new ExemplaryWindow(Book);
            exemplary.Show();
        }
        private void BtnExemplaryBook_Click(object sender, RoutedEventArgs e)
        {
            ShowExemplaries();
        }
    }
}
