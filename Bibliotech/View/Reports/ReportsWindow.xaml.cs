using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities;
using Bibliotech.Services;
using Bibliotech.Singletons;
using Bibliotech.View.Books;
using Bibliotech.View.Lectors;
using Bibliotech.View.Reports.CustomEnums;
using Bibliotech.View.Schools;
using EnumsNET;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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

        private readonly DialogService dialogService = new DialogService();

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

        //Exporta para excel
        private async void ExportToExcel(DataGrid dataGrid, string type)
        {
            btnExport.IsEnabled = false;
            string data = DateTime.Now.ToString();
            data = data.Replace("/", "_");
            data = data.Replace(":", "-");

            string nome = type + "Export" + data + ".xls";

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "XLS (*.xls) |*.xls";
            saveFile.FileName = nome;

            if (saveFile.ShowDialog() != true)
            {
                btnExport.IsEnabled = true;
                return;
            }

            dataGrid.SelectAllCells();
            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGrid);
            dataGrid.UnselectAllCells();
            string result = (string)Clipboard.GetData(DataFormats.Text);

            try
            {
                StreamWriter sw = new StreamWriter(saveFile.FileName);
                sw.WriteLine(result);
                
                sw.Close();
                if (dialogService.ShowQuestion("Deseja abrir o arquivo?", ""))
                {
                    _ = Process.Start(saveFile.FileName);
                }
            }
            catch (Exception)
            { throw new Exception("Deu Merda"); }
            finally
            {
                await Task.Delay(3000);
                btnExport.IsEnabled = true;
            }
        }

        //cria a matriz 
        private string[,] ToArray(DataGrid datagrid)
        {
            /* datagrid.SelectAllCells();
             datagrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
             ApplicationCommands.Copy.Execute(null, datagrid);
             datagrid.UnselectAllCells();
             String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);



             int x = datagrid.Items.Count;
             int y = datagrid.Columns.Count;
             string[,] matriz = new string[x+1, y];
             string[] temp;

             temp = result.Split(',', '\n');
             int index = 0;

             for (int i = 0; i <= x; i++)
             {
                 for (int j = 0; j < y; j++)
                 {
                     matriz[i, j] = temp[index];
                     index++;
                 }
             }
             string a = "";
             foreach (var item in matriz)
             {
                 a = a + item;
             }
             MessageBox.Show(a);

             return matriz;*/


            int x = datagrid.Items.Count;
            int y = datagrid.Columns.Count;

            string[,] matriz = new string[x + 1, y];

            int i = 0;
            int j = 0;

            var rows = datagrid.Items;


            foreach (DataGridColumn item in datagrid.Columns)
            {
                if(item.Header!=null)
                matriz[0, j] = item.Header.ToString();
                j++;
            }

            i = 1;
            j = 0;

            foreach (var row in rows)
            {
                var rowView = row;
                foreach (DataGridColumn column in datagrid.Columns)
                {
                    if (column.GetCellContent(row) is TextBlock)
                    {
                        TextBlock cellContent = column.GetCellContent(row) as TextBlock;
                        //MessageBox.Show(i + " "+ j);
                        matriz[i, j] = cellContent.Text;

                    }
                    j++;
                }
                i++;
                j = 0;
            }
            string rerer = "";
             foreach (var item in matriz)
             {
                if (item != null)
                {
                    rerer = rerer + item.ToString();
                }
                    
                 //MessageBox.Show(item.ToString());
             }
             MessageBox.Show(rerer);
            return matriz;
        }

        //exporta grid pdf
        private async void ExportToPdf(DataGrid datagrid, string type, bool haveImage)
        {
            
            if (datagrid.Items.Count < 1)
            {
                return;
            }
            

            string data = DateTime.Now.ToString();
            data = data.Replace("/", "_");
            data = data.Replace(":", "-");

            string nome = type + "Export" + data + ".pdf";

            btnExport.IsEnabled = false;
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "PDF (*.pdf) |*.pdf";
            saveFile.FileName = nome;


            int x = datagrid.Items.Count;
            int y = datagrid.Columns.Count;
            if (haveImage)
            {
                y--;
            }

            string[,] matriz = new string[x+1, y];
            matriz = ToArray(datagrid);


            if (saveFile.ShowDialog() != true)
            {
                btnExport.IsEnabled = true;
                return;
            }

            try
            {
                PdfPTable pTable = new PdfPTable(y);
                pTable.DefaultCell.Padding = 2;
                pTable.WidthPercentage = 100;
                pTable.HorizontalAlignment = Element.ALIGN_LEFT;



                for (int i = 0; i < y; i++)
                {
                    PdfPCell pCell = new PdfPCell(new Phrase(matriz[0, i]));
                    pCell.BackgroundColor = BaseColor.Cyan;
                    pCell.Border = 0;
                    
                    
                    pTable.AddCell(pCell);

                }

                for (int i = 1; i <= x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        PdfPCell pCell = new PdfPCell(new Phrase(matriz[i, j]));
                        pCell.Padding = 10;
                        /*pCell.BorderColor = BaseColor.Gray;
                        pCell.Border = 0;
                        pCell.EnableBorderSide(3);
                       
                        pCell.BackgroundColor = BaseColor.White;*/
                        pTable.AddCell(pCell);
                    }
                }


                Document document = new Document(PageSize.A4, 8f, 16f, 16f, 8f);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(saveFile.FileName, FileMode.Append));

                document.Open();
                document.Add(pTable);
                document.Close();

                if (dialogService.ShowQuestion("Deseja abrir o arquivo?", ""))
                {
                    _ = Process.Start(saveFile.FileName);
                }
            }
            catch (Exception)
            {
                throw new Exception("Deu Merda");
            }
            finally
            {
                await Task.Delay(3000);
                btnExport.IsEnabled = true;
            }
        }

        private void BtnLendingExport_Click(object sender, RoutedEventArgs e)
        {

            /*TypeReportWindow typeReport = new TypeReportWindow();
            typeReport.ShowDialog();*/
            if (dialogService.ShowQuestion("Excel ou pdf",""))
            {
                switch (tabs)
                {
                    case Tabs.Lendings:
                        if (lendingDataGrid.Items.Count < 1)
                        {
                            dialogService.ShowError("Escolha primeiro O tipo de\nrelatório que deseja!");
                            return;
                        }
                        ExportToExcel(lendingDataGrid, "Lending");
                        break;

                    case Tabs.Lectors:
                        if (lectorDataGrid.Items.Count < 1)
                        {
                            dialogService.ShowError("Escolha primeiro O tipo de\nrelatório que deseja!");
                            return;
                        }
                        ExportToExcel(lectorDataGrid, "Lector");
                        break;

                    case Tabs.Books:
                        if (bookDataGrid.Items.Count < 1)
                        {
                            dialogService.ShowError("Escolha primeiro O tipo de\nrelatório que deseja!");
                            return;
                        }
                        ExportToExcel(bookDataGrid, "Book");
                        break;
                }
            }
            else
            {
                switch (tabs)
                {
                    case Tabs.Lendings:
                        if (lendingDataGrid.Items.Count < 1)
                        {
                            dialogService.ShowError("Escolha primeiro O tipo de\nrelatório que deseja!");
                            return;
                        }
                        ExportToPdf(lendingDataGrid, "Lending", true);
                        break;

                    case Tabs.Lectors:
                        if (lectorDataGrid.Items.Count < 1)
                        {
                            dialogService.ShowError("Escolha primeiro O tipo de\nrelatório que deseja!");
                            return;
                        }
                        ExportToPdf(lectorDataGrid, "Lector", true);
                        break;

                    case Tabs.Books:
                        if (bookDataGrid.Items.Count < 1)
                        {
                            dialogService.ShowError("Escolha primeiro O tipo de\nrelatório que deseja!");
                            return;
                        }
                        ExportToPdf(bookDataGrid, "Book", false);
                        break;
                }
            }
            
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

            Book book = await new DAOBook().GetById(lending.Exemplary.Book.IdBook);

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
