using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Singletons;
using Bibliotech.View.Books;
using Bibliotech.View.Lectors;
using Bibliotech.View.Reports.CustomEnums;
using Bibliotech.View.Schools;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bibliotech.View.Reports
{
    /// <summary>
    /// Lógica interna para ReportsWindow.xaml
    /// </summary>
    public partial class ReportsWindow : Window
    {
        private List<Lending> lendings;

        public Branch SelectedBranch { get; set; }
        private Tabs tabs;
        private Period period;
        private TypeLending typeLending;
        private TypeBook typeBook;
        private Filter filter;

        private readonly DAOLending daoLending;
        private readonly DAOLector daoLector;
        private readonly DAOBook daoBook;

        public ReportsWindow()
        {
            InitializeComponent();

            daoLending = new DAOLending();
            daoLector = new DAOLector();
            daoBook = new DAOBook();

            tabs = Tabs.Lendings;

            dpDate.SelectedDate = DateTime.Now;

            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now;

            List<string> bookTypes = Enum.GetValues(typeof(TypeBook)).Cast<TypeBook>().Select(x => x.AsString(EnumFormat.Description)).ToList();
            cbBookType.ItemsSource = bookTypes;

            typeBook = TypeBook.Title;
            cbBookType.SelectedItem = typeBook.AsString(EnumFormat.Description);

            User currentUser = Session.Instance.User;
            SelectedBranch = new Branch
            {
                IdBranch = currentUser.Branch.IdBranch,
                Name = currentUser.Branch.Name
            };

            gridSchool.Visibility = currentUser.IsController() ? Visibility.Visible : Visibility.Collapsed;

            List<string> typePeriods = Enums.GetValues(typeof(Filter)).Cast<Filter>().Select(x => x.AsString(EnumFormat.Description)).ToList();
            cbFilter.ItemsSource = typePeriods;

            filter = Filter.Specific;
            cbFilter.SelectedItem = filter.AsString(EnumFormat.Description);
        }

        private void SetPeriodsToComboBoxPeriod()
        {
            List<string> periodNames = Enum.GetValues(typeof(Period)).Cast<Period>().Select(x => x.AsString(EnumFormat.Description)).ToList();
            cbPeriod.ItemsSource = periodNames;

            period = Period.Day;
            cbPeriod.SelectedItem = period.AsString(EnumFormat.Description);
        }

        private void SetMonthsToComboBoxMonth()
        {
            List<string> monthName = Enumerable.Range(1, 12).Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x).ToUpper()).ToList();
            cbMonth.ItemsSource = monthName;

            int month = DateTime.Now.Month;
            cbMonth.SelectedItem = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).ToUpper();
        }

        private async Task SetYearsToComboBoxYear()
        {
            btnSearch.IsEnabled = false;
            btnExport.IsEnabled = false;

            List<int> years = await daoLending.GetYears();
            cbYear.ItemsSource = years;

            int year = DateTime.Now.Year;
            cbYear.SelectedItem = year;

            btnSearch.IsEnabled = true;
            btnExport.IsEnabled = true;
        }

        private void SetTypeLendingsToComboBoxTypeLending()
        {
            List<string> typeLendingNames = Enum.GetValues(typeof(TypeLending)).Cast<TypeLending>().Select(x => x.AsString(EnumFormat.Description)).ToList();
            lendingType.ItemsSource = typeLendingNames;

            typeLending = TypeLending.Both;
            lendingType.SelectedItem = typeLending.AsString(EnumFormat.Description);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tabControl.SelectionChanged += TabControl_SelectionChanged;

            SetPeriodsToComboBoxPeriod();
            SetMonthsToComboBoxMonth();
            await SetYearsToComboBoxYear();
            SetTypeLendingsToComboBoxTypeLending();
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
            btnSearch.IsEnabled = false;
            btnExport.IsEnabled = false;
            btnSearchSchool.IsEnabled = false;
            loading.Awaiting = true;

            string selectedItem;
            int month;
            int year;

            switch (tabs)
            {
                case Tabs.Lendings:
                    selectedItem = lendingType.SelectedItem.ToString();
                    typeLending = Enums.Parse<TypeLending>(selectedItem, true, EnumFormat.Description);

                    switch (period)
                    {
                        case Period.Day:
                            DateTime? selectedDate = dpDate.SelectedDate;
                            lendings = await daoLending.SearchLendingsByDay(selectedDate.Value, typeLending, filter, SelectedBranch);
                            lendingDataGrid.ItemsSource = lendings;
                            break;
                        case Period.Mount:
                            selectedItem = cbYear.SelectedItem.ToString();
                            year = int.Parse(selectedItem);

                            selectedItem = cbMonth.SelectedItem.ToString();
                            DateTime dateMonth = DateTime.ParseExact(selectedItem, "MMMM", CultureInfo.CurrentCulture);
                            month = dateMonth.Month;

                            lendings = await daoLending.SearchLendingsByMonth(year, month, typeLending, filter, SelectedBranch);
                            lendingDataGrid.ItemsSource = lendings;
                            break;
                        case Period.Year:
                            selectedItem = cbYear.SelectedItem.ToString();
                            year = int.Parse(selectedItem);

                            lendings = await daoLending.SearchLendingsByYear(year, typeLending, filter, SelectedBranch);
                            lendingDataGrid.ItemsSource = lendings;
                            break;
                        case Period.Custom:
                            DateTime? start = dpStartDate.SelectedDate;
                            DateTime? end = dpEndDate.SelectedDate;
                            lendings = await daoLending.SearchLendingsByCustomTime(start.Value, end.Value, typeLending, filter, SelectedBranch);
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
                            lectorDataGrid.ItemsSource = await daoLector.ReportSearchByDay(selectedDate.Value, filter, SelectedBranch);
                            break;
                        case Period.Mount:
                            selectedItem = cbYear.SelectedItem.ToString();
                            year = int.Parse(selectedItem);

                            selectedItem = cbMonth.SelectedItem.ToString();
                            DateTime dateMonth = DateTime.ParseExact(selectedItem, "MMMM", CultureInfo.CurrentCulture);
                            month = dateMonth.Month;

                            lectorDataGrid.ItemsSource = await daoLector.ReportSearchByMonth(year, month, filter, SelectedBranch);
                            break;
                        case Period.Year:
                            selectedItem = cbYear.SelectedItem.ToString();
                            year = int.Parse(selectedItem);

                            lectorDataGrid.ItemsSource = await daoLector.ReportSearchByYear(year, filter, SelectedBranch); ;
                            break;
                        case Period.Custom:
                            DateTime? start = dpStartDate.SelectedDate;
                            DateTime? end = dpEndDate.SelectedDate;

                            lectorDataGrid.ItemsSource = await daoLector.ReportSearchByCustomTime(start.Value, end.Value, filter, SelectedBranch); ;
                            break;
                        default:
                            break;
                    }
                    break;
                case Tabs.Books:
                    selectedItem = cbBookType.SelectedItem.ToString();
                    typeBook = Enums.Parse<TypeBook>(selectedItem, true, EnumFormat.Description);

                    switch (typeBook)
                    {
                        case TypeBook.Title:
                            bookDataGrid.ItemsSource = await daoBook.ReportSearchByTitle(filter, SelectedBranch);
                            break;
                        case TypeBook.PublishingCompany:
                            bookDataGrid.ItemsSource = await daoBook.ReportSearchByPublishingCompany(filter, SelectedBranch);
                            break;
                        case TypeBook.Authors:
                            bookDataGrid.ItemsSource = await daoBook.ReportSearchByAuthors(filter, SelectedBranch);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            loading.Awaiting = false;
            btnSearch.IsEnabled = true;
            btnExport.IsEnabled = true;
            btnSearchSchool.IsEnabled = true;
        }

        private async void GridCellBook_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnExport.IsEnabled = false;
            btnSearch.IsEnabled = false;
            loading.Awaiting = true;

            int selectedIndex = lendingDataGrid.SelectedIndex;
            Lending lending = lendings[selectedIndex];

            Book book = await new DAOBook().GetById(lending.Exemplaries[0].Book.IdBook);

            loading.Awaiting = false;
            btnSearch.IsEnabled = true;
            btnExport.IsEnabled = true;

            _ = new AddEditBookWindow(book).ShowDialog();
        }

        private async void GridCellLector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnExport.IsEnabled = false;
            btnSearch.IsEnabled = false;
            loading.Awaiting = true;

            DataRowView selectedRow = lectorDataGrid.SelectedItem as DataRowView;
            int idLector = Convert.ToInt32(selectedRow["IdLector"]);

            Lector lector = await new DAOLector().GetById(idLector);

            loading.Awaiting = false;
            btnSearch.IsEnabled = true;
            btnExport.IsEnabled = true;

            _ = new AddEditLectorWindow(lector).ShowDialog();
        }

        private void BtnSearchSchool_Click(object sender, RoutedEventArgs e)
        {
            SearchSchoolWindow searchSchool = new SearchSchoolWindow();
            bool? result = searchSchool.ShowDialog();

            if (result.Value == false)
            {
                return;
            }

            Branch branch = searchSchool.Branch;
            SelectedBranch.IdBranch = branch.IdBranch;
            SelectedBranch.Name = branch.Name;
        }

        private void CbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = cbFilter.SelectedItem.ToString();
            filter = Enums.Parse<Filter>(selectedItem, true, EnumFormat.Description);

            switch (filter)
            {
                case Filter.Specific:
                    tfSchool.Visibility = Visibility.Visible;
                    btnSearchSchool.Visibility = Visibility.Visible;
                    break;
                case Filter.All:
                    tfSchool.Visibility = Visibility.Collapsed;
                    btnSearchSchool.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

        }
    }
}
