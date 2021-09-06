using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using Bibliotech.Singletons;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Bibliotech.BarCode;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para ExemplaryWindow.xaml
    /// </summary>
    public partial class ExemplaryWindow : Window
    {
        private List<Exemplary> exemplaries;

        private readonly DAOExamplary daoExemplary;
        private TypeSearch typeSearch;
        private Status filterStatus;
        private readonly DialogService dialogService;
        private readonly Book Book;
        private readonly Branch currentBranch;
        private  GenerateAndPrintBarCorde generateAndPrintBarCorde;

        public ExemplaryWindow(Book book)
        {
            InitializeComponent();

            daoExemplary = new DAOExamplary();
            dialogService = new DialogService();
            generateAndPrintBarCorde = new GenerateAndPrintBarCorde();
            Book = book;

            filter.ItemsSource = Enum.GetValues(typeof(Status))
                .Cast<Status>()
                .Where(x => x != Status.Active)
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();

            filterStatus = Status.All;
            filter.SelectedItem = filterStatus.AsString(EnumFormat.Description);

            User currentUser = Session.Instance.User;
            currentBranch = currentUser.Branch;
            searchField.ItemsSource = Enum.GetValues(typeof(TypeSearch))
                .Cast<TypeSearch>()
                .Where(x => !currentUser.IsUser() || x != TypeSearch.All)
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();

            typeSearch = TypeSearch.Current;
            searchField.SelectedItem = typeSearch.AsString(EnumFormat.Description);

            filter.SelectionChanged += Filter_SelectionChanged;
        }

        private void SetButtons(bool value)
        {
            searchField.IsEnabled = value;
            filter.IsEnabled = value;
            btnLost.IsEnabled = value;
            btnNew.IsEnabled = value;
            btnPrint.IsEnabled = value;
            btnInactive.IsEnabled = value;

            loading.Awaiting = !value;
        }
        
        private async void SearchEemplaries()
        {
            string text = searchField.Text;

            typeSearch = Enums.Parse<TypeSearch>(searchField.SelectedItem.ToString(), false, EnumFormat.Description);
            filterStatus = Enums.Parse<Status>(filter.SelectedItem.ToString(), false, EnumFormat.Description);

            columnSchool.Visibility = typeSearch == TypeSearch.Current ? Visibility.Hidden : Visibility.Visible;

            SetButtons(false);

            exemplaries = await daoExemplary.GetExemplarysByBook(Book, typeSearch, currentBranch, text, filterStatus);
            dataGrid.ItemsSource = exemplaries;

            SetButtons(true);
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
            if (dataGrid.SelectedItem == null)
            {
                dialogService.ShowError("Você deve selecionar um exemplar!!!");
                return;
            }
            SetButtons(false);
            List<Exemplary> exemplarySelected = new List<Exemplary>();
            exemplarySelected.Add(GetExemplaryInGrid());
            generateAndPrintBarCorde.BaseDocument(exemplarySelected, currentBranch);
            SetButtons(true);
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

            SetButtons(false);
            result = await daoExemplary.SetStatus(exemplary, Status.Lost);
            SetButtons(true);

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

            SetButtons(false);
            result = await daoExemplary.SetStatus(exemplary, Status.Inactive);
            SetButtons(true);

            if (result == false)
            {
                return;
            }

            exemplary.Status = Status.Inactive;
            dialogService.ShowInformation("Livro Inativo!!!");
        }

        private async void BtnNew_OnClick(object sender, RoutedEventArgs e)
        {
            string text = dialogService.ShowAddDialog("Deseja adicionar quantos exemplares?", "QUANTIDADE DE EXEMPLARES:");
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            int numberExemplaries = int.Parse(text);
            if (numberExemplaries <= 0)
            {
                dialogService.ShowError("A quantidade precisa ser maior que 0!");
            }

            SetButtons(false);
            bool result = await daoExemplary.AddExemplaries(currentBranch, Book, numberExemplaries);
            SetButtons(true);

            if (result == false)
            {
                return;
            }

            dialogService.ShowSuccess($"{numberExemplaries} Exemplares adicionados!");
            SearchEemplaries();
        }

        private void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            SetButtons(false);
            generateAndPrintBarCorde.BaseDocument(exemplaries, currentBranch);
            SetButtons(true);
        }
    }
}
