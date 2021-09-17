using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Bibliotech.View.Devolutions
{
    /// <summary>
    /// Lógica interna para DevolutionWindow.xaml
    /// </summary>
    public partial class DevolutionWindow : Window
    {
        private Lector lector;
        private Exemplary exemplary;
        private List<Exemplary> exemplaries;
        private readonly DAOLector DAOLector;
        private readonly DialogService dialogService;
        public DevolutionWindow()
        {
            InitializeComponent();
            DAOLector = new DAOLector();
            dialogService = new DialogService();

            exemplary = new Exemplary();
            dateDevolution.date.Text = DateTime.Now.Date.ToShortDateString();

        }

        private void IsEnabledControls(bool result)
        {
            loading.Awaiting = result;
            btnSearchLector.IsEnabled = !result;
            btnMisplaced.IsEnabled = !result;
            btnExtend.IsEnabled = !result;
            btnDevolution.IsEnabled = !result;

            if (dataGrid.Items.Count == 0)
            {
                btnMisplaced.IsEnabled = false;
                btnExtend.IsEnabled = false;
                btnDevolution.IsEnabled = false;
            }
        }

        private async void SearchExemplaries()
        {
            IsEnabledControls(true);
            exemplaries = new List<Exemplary>();
            exemplaries = await DAOLector.GetBooks(lector.IdLector);
            dataGrid.ItemsSource = exemplaries;
            IsEnabledControls(false);
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(tfLectorRegister.Text))
            {
                dialogService.ShowError("Escolha um Leitor.");
                return false;
            }

            if (exemplaries.Count < 1)
            {
                dialogService.ShowError("Escolha um Exemplar.");
                return false;
            }

            if (string.IsNullOrEmpty(dateDevolution.date.Text))
            {
                dialogService.ShowError("Escolha uma data de devolução.");
                return false;
            }

            if (dateDevolution.date.SelectedDate < DateTime.Now.Date)
            {
                dialogService.ShowError("Escolha uma data de devolução maior que a atual.");
                return false;
            }

            return true;
        }

        private void BtnSearhLector_Click(object sender, RoutedEventArgs e)
        {
            SearchLectorWindow lectorWindow = new SearchLectorWindow();
            lectorWindow.ShowDialog();

            lector = new Lector();
            lector = lectorWindow.Selectedlectors;

            if (lector.IdLector == -1)
            {
                tfLectorRegister.Text = " ";
                tfNameLector.Text = " ";
            }
            else
            {
                tfLectorRegister.Text = lector.IdLector.ToString("D6");
                tfNameLector.Text = lector.Name;
            }

            SearchExemplaries();

        }

        private Exemplary Exemplary()
        {
            int index = dataGrid.SelectedIndex;
            Exemplary exemplary = exemplaries[index];
            return exemplary;
        }

        private async void BtnMisplaced_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                dialogService.ShowError("Selecione um exemplar!!!");
                return;
            }

            if (!dialogService.ShowQuestion("Extraviar", "Deseja extraviar esse livro?"))
            {
                return;
            }

            exemplary = Exemplary();

            IsEnabledControls(true);
            
            await new DAOExamplary().SetStatus(exemplary, Status.Lost);
            dialogService.ShowSuccess("Exemplar extraviado com sucesso!!!");

            IsEnabledControls(false);
            SearchExemplaries();
        }

        private async void BtnDevolution_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                dialogService.ShowError("Selecione um exemplar!!!");
                return;
            }

            if(dialogService.ShowQuestion("Devoulução", "Deseja devolver esse livro?"))
            {
                exemplary = Exemplary();
                IsEnabledControls(true);
                DateTime devolution = DateTime.Parse(dateDevolution.date.Text);
                await DAOLector.GetStatusDevolution(Status.Stock, exemplary.IdExemplary, exemplary.Lending.IdLending, devolution);

                dialogService.ShowSuccess("Exemplar devolvido com sucesso!!!");
                IsEnabledControls(false);
                SearchExemplaries();
            }
            
        }

        private async void BtnExtend_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                dialogService.ShowError("Selecione um exemplar!!!");
                return;
            }

            if (!dialogService.ShowQuestion("Estender prazo", "Deseja estender o prazo desse livro?"))
            {
                return;
            }

            exemplary = Exemplary();

            IsEnabledControls(true);
            
            DateTime date = Convert.ToDateTime(dateDevolution.date.SelectedDate);
            await DAOLector.GetExtendDevolution(date, exemplary.Lending.IdLending);
            dialogService.ShowSuccess("Data Atualizada");
            
            IsEnabledControls(false);
            SearchExemplaries();

        }

        private void btnMisplaced_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
