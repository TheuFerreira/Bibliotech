using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using EnumsNET;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para ExemplaryWindow.xaml
    /// </summary>
    public partial class ExemplaryWindow : Window
    {
        private ObservableCollection<Exemplary> exemplaries;

        private readonly DAOBook daoBook;
        private readonly DAOExamplary daoExemplary;
        private TypeSearch typeSearch;
        private Status filterStatus;
        private readonly DialogService dialogService;
        private Book book;

        // SUBSTITUIR DEPOIS PELO SINGLETON
        private readonly Branch currentBranch = new Branch(1, "Senador");

        public ExemplaryWindow()
        {
            InitializeComponent();

            daoBook = new DAOBook();
            daoExemplary = new DAOExamplary();
            dialogService = new DialogService();

            searchField.ItemsSource = Enum.GetValues(typeof(TypeSearch))
                .Cast<TypeSearch>()
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();

            typeSearch = TypeSearch.Current;
            searchField.SelectedItem = typeSearch.AsString(EnumFormat.Description);

            filter.ItemsSource = Enum.GetValues(typeof(Status))
                .Cast<Status>()
                .Where(x => x != Status.Active)
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();

            filterStatus = Status.All;
            filter.SelectedItem = filterStatus.AsString(EnumFormat.Description);
        }

        private async void SearchEemplaries()
        {
            book = await daoBook.GetById(1);

            string text = searchField.Text;

            typeSearch = Enums.Parse<TypeSearch>(searchField.SelectedItem.ToString(), false, EnumFormat.Description);
            filterStatus = Enums.Parse<Status>(filter.SelectedItem.ToString(), false, EnumFormat.Description);

            columnSchool.Visibility = typeSearch == TypeSearch.Current ? Visibility.Hidden : Visibility.Visible;

            exemplaries = await daoExemplary.GetExemplarysByBook(book, typeSearch, currentBranch, text, filterStatus);
            dataGrid.ItemsSource = exemplaries;
        }

        private void SearchField_Click(object sender, RoutedEventArgs e)
        {
            SearchEemplaries();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchEemplaries();
        }

        private void CellPrint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new Exception("MÉTODO AINDA NÃO IMPLEMENTADO");
        }

        private Exemplary GetExemplaryInGrid()
        {
            int selectedExemplary = dataGrid.SelectedIndex;
            return exemplaries[selectedExemplary];
        }

        private async void BtnLost_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex < 0)
            {
                return;
            }

            Exemplary exemplary = GetExemplaryInGrid();
            if (exemplary.IsLost())
            {
                return;
            }

            int idIndex = exemplary.IdIndex;
            bool result = dialogService.ShowQuestion("EXTRAVIAR", $"Tem certeza de que deseja marcar o exemplar {idIndex:D2}, como EXTRAVIADO???");
            if (result == false)
            {
                return;
            }

            result = await daoExemplary.SetStatus(exemplary, Status.Lost);
            if (result == false)
            {
                return;
            }

            exemplary.Status = Status.Lost;
            dialogService.ShowInformation("Livro Extraviado!!!");
        }

        private async void BtnInactive_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex < 0)
            {
                return;
            }

            Exemplary exemplary = GetExemplaryInGrid();
            if (exemplary.IsInactive())
            {
                return;
            }

            int idIndex = exemplary.IdIndex;
            bool result = dialogService.ShowQuestion("EXCLUSÃO", $"Tem certeza de que deseja marcar o exemplar {idIndex:D2}, como INATIVO???");
            if (result == false)
            {
                return;
            }

            result = await daoExemplary.SetStatus(exemplary, Status.Inactive);
            if (result == false)
            {
                return;
            }

            exemplary.Status = Status.Inactive;
            dialogService.ShowInformation("Livro Inativo!!!");
        }

        private void BtnNew_OnClick(object sender, RoutedEventArgs e)
        {
            string value = dialogService.ShowAddDialog("Deseja adicionar quantos exemplares?", "QUANTIDADE DE EXEMPLARES:");
        }
    }
}
