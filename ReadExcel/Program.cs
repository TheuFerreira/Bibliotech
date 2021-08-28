using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ReadExcel.Model.Entities;
using ReadExcel.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace ReadExcel
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Menu.Render();
            return;
            
            AuthorService authorService = new AuthorService();

            ISheet sheet1 = GetSheet(@"C:\Users\Usuario\Documents\[ENOE] Cadastro de Livros Literários.xlsx");
            ISheet sheet2 = GetSheet(@"C:\Users\Usuario\Documents\[HELENA] Cadastro de Livros Literários.xlsx");

            for (int i = 1; i <= sheet1.LastRowNum; i++)
            {
                IRow row = sheet1.GetRow(i);

                int fieldTitle = 0;
                int fieldSubtitle = 1;
                int fieldAuthors = 2;
                int fieldPublishingCompany = 3;
                int fieldPages = 4;
                int fieldYearPublication = 5;
                int fieldGender = 6;
                int fieldEdition = 7;
                int fieldLanguage = 8;
                int fieldCollection = 9;
                int fieldVolume = 10;

                string title = GetCellStringValue(row, fieldTitle);
                if (string.IsNullOrEmpty(title))
                    continue;

                string subtitle = GetCellStringValue(row, fieldSubtitle);
                string publishingCompany = GetCellStringValue(row, fieldPublishingCompany);

                string cellAuthors = GetCellStringValue(row, fieldAuthors);
                if (string.IsNullOrEmpty(cellAuthors))
                {
                    Console.WriteLine($"Livro sem Autor: Título: {title} | Subtítulo: {subtitle} | Editora: {publishingCompany}");
                    continue;
                }

                int? pages = GetCellIntValue(row, fieldPages);
                int? yearPublication = GetCellIntValue(row, fieldYearPublication);
                string gender = GetCellStringValue(row, fieldGender);
                string edition = GetCellStringValue(row, fieldEdition);
                string language = GetCellStringValue(row, fieldLanguage);
                string collection = GetCellStringValue(row, fieldCollection);
                string volume = GetCellStringValue(row, fieldVolume);

                List<Author> authors = authorService.ValueToAuthors(cellAuthors);
                for (int j = 0; j < authors.Count; j++)
                {
                    authors[j] = authorService.GetAuthorInList(authors[j]);
                }

                Book book = new Book
                {
                    Title = title,
                    Subtitle = subtitle,
                    Authors = authors,
                    PublishingCompany = publishingCompany,
                    Pages = pages,
                    YearPublication = yearPublication,
                    Gender = gender,
                    Edition = edition,
                    Language = language,
                    Collection = collection,
                    Volume = volume,
                };
            }

            Console.ReadKey();
        }

        private static ISheet GetSheet(string filePath)
        {
            Stream excelFile = new FileStream(filePath, FileMode.Open);
            XSSFWorkbook sheets = new XSSFWorkbook(excelFile);
            return sheets.GetSheetAt(0);
        }

        private static string GetCellStringValue(IRow row, int position)
        {
            ICell cell = row.GetCell(position);
            if (cell == null)
                return string.Empty;

            return cell.ToString();
        }

        private static int? GetCellIntValue(IRow row, int position)
        {
            ICell cell = row.GetCell(position);
            if (cell == null)
                return null;

            if (string.IsNullOrWhiteSpace(cell.ToString()))
                return null;

            return int.Parse(cell.ToString().Trim());
        }
    }
}
