using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.View.Authors
{
    /// <summary>
    /// Lógica interna para SearchAuthorWindow.xaml
    /// </summary>
    public partial class SearchAuthorWindow : Window
    {
        private readonly Book book;
        private readonly DAOAuthor daoAuthor;
        private readonly DialogService dialogService;
        private List<Author> authors;

        public SearchAuthorWindow(Book book)
        {
            InitializeComponent();

            daoAuthor = new DAOAuthor();
            dialogService = new DialogService();
            authors = new List<Author>();

            this.book = book;
        }

        private void IsEnabledButtons(bool result)
        {
            loading.Awaiting = result;
            addButton.IsEnabled = !result;
            editButton.IsEnabled = !result;
            selectButton.IsEnabled = !result;
            searchField.IsEnabled = !result;
        }

        private async void AddNewAuthor(string Title, string Descriprion)
        {
            string text = dialogService.ShowAddTextDialog(Descriprion, Title);
            if (text == string.Empty)
            {
                return;
            }

            IsEnabledButtons(true);

            Author author = new Author
            {
                Name = text,
                Status = Status.Active,
            };
            await daoAuthor.Insert(author);

            IsEnabledButtons(false);

            dialogService.ShowSuccess("Autor inserido com Sucesso!");

            await SearchAuthors();
        }

        private async void EditAuthor(Author author)
        {
            string text = dialogService.ShowAddTextDialog("Editar Autor", "Editar Autor");
            if (text == string.Empty)
            {
                return;
            }

            IsEnabledButtons(true);

            author.Name = text;
            await daoAuthor.Update(author);

            IsEnabledButtons(false);

            dialogService.ShowSuccess("Autor alterado com Sucesso!");

            await SearchAuthors();
        }

        private async Task SearchAuthors()
        {
            IsEnabledButtons(true);

            string text = searchField.Text;
            authors = await daoAuthor.GetAll(text);

            authors.ForEach(x => x.Status = book.Authors.Contains(x) ? Status.Active : Status.Inactive);
            authors = authors.OrderByDescending(x => x.Status).ToList();
            dataGrid.ItemsSource = authors;

            IsEnabledButtons(false);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddNewAuthor("Adicionar Autor", "Digite o nome do Autor");
        }

        private async void SearchField_Click(object sender, RoutedEventArgs e)
        {
            await SearchAuthors();
        }

        private Author GetAuthor()
        {
            int index = dataGrid.SelectedIndex;
            return index < 0 ? null : authors[index];
        }

        private void SelectedButton_Click(object sender, RoutedEventArgs e)
        {
            Author author = GetAuthor();
            if (author == null)
            {
                dialogService.ShowError("Selecione um Autor!!!");
                return;
            }

            if (author.Status == Status.Active)
            {
                _ = book.Authors.Remove(author);
                book.Authors = book.Authors.OrderBy(x => x.IdAuthor).ToList();

                authors.ForEach(x => x.Status = book.Authors.Contains(x) ? Status.Active : Status.Inactive);
                authors = authors.OrderByDescending(x => x.Status).ToList();
                dataGrid.ItemsSource = authors;

                dialogService.ShowSuccess("Autor Removido");
            }
            else
            {
                book.Authors.Add(author);
                book.Authors = book.Authors.OrderBy(x => x.IdAuthor).ToList();

                authors.ForEach(x => x.Status = book.Authors.Contains(x) ? Status.Active : Status.Inactive);
                authors = authors.OrderByDescending(x => x.Status).ToList();
                dataGrid.ItemsSource = authors;

                dialogService.ShowSuccess("Autor Adicionado");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Author author = GetAuthor();
            if (author == null)
            {
                dialogService.ShowError("Selecione um Autor!!!");
                return;
            }

            EditAuthor(author);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await SearchAuthors();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Author author = GetAuthor();
            if (author == null)
            {
                return;
            }
            selectButton.Text = author.Status == Status.Active ? "DESMARCAR" : "SELECIONAR";
        }
    }
}
