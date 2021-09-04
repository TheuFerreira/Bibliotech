using Bibliotech.View.Reports.CustomEnums;
using System.Windows;

namespace Bibliotech.View.Reports
{
    /// <summary>
    /// Interaction logic for TypeReportWindow.xaml
    /// </summary>
    public partial class TypeReportWindow : Window
    {
        public ExportType ExportType { get; private set; }

        public TypeReportWindow()
        {
            InitializeComponent();

            ExportType = ExportType.None;
        }

        private void BtnExcel_OnClick(object sender, RoutedEventArgs e)
        {
            ExportType = ExportType.Excel;
            Close();
        }

        private void BtnPdf_OnClick(object sender, RoutedEventArgs e)
        {
            ExportType = ExportType.PDF;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
