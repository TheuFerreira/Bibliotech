using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bibliotech.Export.Excel
{
    public class ReportsExcel
    {
        public bool GenerateByGrid(string filePath, DataGrid dataGrid)
        {
            dataGrid.SelectAllCells();
            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGrid);
            dataGrid.UnselectAllCells();
            string result = (string)Clipboard.GetData(DataFormats.Text);

            try
            {
                StreamWriter sw = new StreamWriter(filePath);
                sw.WriteLine(result);

                sw.Close();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
