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
    public class AuthorService
    {
        public List<Author> Authors { get; private set; }

        private const int FIELD_AUTHORS = 2;
        private readonly DAOAuthor daoAuthor;

        public AuthorService()
        {
            Authors = new List<Author>();
            daoAuthor = new DAOAuthor();
        }

        public async Task LoadAuthors()
        {
            Console.WriteLine("Carregando Autores do Banco de Dados!!!");
            Authors = await daoAuthor.GetAll();
        }

        public async Task AddNewToDatabase()
        {
            await daoAuthor.InsertAll(Authors);
            Console.WriteLine("Novos Autores Inseridos!!");
            Console.Write("Pressione qualquer tecla para continuar");
            Console.ReadLine();
        }

        public void ReadSheetToGetAuthors(ISheet sheet)
        {
            Console.WriteLine("Lendo Arquivo de Livros...");
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                string value = GetCellStringValue(row);

                List<Author> authors = ValueToAuthors(value);
                authors.ForEach(AddAuthorInListIfNotContains);
            }
        }

        private string GetCellStringValue(IRow row)
        {
            ICell cell = row.GetCell(FIELD_AUTHORS);
            return cell == null ? string.Empty : cell.ToString();
        }

        public List<Author> ValueToAuthors(string value)
        {
            string[] values = SplitAuthors(value);
            return values.Select(x => GenerateNewAuthor(x)).ToList();
        }

        private string[] SplitAuthors(string str)
        {
            return str.Split(new string[] { ",", " e ", ";", "/" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private Author GenerateNewAuthor(string value)
        {
            value = TrimAllSpacies(value);
            return new Author(value);
        }

        private string TrimAllSpacies(string str)
        {
            return str.Trim().Replace("  ", " ");
        }

        private void AddAuthorInListIfNotContains(Author author)
        {
            bool contain = AuthorIsInList(author);
            if (contain)
            {
                return;
            }

            Authors.Add(author);
        }

        private bool AuthorIsInList(Author author)
        {
            foreach (Author aut in Authors)
            {
                bool result = CompareTwoAuthors(aut, author);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CompareTwoAuthors(Author author1, Author author2)
        {
            string name1 = author1.Name.ToLower().RemoveDiacritics();
            string name2 = author2.Name.ToLower().RemoveDiacritics();
            return name1.Equals(name2);
        }

        public Author GetAuthorInList(Author author)
        {
            foreach (Author aut in Authors)
            {
                bool result = CompareTwoAuthors(aut, author);
                if (result)
                {
                    return aut;
                }
            }

            return null;
        }

        public void Print()
        {
            Console.WriteLine("--------------------------------------------");
            Authors = Authors.OrderBy(x => x.Name).ToList();
            Authors.ForEach(x =>
            {
                Console.WriteLine(x.Name);
            });
            Console.WriteLine($"{Authors.Count} Autores");
            Console.WriteLine("--------------------------------------------");
        }
    }
}
