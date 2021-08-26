using Bibliotech.Model.Entities;
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
using Bibliotech.Model.DAO;
using Bibliotech.Services;

namespace Bibliotech.View.Devolutions
{
    /// <summary>
    /// Lógica interna para DevolutionWindow.xaml
    /// </summary>
    public partial class DevolutionWindow : Window
    {
        private Lector lector;
        private List<Book> books;
        private readonly DAOLector DAOLector;
        private readonly DialogService dialogService;
        public DevolutionWindow()
        {
            InitializeComponent();
            DAOLector = new DAOLector();
            books = new List<Book>();
            dialogService = new DialogService();
        } 
        private void IsEnabledControls(bool result)
        {
            loading.Awaiting = result;
            btnSearchLector.IsEnabled = !result;
            btnMisplaced.IsEnabled = !result;
            btnExtend.IsEnabled = !result;
            btnDevolution.IsEnabled = !result;
        }
        private async void SearchExemplaries()
        {
            IsEnabledControls(true);
            books = await DAOLector.GetBooks(lector.IdLector);
            dataGrid.ItemsSource = books;
            IsEnabledControls(false);
        } 
        private void ShowMessage(string title, string description)
        {
            dialogService.ShowSuccess(description);
        }
        private void ClearFields()
        {
            tfLectorRegister.Text = string.Empty;
            tfNameLector.Text = string.Empty;
        }
        private void BtnSearhLector_Click(object sender, RoutedEventArgs e)
        {
            SearchLectorWindow lectorWindow = new SearchLectorWindow();
            lectorWindow.ShowDialog();

            lector = new Lector();
            lector = lectorWindow.Selectedlectors;
            tfLectorRegister.Text = lector.IdLector.ToString();
            tfNameLector.Text = lector.Name;
            
            SearchExemplaries();

        }
        private Book book()
        {
            int index = dataGrid.SelectedIndex;
            Book book = books[index];
            return book;
        }
        private void BtnMisplaced_OnClick(object sender, RoutedEventArgs e)
        {
            IsEnabledControls(true);
            DAOLector.GetStatusDevolution(4, book().IdExemplary);
            IsEnabledControls(false);
            ShowMessage(" ", "Exemplar extraviado com sucesso!!!");
        }

        private void BtnDevolution_OnClick(object sender, RoutedEventArgs e)
        {
            IsEnabledControls(true);
            DAOLector.GetStatusDevolution(3, book().IdExemplary);
            IsEnabledControls(false);
            ShowMessage(" ", "Exemplar devolvido com sucesso!!!");
           
        }
    }
}
