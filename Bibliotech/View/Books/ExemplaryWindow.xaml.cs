using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para ExemplaryWindow.xaml
    /// </summary>
    public partial class ExemplaryWindow : Window
    {
        private readonly DAOBook daoBook;
        private readonly DAOExamplary daoExemplary;
        private TypeSearch typeSearch;

        // SUBSTITUIR DEPOIS PELO SINGLETON
        private readonly Branch currentBranch = new Branch(1, "Senador");

        public ExemplaryWindow()
        {
            InitializeComponent();

            daoBook = new DAOBook();
            daoExemplary = new DAOExamplary();

            searchField.ItemsSource = Enum.GetValues(typeof(TypeSearch))
                .Cast<TypeSearch>()
                .Select(x => x.AsString(EnumFormat.Description))
                .ToList();

            typeSearch = TypeSearch.Current;
            searchField.SelectedItem = typeSearch.AsString(EnumFormat.Description);

            SearchEemplaries();
        }

        private async void SearchEemplaries()
        {
            Book book = await daoBook.GetById(1);

            string text = searchField.Text;

            typeSearch = Enums.Parse<TypeSearch>(searchField.SelectedItem.ToString(), false, EnumFormat.Description);
            columnSchool.Visibility = typeSearch == TypeSearch.Current ? Visibility.Hidden : Visibility.Visible;

            List<Exemplary> exemplaries = await daoExemplary.GetExemplarysByBook(book, typeSearch, currentBranch, text);
            dataGrid.ItemsSource = exemplaries;
        }

        private void SearchField_Click(object sender, RoutedEventArgs e)
        {
            SearchEemplaries();
        }

        private void CellPrint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new Exception("MÉTODO AINDA NÃO IMPLEMENTADO");
        }
    }
}
