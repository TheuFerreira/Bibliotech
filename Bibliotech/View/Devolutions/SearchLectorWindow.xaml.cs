using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using Bibliotech.Singletons;
using System.Collections.Generic;
using System.Windows;

namespace Bibliotech.View.Devolutions
{
    /// <summary>
    /// Lógica interna para SearchLectorWindow.xaml
    /// </summary>
    public partial class SearchLectorWindow : Window
    {
        private readonly DAOLector DAOLector;
        private readonly DialogService dialogService;
        private readonly int IdBranch = Session.Instance.User.Branch.IdBranch;
        private List<Lector> lectors;
        public Lector Selectedlectors;

        public SearchLectorWindow()
        {
            InitializeComponent();
            DAOLector = new DAOLector();
            lectors = new List<Lector>();
            Selectedlectors = new Lector();
            dialogService = new DialogService();
        }

        private void SetIsEnabled(bool result)
        {
            loading.Awaiting = result;
            searchField.IsEnabled = !result;
            btnSelectLector.IsEnabled = !result;
        }

        private async void SearchLector()
        {
            SetIsEnabled(true);
            string text = searchField.Text;
            lectors = await DAOLector.GetLectors(IdBranch, text);
            dataGrid.ItemsSource = lectors;
            SetIsEnabled(false);
        }

        private Lector GetLector()
        {
            int index = dataGrid.SelectedIndex;
            Lector lector = lectors[index];
            return lector;
        }

        private void SelectLectorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SearchLector();
        }

        private void BtnSelectLector_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                dialogService.ShowError("Selecione um Leitor!!!");
                return;
            }

            Selectedlectors = GetLector();
            Close();
        }

        private void SearchField_Click(object sender, RoutedEventArgs e)
        {
            SearchLector();
        }
    }
}
