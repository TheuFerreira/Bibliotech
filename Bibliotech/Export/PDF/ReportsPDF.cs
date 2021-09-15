using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.IO;
using System.Windows.Controls;

namespace Bibliotech.Export.PDF
{
    public class ReportsPDF
    {
        public bool GenerateByGrid(string filePath, DataGrid datagrid, bool haveImage)
        {
            int x = datagrid.Items.Count;
            int y = datagrid.Columns.Count;
            if (haveImage)
            {
                y--;
            }

            string[,] matriz = ToArray2(datagrid, haveImage);

            try
            {
                PdfPTable pTable = new PdfPTable(y);
                pTable.DefaultCell.Padding = 2;
                pTable.WidthPercentage = 100;
                pTable.HorizontalAlignment = Element.ALIGN_LEFT;

                for (int i = 0; i < y; i++)
                {
                    PdfPCell pCell = new PdfPCell(new Phrase(matriz[0, i]))
                    {
                        BackgroundColor = BaseColor.Gray,
                        Border = 0,
                        PaddingLeft = 5,
                        PaddingRight = 5,
                        PaddingBottom = 10,
                        PaddingTop = 10
                    };
                    pTable.AddCell(pCell);
                }

                for (int i = 1; i <= x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        PdfPCell pCell = new PdfPCell(new Phrase(matriz[i, j]))
                        {
                            Border = 0,
                            Padding = 5
                        };

                        if (i % 2 == 0 && i > 0)
                            pCell.BackgroundColor = BaseColor.LightGray;
                        else
                            pCell.BackgroundColor = BaseColor.White;

                        pTable.AddCell(pCell);
                    }
                }

                Document document = new Document(PageSize.A4, 8f, 16f, 16f, 8f);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Append));

                document.Open();
                document.Add(pTable);
                document.Close();

                return true;
            }
            catch (Exception)
            {
                throw;
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
    }
}
