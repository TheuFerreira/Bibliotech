using NPOI.SS.UserModel;
using ReadExcel.Model.DAO;
using ReadExcel.Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadExcel.Services
{
    public class ExemplaryService
    {
        private const int FIELD_Title = 0;
        private const int FIELD_Subtitle = 1;
        private const int FIELD_Authors = 2;
        private const int FIELD_PublishingCompany = 3;
        private const int FIELD_Quantity = 11;

        public async Task AddNewToDatabase(Branch branch)
        {
            await new DAOExemplary().AddListOfExemplaries(branch, Menu.BookService.Books);

            ClearExemplaries();

            Console.WriteLine("Novos Exemplares Inseridos!!");
            Console.Write("Pressione qualquer tecla para continuar");
            Console.ReadLine();
        }

        public void ReadSheetToGetExemplaries(ISheet sheet)
        {
            Console.WriteLine("Lendo Arquivo de Livros...");
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                string title = GetCellStringValue(row, FIELD_Title);
                if (string.IsNullOrEmpty(title))
                {
                    continue;
                }

                string subtitle = GetCellStringValue(row, FIELD_Subtitle);
                string publishingCompany = GetCellStringValue(row, FIELD_PublishingCompany);

                string cellAuthors = GetCellStringValue(row, FIELD_Authors);
                if (string.IsNullOrEmpty(cellAuthors))
                {
                    continue;
                }

                int quantity = GetCellQuantityValue(row, FIELD_Quantity);

                AddExemplarieToListBook(title, subtitle, publishingCompany, quantity);
            }
        }

        private string GetCellStringValue(IRow row, int position)
        {
            ICell cell = row.GetCell(position);
            if (cell == null)
            {
                return string.Empty;
            }

            string value = cell.ToString();
            return TrimAllSpacies(value);
        }

        private string TrimAllSpacies(string str)
        {
            return str.Trim().Replace("  ", " ");
        }

        private int GetCellQuantityValue(IRow row, int position)
        {
            ICell cell = row.GetCell(position);
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
            {
                return 1;
            }

            return int.Parse(cell.ToString().Trim());
        }

        private void AddExemplarieToListBook(string title, string subtitle, string publishingCompany, int quantity)
        {
            for (int j = 0; j < Menu.BookService.Books.Count; j++)
            {
                Book book = Menu.BookService.Books[j];
                if (!CheckBookIsEquals(book, title, subtitle, publishingCompany))
                {
                    continue;
                }

                AddNewExemplaries(book, quantity);
            }
        }

        private bool CheckBookIsEquals(Book book, string title, string subtitle, string publishingCompany)
        {
            bool titleIsEqual = book.Title.Equals(title);
            bool subtitleIsEqual = book.Subtitle.Equals(subtitle);
            bool publishingCompanyIsEqual = book.PublishingCompany.Equals(publishingCompany);

            return titleIsEqual && subtitleIsEqual && publishingCompanyIsEqual;
        }

        private void AddNewExemplaries(Book book, int quantity)
        {
            for (int k = 0; k < quantity; k++)
            {
                book.Exemplaries.Add(new Exemplary());
            }
        }

        private void ClearExemplaries()
        {
            foreach (Book book in Menu.BookService.Books)
            {
                book.Authors = new List<Author>();
            }
        }

        public void Print()
        {
            foreach (Book book in Menu.BookService.Books)
            {
                Console.WriteLine($"{book.Title} | {book.Exemplaries.Count}");
            }
        }
    }
}
