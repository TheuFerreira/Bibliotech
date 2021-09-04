using Bibliotech.Model.Entities;
using Bibliotech.Services;
using System;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Windows;
using iText.Barcodes;

namespace Bibliotech.BarCode
{
    public class GenerateAndPrintBarCorde
    {
        private DialogService dialogService;
        protected Document doc;

        protected PdfPCell GetNewCell(string texto, Font fonte, int Alinhamento, float espacamento, int borda, BaseColor corBorda, BaseColor corFundo)
        {
            var cell = new PdfPCell(new Phrase(texto, fonte));
            cell.HorizontalAlignment = Alinhamento;
            cell.Padding = espacamento;
            cell.Border = borda;
            cell.BorderColor = corBorda;
            cell.BackgroundColor = corFundo;

            return cell;
        }

        protected PdfPCell getNewCell(string Texto, Font Fonte, int Alinhamento = 0, float Espacamento = 5, int Borda = 0)
        {
            return GetNewCell(Texto, Fonte, Alinhamento, Espacamento, Borda, new BaseColor(0, 0, 0), new BaseColor(255, 255, 255));
        }
        public void BaseDocument(List<Exemplary> exemplary, Branch currentBranch)
        {
            dialogService = new DialogService();
            dialogService.ShowInformation("Gerando");
            doc = new Document(PageSize.A4);
            Font font = FontFactory.GetFont(BaseFont.HELVETICA, 10);

            _ = doc.SetMargins(5, 5, 20, 10);
            _ = doc.AddCreationDate();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.AddExtension = true;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Salvar";
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.DefaultExt = "pdf";
            saveFileDialog.FileName = " - Código de Barras";
            saveFileDialog.ShowDialog();

            MessageBox.Show(saveFileDialog.FileName);

            string path = saveFileDialog.FileName;
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            PdfContentByte cb = new PdfContentByte(writer);
            string bookTitle = string.Empty;
            doc.Open();

            foreach (Exemplary e in exemplary)
            {
                string idBranch = currentBranch.IdBranch.ToString().PadLeft(4, '0');
                string idBook = e.Book.IdBook.ToString().PadLeft(4, '0');
                string idIndex = e.IdIndex.ToString().PadLeft(5, '0');
                bookTitle = e.Book.Title;

                string concat = $"" + idBranch + "" + idBook + "" + idIndex;
                iTextSharp.text.pdf.BarcodeEan barcodeEAN = new BarcodeEan();
                barcodeEAN.Code = concat;
                Image image = barcodeEAN.CreateImageWithBarcode(cb, null, null);

                doc.AddAuthor(currentBranch.Name.ToString());

                string name = string.Empty;

                foreach (Author author in e.Book.Authors)
                {
                    name = name + author.Name.ToString() + "; ";
                }

                PdfPTable pdfPTable = new PdfPTable(6);
                float[] cols = { 10, 30, 30, 30, 30, 40 };
                pdfPTable.SetWidths(cols);

                pdfPTable.DefaultCell.Border = PdfPCell.BOTTOM_BORDER;
                pdfPTable.DefaultCell.BorderColor = BaseColor.Black;
                pdfPTable.DefaultCell.BorderColorBottom = BaseColor.White;
                pdfPTable.DefaultCell.Padding = 10;

                pdfPTable.CompleteRow();
                pdfPTable.AddCell(getNewCell(e.IdIndex.ToString(), font, Element.ALIGN_LEFT, 5, PdfPCell.BOTTOM_BORDER));
                pdfPTable.AddCell(getNewCell(e.Book.Title, font, Element.ALIGN_LEFT, 5, PdfPCell.BOTTOM_BORDER));
                pdfPTable.AddCell(getNewCell(e.Book.Subtitle, font, Element.ALIGN_LEFT, 5, PdfPCell.BOTTOM_BORDER));
                pdfPTable.AddCell(getNewCell(name, font, Element.ALIGN_LEFT, 5, PdfPCell.BOTTOM_BORDER)); ;
                pdfPTable.AddCell(getNewCell(e.Book.PublishingCompany, font, Element.ALIGN_LEFT, 5, PdfPCell.BOTTOM_BORDER));
                pdfPTable.AddCell(image);

                doc.Add(pdfPTable);
            }

            doc.Close();

            dialogService.ShowInformation("finished");
            /*
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.AddExtension = true;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Salvar";
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.DefaultExt = "pdf";
            saveFileDialog.FileName = bookTitle + " - Código de Barras";
            
            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(doc);
                streamWriter.Close();
            }
            */
        }

    }
}
