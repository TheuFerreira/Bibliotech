using Bibliotech.View.Books;
using Bibliotech.View.Lectors;
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

namespace Bibliotech.View.Lendings
{
    /// <summary>
    /// Lógica interna para LendingWindow.xaml
    /// </summary>
    public partial class LendingWindow : Window
    {
        public LendingWindow()
        {
            InitializeComponent();
        }

        private void btnSearchLector_Click(object sender, RoutedEventArgs e)
        {
            SearchLectorWindow searchLector = new SearchLectorWindow();
            searchLector.ShowDialog();
        }

        private void btnSearchBook_Click(object sender, RoutedEventArgs e)
        {
            SearchBookWindow searchBook = new SearchBookWindow();
            searchBook.ShowDialog();
        }
    }
}
