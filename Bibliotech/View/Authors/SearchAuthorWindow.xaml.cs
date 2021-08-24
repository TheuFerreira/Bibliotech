﻿using System.Windows;
using Bibliotech.Model.Entities;
using Bibliotech.Model.DAO;
using Bibliotech.UserControls.CustomDialog;
using Bibliotech.Services;
using Bibliotech.Model.Entities.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Bibliotech.View.Authors
{
    /// <summary>
    /// Lógica interna para SearchAuthorWindow.xaml
    /// </summary>
    public partial class SearchAuthorWindow : Window
    {
        private Author author;
        private DAOAuthor DAOAuthor;
        private Book book;
        private readonly DialogService dialogService;
        private List<Author> authors;
        public List<int> SetIdAuthor;

        public SearchAuthorWindow()
        {
            InitializeComponent();
            author = new Author();
            DAOAuthor = new DAOAuthor();
            book = new Book();
            dialogService = new DialogService();
            authors = new List<Author>();
            SetIdAuthor = new List<int>();
        }
        
        private async void AddNewAuthor(string Title, string Descriprion)
        {
            string text = dialogService.ShowAddAuthorDialog(Descriprion, Title);
            if (text == string.Empty) return;
            author.Name = text;
            author.Status = Status.Active;
            await DAOAuthor.InsertAuthor(author);
            dialogService.ShowSuccess("Autor inserido com Sucesso!");
        }

        private void IsEnabled(bool result)
        {
            loading.Awaiting = result;
            addButton.IsEnabled = !result;
            selectButton.IsEnabled = !result;
            searchField.IsEnabled = !result;
        }
        private async Task SearchAuthor()
        {
            string text = searchField.Text;
            IsEnabled(true);
            authors = await DAOAuthor.GetAuthor(text);
            dataGrid.ItemsSource = authors;
            IsEnabled(false);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddNewAuthor("Adicionar Autor", "Digite o nome do Autor");
        }
        private async void AuthorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await SearchAuthor();
        }

        private async void SearchField_Click(object sender, RoutedEventArgs e)
        {
            await SearchAuthor();
        }

        private Author GetIdAuthor()
        {
            int index = dataGrid.SelectedIndex;
            Author author = authors[index];
            return author;
        }
        private void SelectedButton_Click(object sender, RoutedEventArgs e)
        {
            SetIdAuthor.Add(GetIdAuthor().IdAuthor);
        }
    }
}
