using NPOI.SS.UserModel;
using ReadExcel.Extensions;
using ReadExcel.Model.DAO;
using ReadExcel.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadExcel.Services
{
    public class BookService
    {
        public List<Book> Books { get; private set; }

        private readonly DAOBook daoAuthor;

        private const int FIELD_Title = 0;
        private const int FIELD_Subtitle = 1;
        private const int FIELD_Authors = 2;
        private const int FIELD_PublishingCompany = 3;
        private const int FIELD_Pages = 4;
        private const int FIELD_YearPublication = 5;
        private const int FIELD_Gender = 6;
        private const int FIELD_Edition = 7;
        private const int FIELD_Language = 8;
        private const int FIELD_Collection = 9;
        private const int FIELD_Volume = 10;

        public BookService()
        {
            Books = new List<Book>();
            daoAuthor = new DAOBook();
        }

        public async Task LoadBooks()
        {
            Console.WriteLine("Carregando Livros do Banco de Dados!!!");
            Books = await daoAuthor.GetAll();
        }

        public async Task AddNewToDatabase()
        {
            await daoAuthor.InsertAll(Books);
            Console.WriteLine("Novos Livros Inseridos!!");
            Console.Write("Pressione qualquer tecla para continuar");
            Console.ReadLine();
        }

        public void ReadSheetToGetBooks(ISheet sheet)
        {
            Console.WriteLine("Lendo Arquivo de Livros...");
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                string title = GetCellStringValue(row, FIELD_Title);
                if (string.IsNullOrEmpty(title))
                    continue;

                string subtitle = GetCellStringValue(row, FIELD_Subtitle);
                string publishingCompany = GetCellStringValue(row, FIELD_PublishingCompany);

                string cellAuthors = GetCellStringValue(row, FIELD_Authors);
                if (string.IsNullOrEmpty(cellAuthors))
                {
                    Console.WriteLine($"Livro sem Autor: Título: {title} | Subtítulo: {subtitle} | Editora: {publishingCompany}");
                    continue;
                }

                int? pages = GetCellIntValue(row, FIELD_Pages);
                int? yearPublication = GetCellIntValue(row, FIELD_YearPublication);
                string gender = GetCellStringValue(row, FIELD_Gender);
                string edition = GetCellStringValue(row, FIELD_Edition);
                string language = GetCellStringValue(row, FIELD_Language);
                string collection = GetCellStringValue(row, FIELD_Collection);
                string volume = GetCellStringValue(row, FIELD_Volume);

                List<Author> authors = GetAuthors(cellAuthors);

                Book book = new Book
                {
                    IdBook = -1,
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

                AddBookInListIfNotContains(book);
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

        private int? GetCellIntValue(IRow row, int position)
        {
            ICell cell = row.GetCell(position);
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
            {
                return null;
            }

            return int.Parse(cell.ToString().Trim());
        }

        public List<Author> GetAuthors(string value)
        {
            List<Author> authors = Menu.AuthorService.ValueToAuthors(value);
            for (int j = 0; j < authors.Count; j++)
            {
                authors[j] = Menu.AuthorService.GetAuthorInList(authors[j]);
            }

            return authors;
        }

        private void AddBookInListIfNotContains(Book book)
        {
            bool contain = BookIsInList(book);
            if (contain)
                return;

            Books.Add(book);
        }

        private bool BookIsInList(Book book)
        {
            foreach (Book bk in Books)
            {
                bool result = CompareTwoBooks(bk, book);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CompareTwoBooks(Book book1, Book book2)
        {
            string title1 = book1.Title.ToLower().RemoveDiacritics();
            string title2 = book2.Title.ToLower().RemoveDiacritics();

            string subtitle1 = book1.Subtitle.ToLower().RemoveDiacritics();
            string subtitle2 = book2.Subtitle.ToLower().RemoveDiacritics();

            string publishingCompany1 = book1.PublishingCompany.ToLower().RemoveDiacritics();
            string publishingCompany2 = book2.PublishingCompany.ToLower().RemoveDiacritics();

            if (title1.Equals(title2) && subtitle1.Equals(subtitle2) && publishingCompany1.Equals(publishingCompany2))
            {
                return true;
            }

            return false;
        }

        public void Print()
        {
            Console.WriteLine("--------------------------------------------");
            Books = Books.OrderBy(x => x.Title).ToList();
            Books.ForEach(x =>
            {
                Console.WriteLine($"Título: {x.Title} | Subtítulo: {x.Subtitle} | Editora: {x.PublishingCompany}");
            });
            Console.WriteLine($"{Books.Count} Livros");
            Console.WriteLine("--------------------------------------------");
            Console.ReadKey();
        }
    }
}
