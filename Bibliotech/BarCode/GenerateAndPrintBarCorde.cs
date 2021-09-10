using Bibliotech.Model.Entities;
using Bibliotech.Services;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;

namespace Bibliotech.BarCode
{
    public class GenerateAndPrintBarCorde
    {
        protected Document doc;
        private DialogService dialogService = new DialogService();
        
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
        public void BaseDocument(List<Exemplary> exemplary, Branch currentBranch, string path)
        {
            
            doc = new Document(PageSize.A4);
            Font font = FontFactory.GetFont(BaseFont.HELVETICA, 10);

            _ = doc.SetMargins(5, 5, 20, 10);
            _ = doc.AddCreationDate();

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
                   if(e.Book.Authors.Count > 1)
                   {
                        name = name + author.Name + "; ";
                   }
                    else
                    {
                        name = author.Name;
                    }
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

    }
}
