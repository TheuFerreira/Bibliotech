using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
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

        private string[,] ToArray2(DataGrid dataGrid, bool haveImage)
        {
            int x = dataGrid.Items.Count;
            int y = dataGrid.Columns.Count;
            if (haveImage)
            {
                y--;
            }

            string[,] matriz = new string[x + 1, y];

            int j = 0;
            foreach (DataGridColumn item in dataGrid.Columns)
            {
                if (item.Header != null)
                {
                    matriz[0, j] = item.Header.ToString();
                }

                j++;
                if (j >= y)
                {
                    break;
                }
            }

            int i = 1;
            foreach (DataRowView row in dataGrid.Items)
            {
                for (j = 0; j < y; j++)
                {
                    matriz[i, j] = row.Row.ItemArray[j].ToString();
                }
                i++;
            }

            return matriz;
        }

        public async Task<bool> ExportToExcel(bool haveImage, DataGrid datagrid, string fileNme)
        {
            int x = datagrid.Items.Count;
            int y = datagrid.Columns.Count;
            if (haveImage)
            {
                y--;
            }

            string[,] matriz = new string[x + 1, y];

            matriz = ToArray2(datagrid, haveImage);

           /* SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Arquivo";
            saveFile.FileName = "";
            saveFile.Filter = "Excel Files (2007|*.xlsx|Excel Files(.CSV)|*.csv";
            if (saveFile.ShowDialog() != true)
            {
                return;
            }*/

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Application.Workbooks.Add(Type.Missing);

      

            if(!await addAsync(x, y, excelApp, matriz))
            {
                return false;
            }
 
            excelApp.Columns.AutoFit();
            excelApp.ActiveWorkbook.SaveCopyAs(fileNme);
            excelApp.ActiveWorkbook.Saved = true;
            excelApp.Quit();
            return true;
        }

        private async Task<bool> addAsync(int x, int y, Microsoft.Office.Interop.Excel.Application excelApp, string[,] matriz)
        {
            return await Task<bool>.Run(() => add(x+1, y, excelApp, matriz));
        }

        private bool add(int x, int y, Microsoft.Office.Interop.Excel.Application excelApp, string[,] matriz)
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    try
                    {
                        excelApp.Cells[i + 1, j + 1] = matriz[i, j];
                        
                    }

                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
