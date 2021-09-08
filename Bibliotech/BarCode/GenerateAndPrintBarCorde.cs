using Bibliotech.Model.Entities;
using Bibliotech.Services;
using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Windows;
using Bibliotech.Services;

namespace Bibliotech.BarCode
{
    public class GenerateAndPrintBarCorde
    {
        protected Document doc;
        private DialogService dialogService = new DialogService();
        public bool IsFileOpen(string filePath)
        {
            bool fileOpened = false;
            try
            {
                System.IO.FileStream fs = System.IO.File.OpenWrite(filePath);
                fs.Close();
            }
            catch (System.IO.IOException ex)
            {
                fileOpened = true;
            }

            return fileOpened;
        }
        protected PdfPCell GetNewCell(string text, Font font, int alignment, float padding, int borda, BaseColor borderColor, BaseColor backgroundColor)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = alignment;
            cell.Padding = padding;
            cell.Border = borda;
            cell.BorderColor = borderColor;
            cell.BackgroundColor = backgroundColor;

            return cell;
        }

        protected PdfPCell getNewCell(string Text, Font Font, int Alignment = 0, float Spacing = 5, int Border = 0)
        {
            return GetNewCell(Text, Font, Alignment, Spacing, Border, new BaseColor(0, 0, 0), new BaseColor(255, 255, 255));
        }
        public void BaseDocument(List<Exemplary> exemplary, Branch currentBranch)
        {
            try
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

                if (IsFileOpen(path))
                {
                    dialogService.ShowError("O arquivo já está aberto em outro programa.");
                    return;
                }

                FileStream fileStream = new FileStream(path, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, fileStream);
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
                fileStream.Close();
                writer.Close();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
