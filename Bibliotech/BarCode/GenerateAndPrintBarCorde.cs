using Bibliotech.Model.Entities;
using Bibliotech.Services;
using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Windows;

namespace Bibliotech.BarCode
{
    public class GenerateAndPrintBarCorde
    {
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
            doc = new Document(PageSize.A4);
            Font font = FontFactory.GetFont(BaseFont.HELVETICA, 10);

            _ = doc.SetMargins(5, 5, 20, 10);
            _ = doc.AddCreationDate();

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AddExtension = true,
                RestoreDirectory = true,
                Title = "Salvar",
                Filter = "PDF Files|*.pdf",
                DefaultExt = "pdf",
                FileName = "Código de Barras"
            };
            _ = saveFileDialog.ShowDialog();

            string path = saveFileDialog.FileName;
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            PdfContentByte cb = new PdfContentByte(writer);

            doc.Open();

            foreach (Exemplary e in exemplary)
            {
                string idBranch = currentBranch.IdBranch.ToString().PadLeft(2, '0');
                string idBook = e.Book.IdBook.ToString().PadLeft(6, '0');
                string idIndex = e.IdIndex.ToString().PadLeft(5, '0');

                string concat = $"" + idBranch + "" + idBook + "" + idIndex;
                BarcodeEan barcodeEAN = new BarcodeEan();
                barcodeEAN.Code = concat;
                Image image = barcodeEAN.CreateImageWithBarcode(cb, null, null);

                _ = doc.AddAuthor(currentBranch.Name.ToString());

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

                _ = doc.Add(pdfPTable);
            }

            doc.Close();
        }

    }
}
