using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using Bibliotech.Singletons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para SearchBookWindow.xaml
    /// </summary>
    public partial class SearchBookWindow : Window
    {
        private readonly DAOBook daoBook = new DAOBook();

        public Book book = new Book();

        private readonly DialogService dialogService = new DialogService();

        public Exemplary exemplary = new Exemplary();
        //trocar pro krai do session
        private int idBranch = Session.Instance.User.Branch.IdBranch;

        private List<Exemplary> exemplaries = new List<Exemplary>();

        public List<Exemplary> Exemplaries { get => exemplaries; set => exemplaries = value; }

        public bool IsConfirmed { get => isConfirmed; set => isConfirmed = value; }

        private bool isConfirmed = false;

        public SearchBookWindow()
        {
            InitializeComponent();
            UpdateGrid();
        }

        private void OnOffControls(bool awaiting)
        {
            selectButton.IsEnabled = awaiting;
            searchField.IsEnabled = awaiting;
        }

        private bool VerifyDuplicate()
        {
            if (!exemplaries.Contains(exemplary))
            {
                return true;
            }

            dialogService.ShowError("Este livro já está selecionado!\nPor favor selecione outro.");
            return false;

        }

        private async void UpdateGrid()
        {
            loading.Awaiting = true;
            OnOffControls(false);
            DataTable dataTable = await daoBook.FillSearchDataGrid(searchField.Text, idBranch);
            if (dataTable != null)
            {
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            loading.Awaiting = false;
            OnOffControls(true);
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;

            if (row_selected != null)
            {
                book.IdBook = Convert.ToInt32(row_selected["id_book"].ToString());
                book.Title = row_selected["title"].ToString();
                book.Subtitle = row_selected["subtitle"].ToString();
                book.Author.Name = row_selected["autores"].ToString();
                book.PublishingCompany = row_selected["publishing_company"].ToString();

                exemplary.IdIndex = Convert.ToInt32(row_selected["id_index"].ToString());
                exemplary.IdExemplary = Convert.ToInt32(row_selected["id_exemplary"].ToString());
            }

        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                dialogService.ShowError("Escolha um livro primeiro!");
                return;
            }

            if (VerifyDuplicate())
            {
                isConfirmed = true;
                Close();
            }

        }

        private void searchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }
    }
}
