using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using System.Collections.Generic;
using System.Windows;

namespace Bibliotech.View.Books
{
    /// <summary>
    /// Lógica interna para ExemplaryWindow.xaml
    /// </summary>
    public partial class ExemplaryWindow : Window
    {
        private readonly DAOBook daoBook;
        private readonly DAOExamplary daoExemplary;

        public ExemplaryWindow()
        {
            InitializeComponent();

            daoBook = new DAOBook();
            daoExemplary = new DAOExamplary();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Book book = await daoBook.GetById(1);
            List<Exemplary> exemplaries = await daoExemplary.GetExemplarysByBook(book);

            dataGrid.ItemsSource = exemplaries;
        }
    }
}
