using Bibliotech.Model.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bibliotech.Export.PDF
{
    public class BarCode
    {
        protected Document doc;

        private const int ALIGNMENT = Element.ALIGN_LEFT;
        private const int BORDER = Rectangle.BOTTOM_BORDER;

        protected PdfPCell GetNewCell(string text, Font font, int alignment = 0, float padding = 5, int border = 0)
        {
            Phrase phrase = new Phrase(text, font);
            return new PdfPCell(phrase)
            {
                HorizontalAlignment = alignment,
                Padding = padding,
                Border = border,
                BorderColor = new BaseColor(0, 0, 0),
                BackgroundColor = new BaseColor(255, 255, 255)
            };
        }

        public void Build(List<Exemplary> exemplaries, Branch currentBranch, string path)
        {
            doc = new Document(PageSize.A4);
            Font font = FontFactory.GetFont(BaseFont.HELVETICA, 10);

            _ = doc.SetMargins(5, 5, 20, 10);
            _ = doc.AddCreationDate();

            FileStream fileStream = new FileStream(path, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fileStream);
            PdfContentByte cb = new PdfContentByte(writer);

            doc.Open();

            foreach (Exemplary e in exemplaries)
            {
                string idBranch = currentBranch.IdBranch.ToString().PadLeft(2, '0');
                string idBook = e.Book.IdBook.ToString().PadLeft(6, '0');
                string idIndex = e.IdIndex.ToString().PadLeft(5, '0');

                string concat = $"{idBranch}{idBook}{idIndex}";
                BarcodeEan barcodeEAN = new BarcodeEan
                {
                    Code = concat
                };
                Image image = barcodeEAN.CreateImageWithBarcode(cb, null, null);

                _ = doc.AddAuthor(currentBranch.Name.ToString());

                string name = string.Empty;
                foreach (Author author in e.Book.Authors)
                {
                    name += author.Name;
                    if (e.Book.Authors.IndexOf(author) < e.Book.Authors.Count - 1)
                    {
                        name += "; ";
                    }
                }

                PdfPTable pdfPTable = new PdfPTable(6);
                float[] cols = { 10, 30, 30, 30, 30, 40 };
                pdfPTable.SetWidths(cols);

                pdfPTable.DefaultCell.Border = Rectangle.BOTTOM_BORDER;
                pdfPTable.DefaultCell.BorderColor = BaseColor.Black;
                pdfPTable.DefaultCell.BorderColorBottom = BaseColor.White;
                pdfPTable.DefaultCell.Padding = 10;

                pdfPTable.CompleteRow();
                pdfPTable.AddCell(GetNewCell(e.IdIndex.ToString(), font, ALIGNMENT, 5, BORDER));
                pdfPTable.AddCell(GetNewCell(e.Book.Title, font, ALIGNMENT, 5, BORDER));
                pdfPTable.AddCell(GetNewCell(e.Book.Subtitle, font, ALIGNMENT, 5, BORDER));
                pdfPTable.AddCell(GetNewCell(name, font, ALIGNMENT, 5, BORDER)); ;
                pdfPTable.AddCell(GetNewCell(e.Book.PublishingCompany, font, ALIGNMENT, 5, BORDER));
                pdfPTable.AddCell(image);

                _ = doc.Add(pdfPTable);
            }

            doc.Close();
            fileStream.Close();
            writer.Close();
        }

        public async Task BuildAsync(List<Exemplary> exemplaries, Branch currentBranch, string path)
        {
            await Task.Run(() => Build(exemplaries, currentBranch, path));
        }
    }
}
