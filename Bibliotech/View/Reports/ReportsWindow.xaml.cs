﻿using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.View.Reports.CustomEnums;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.View.Reports
{
    /// <summary>
    /// Lógica interna para ReportsWindow.xaml
    /// </summary>
    public partial class ReportsWindow : Window
    {
        private Tabs tabs;

        private Period period;
        private int month;
        private int year;

        private TypeLending typeLending;

        private readonly DAOLending daoLending;
        private readonly DAOLector daoLector;

        private async void SetYearsToComboBoxYear()
        {
            List<int> years = await daoLending.GetYears();
            cbYear.ItemsSource = years;

            year = DateTime.Now.Year;
            cbYear.SelectedItem = year;
        }

        public ReportsWindow()
        {
            InitializeComponent();

            daoLending = new DAOLending();
            daoLector = new DAOLector();

            tabs = Tabs.Lendings;

            List<string> periodNames = Enum.GetValues(typeof(Period)).Cast<Period>().Select(x => x.AsString(EnumFormat.Description)).ToList();
            cbPeriod.ItemsSource = periodNames;

            period = Period.Day;
            cbPeriod.SelectedItem = period.AsString(EnumFormat.Description);

            dpDate.SelectedDate = DateTime.Now;

            List<string> monthName = Enumerable.Range(1, 12).Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x).ToUpper()).ToList();
            cbMonth.ItemsSource = monthName;

            month = DateTime.Now.Month;
            cbMonth.SelectedItem = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).ToUpper();

            SetYearsToComboBoxYear();

            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now;

            List<string> typeLendingNames = Enum.GetValues(typeof(TypeLending)).Cast<TypeLending>().Select(x => x.AsString(EnumFormat.Description)).ToList();
            lendingType.ItemsSource = typeLendingNames;

            typeLending = TypeLending.Both;
            lendingType.SelectedItem = typeLending.AsString(EnumFormat.Description);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tabs = (Tabs)tabControl.SelectedIndex;

            switch (tabs)
            {
                case Tabs.Lendings:
                    gridPeriod.Visibility = Visibility.Visible;
                    break;
                case Tabs.Lectors:
                    gridPeriod.Visibility = Visibility.Visible;
                    break;
                case Tabs.Books:
                    gridPeriod.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

        }

        private void CbPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = cbPeriod.SelectedItem.ToString();
            period = Enums.Parse<Period>(selectedItem, true, EnumFormat.Description);

            dpDate.Visibility = Visibility.Collapsed;
            cbMonth.Visibility = Visibility.Collapsed;
            cbYear.Visibility = Visibility.Collapsed;

            dpStartDate.Visibility = Visibility.Collapsed;
            dpEndDate.Visibility = Visibility.Collapsed;

            switch (period)
            {
                case Period.Day:
                    dpDate.Visibility = Visibility.Visible;
                    break;
                case Period.Mount:
                    cbYear.Visibility = Visibility.Visible;
                    cbMonth.Visibility = Visibility.Visible;
                    break;
                case Period.Year:
                    cbYear.Visibility = Visibility.Visible;
                    break;
                case Period.Custom:
                    dpStartDate.Visibility = Visibility.Visible;
                    dpEndDate.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void BtnLendingExport_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("Falta Implementar a Exportação nos Empréstimos");
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            loading.Awaiting = true;

            List<Lending> lendings = new List<Lending>();
            string selectedItem = string.Empty;

            switch (tabs)
            {
                case Tabs.Lendings:
                    selectedItem = lendingType.SelectedItem.ToString();
                    typeLending = Enums.Parse<TypeLending>(selectedItem, true, EnumFormat.Description);

                    switch (period)
                    {
                        case Period.Day:
                            DateTime? selectedDate = dpDate.SelectedDate;
                            lendings = await daoLending.SearchLendingsByDay(selectedDate.Value, typeLending);
                            lendingDataGrid.ItemsSource = lendings;
                            break;
                        case Period.Mount:
                            selectedItem = cbYear.SelectedItem.ToString();
                            year = int.Parse(selectedItem);

                            selectedItem = cbMonth.SelectedItem.ToString();
                            DateTime dateMonth = DateTime.ParseExact(selectedItem, "MMMM", CultureInfo.CurrentCulture);
                            month = dateMonth.Month;

                            lendings = await daoLending.SearchLendingsByMonth(year, month, typeLending);
                            lendingDataGrid.ItemsSource = lendings;
                            break;
                        case Period.Year:
                            selectedItem = cbYear.SelectedItem.ToString();
                            year = int.Parse(selectedItem);

                            lendings = await daoLending.SearchLendingsByYear(year, typeLending);
                            lendingDataGrid.ItemsSource = lendings;
                            break;
                        case Period.Custom:
                            DateTime? start = dpStartDate.SelectedDate;
                            DateTime? end = dpEndDate.SelectedDate;
                            lendings = await daoLending.SearchLendingsByCustomTime(start.Value, end.Value, typeLending);
                            lendingDataGrid.ItemsSource = lendings;
                            break;
                        default:
                            break;
                    }
                    break;
                case Tabs.Lectors:
                    switch (period)
                    {
                        case Period.Day:
                            DateTime? selectedDate = dpDate.SelectedDate;
                            lectorDataGrid.ItemsSource = await daoLector.ReportSearchByDay(selectedDate.Value);
                            break;
                        case Period.Mount:
                            selectedItem = cbYear.SelectedItem.ToString();
                            year = int.Parse(selectedItem);

                            selectedItem = cbMonth.SelectedItem.ToString();
                            DateTime dateMonth = DateTime.ParseExact(selectedItem, "MMMM", CultureInfo.CurrentCulture);
                            month = dateMonth.Month;

                            lectorDataGrid.ItemsSource = await daoLector.ReportSearchByMonth(year, month);
                            break;
                        case Period.Year:
                            selectedItem = cbYear.SelectedItem.ToString();
                            year = int.Parse(selectedItem);

                            lectorDataGrid.ItemsSource = await daoLector.ReportSearchByYear(year); ;
                            break;
                        case Period.Custom:
                            DateTime? start = dpStartDate.SelectedDate;
                            DateTime? end = dpEndDate.SelectedDate;

                            lectorDataGrid.ItemsSource = await daoLector.ReportSearchByCustomTime(start.Value, end.Value); ;
                            break;
                        default:
                            break;
                    }
                    break;
                case Tabs.Books:
                    break;
                default:
                    break;
            }

            loading.Awaiting = false;
        }

    }
}
