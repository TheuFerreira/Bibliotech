﻿using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.Services;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.View.Schools
{
    /// <summary>
    /// Lógica interna para SchoolsWindow.xaml
    /// </summary>
    public partial class SchoolsWindow : Window
    {
        private readonly DialogService dialogService = new DialogService();
        private readonly DAOBranch daoSchool = new DAOBranch();
        private readonly Address address = new Address();
        private readonly Branch school = new Branch();

        public SchoolsWindow()
        {
            InitializeComponent();
        }

        private async void UpdateGrid()
        {
            DataTable dataTable = await daoSchool.FillDataGrid(searchField.Text);
            schoolGrid.ItemsSource = dataTable.DefaultView;
        }

        private async void ButtonOnOff_OnClick(object sender, RoutedEventArgs e)
        {
            if (school.IdBranch < 1)
            {
                return;
            }

            if (school.Status == Status.Active)
            {
                if (dialogService.ShowQuestion("Tem certeza que deseja\ndesativar esta escola?", ""))
                {
                    await daoSchool.OnOff(0, school.IdBranch);
                    dialogService.ShowSuccess("Desativado com sucesso!");
                }

                UpdateGrid();
                return;
            }

            if (dialogService.ShowQuestion("Tem certeza que deseja\nativar esta escola?", ""))
            {
                await daoSchool.OnOff(1, school.IdBranch);
                dialogService.ShowSuccess("Ativado com sucesso!");
            }

            UpdateGrid();
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            AddEditSchoolWindow addEdit = new AddEditSchoolWindow();
            addEdit.Id = school.IdBranch;
            addEdit.Id_address = school.Address.IdAddress;
            addEdit.tfName.Text = school.Name;
            addEdit.tfCity.Text = address.City;
            addEdit.tfDistrict.Text = address.Neighborhood;
            addEdit.tfPhone.Text = school.Telephone.ToString();
            addEdit.tfStreet.Text = address.Street;
            addEdit.tfNumber.Text = address.Number;
            addEdit.IsUpdate = true;
            addEdit.tbInfo.Text = "Editar Escola";
            
            if (school.IdBranch >= 1)
            {
                addEdit.ShowDialog();
            }

            UpdateGrid();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            AddEditSchoolWindow addEdit = new AddEditSchoolWindow();
            addEdit.tbInfo.Text = "Adicionar Escola";
            addEdit.IsUpdate = false;
            _ = addEdit.ShowDialog();

            UpdateGrid();
        }

        public void SchoolGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void schoolGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                address.IdAddress = Convert.ToInt32(row_selected["id_address"].ToString());
                address.City = row_selected["city"].ToString();
                address.Neighborhood = row_selected["neighborhood"].ToString();
                address.Street = row_selected["street"].ToString();
                address.Number = row_selected["number"].ToString();

                school.IdBranch = Convert.ToInt32(row_selected["id_branch"].ToString());
                school.Address = address;
                school.Name = row_selected["name"].ToString();

                long? telephone = null;
                if (long.TryParse(row_selected["telephone"].ToString(), out long temp))
                {
                    telephone = temp;
                }
                school.Telephone = telephone;

                if (row_selected["description"].ToString() == "Ativo")
                {
                    school.Status = Status.Active;
                }
                else
                {
                    school.Status = Status.Inactive;
                }
            }
        }

        private void searchField_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void searchField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchField.Text))
            {
                UpdateGrid();
            }
        }
    }
}
