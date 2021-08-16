using Bibliotech.Model.DAO;
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

        public ReportsWindow()
        {
            InitializeComponent();

            daoLending = new DAOLending();

            tabs = Tabs.Lendings;

            List<string> periodNames = Enum.GetValues(typeof(Period)).Cast<Period>().Select(x => x.AsString(EnumFormat.Description)).ToList();
            lendingPeriod.ItemsSource = periodNames;

            period = Period.Day;
            lendingPeriod.SelectedItem = period.AsString(EnumFormat.Description);

            lendingDate.SelectedDate = DateTime.Now;

            List<string> monthName = Enumerable.Range(1, 12).Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x).ToUpper()).ToList();
            lendingMonth.ItemsSource = monthName;

            month = DateTime.Now.Month;
            lendingMonth.SelectedItem = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).ToUpper();

            List<int> years = new List<int>() { 2020, 2021 };
            lendingYear.ItemsSource = years;

            year = DateTime.Now.Year;
            lendingYear.SelectedItem = year;

            lendingStartDate.SelectedDate = DateTime.Now;
            lendingEndDate.SelectedDate = DateTime.Now;

            List<string> typeLendingNames = Enum.GetValues(typeof(TypeLending)).Cast<TypeLending>().Select(x => x.AsString(EnumFormat.Description)).ToList();
            lendingType.ItemsSource = typeLendingNames;

            typeLending = TypeLending.Both;
            lendingType.SelectedItem = typeLending.AsString(EnumFormat.Description);
        }

        private void LendingPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = lendingPeriod.SelectedItem.ToString();
            period = Enums.Parse<Period>(selectedItem, true, EnumFormat.Description);

            lendingDate.Visibility = Visibility.Collapsed;
            lendingMonth.Visibility = Visibility.Collapsed;
            lendingYear.Visibility = Visibility.Collapsed;

            lendingStartDate.Visibility = Visibility.Collapsed;
            lendingEndDate.Visibility = Visibility.Collapsed;

            switch (period)
            {
                case Period.Day:
                    lendingDate.Visibility = Visibility.Visible;
                    break;
                case Period.Mount:
                    lendingMonth.Visibility = Visibility.Visible;
                    break;
                case Period.Year:
                    lendingYear.Visibility = Visibility.Visible;
                    break;
                case Period.Custom:
                    lendingStartDate.Visibility = Visibility.Visible;
                    lendingEndDate.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void LendingMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = lendingMonth.SelectedItem.ToString();
            DateTime dateMonth = DateTime.ParseExact(selectedItem, "MMMM", CultureInfo.CurrentCulture);
            month = dateMonth.Month;
        }

        private void LendingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = lendingYear.SelectedItem.ToString();
            year = int.Parse(selectedItem);
        }

        private void LendingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = lendingType.SelectedItem.ToString();
            typeLending = Enums.Parse<TypeLending>(selectedItem, true, EnumFormat.Description);
        }

        private void BtnLendingExport_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("Falta Implementar a Exportação nos Empréstimos");
        }

        private async void BtnLendingSearch_Click(object sender, RoutedEventArgs e)
        {
            List<Lending> lendings = new List<Lending>();

            switch (period)
            {
                case Period.Day:
                    DateTime? selectedDate = lendingDate.SelectedDate;
                    lendings = await daoLending.SearchLendingsByDay(selectedDate.Value, typeLending);
                    lendingDataGrid.ItemsSource = lendings;
                    break;
                case Period.Mount:
                    lendings = await daoLending.SearchLendingsByMonth(month, typeLending);
                    lendingDataGrid.ItemsSource = lendings;
                    break;
                case Period.Year:
                    lendings = await daoLending.SearchLendingsByYear(year, typeLending);
                    lendingDataGrid.ItemsSource = lendings;
                    break;
                case Period.Custom:
                    DateTime? start = lendingStartDate.SelectedDate;
                    DateTime? end = lendingEndDate.SelectedDate;
                    lendings = await daoLending.SearchLendingsByCustomTime(start.Value, end.Value, typeLending);
                    lendingDataGrid.ItemsSource = lendings;
                    break;
                default:
                    break;
            }
        }
    }
}
