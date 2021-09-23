using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
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



            /*Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Application.Workbooks.Add(Type.Missing);*/

      

            if(!await addAsync(x, y, fileNme /*excelApp*/, matriz))
            {
                return false;
            }
 
           /* excelApp.Columns.AutoFit();
            excelApp.ActiveWorkbook.SaveCopyAs(fileNme);
            excelApp.ActiveWorkbook.Saved = true;
            excelApp.Quit();*/
            return true;


        }

        private async Task<bool> addAsync(int x, int y, string fileName /*Microsoft.Office.Interop.Excel.Application excelApp*/, string[,] matriz)
        {
            return await Task<bool>.Run(() => add(x+1, y, fileName /*excelApp*/, matriz));
        }

        private bool add(int x, int y, string fileName /*Microsoft.Office.Interop.Excel.Application excelApp*/, string[,] matriz)
        {
            /* for (int i = 0; i < x; i++)
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
             return true;*/

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Planilha1");

            try
            {
                for (int i = 0; i < x; i++)
                {
                    int rowIndex = i;// + 1;
                    IRow row = sheet.CreateRow(rowIndex);

                    for (int j = 0; j < y; j++)
                    {
                        ICell cell = row.CreateCell(j);

                        cell.SetCellValue(matriz[i, j]);
                        sheet.AutoSizeColumn(j);

                    }
                }
                var stream = new MemoryStream();
                workbook.Write(stream);

                FileStream file = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
                stream.WriteTo(file);
                file.Close();
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
