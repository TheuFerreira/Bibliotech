using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ReadExcel.Model.DAO;
using ReadExcel.Model.Entities;
using ReadExcel.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace ReadExcel
{
    public class Menu
    {
        public static AuthorService AuthorService { get; set; }
        public static BookService BookService { get; set; }
        public static ExemplaryService ExemplaryService { get; set; }


        private static readonly DialogService dialogService = new DialogService();

        public static void Render()
        {
            AuthorService = new AuthorService();
            AuthorService.LoadAuthors().Wait();

            BookService = new BookService();
            BookService.LoadBooks().Wait();

            ExemplaryService = new ExemplaryService();

            int option = -1;
            while (option != 0)
            {
                option = -1;
                Console.Clear();

                Console.WriteLine("Escolha uma das Opções: ");
                Console.WriteLine("1 - Adicionar Novos Leitores");
                Console.WriteLine("2 - Adicionar Novos Livros");
                Console.WriteLine("3 - Adicionar Exemplares");
                Console.WriteLine("0 - Sair");

                string line = Console.ReadLine();
                if (int.TryParse(line, out int value))
                {
                    option = value;
                }

                switch (option)
                {
                    case 1:
                        AddNewAuthors();
                        break;
                    case 2:
                        AddNewBooks();
                        break;
                    case 3:
                        AddNewExemplaries();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void AddNewAuthors()
        {
            string filePath = dialogService.OpenSheetDialog();
            if (filePath == string.Empty)
            {
                return;
            }

            ISheet sheet = OpenExcelFile(filePath);
            AuthorService.ReadSheetToGetAuthors(sheet);
            AuthorService.AddNewToDatabase().Wait();
        }

        private static void AddNewBooks()
        {
            string filePath = dialogService.OpenSheetDialog();
            if (filePath == string.Empty)
            {
                return;
            }

            ISheet sheet = OpenExcelFile(filePath);
            BookService.ReadSheetToGetBooks(sheet);
            BookService.AddNewToDatabase().Wait();
        }

        private static void AddNewExemplaries()
        {
            List<Branch> branches = new DAOBranch().GetAll();
            Branch selectedBranch = null;

            int op = -1;
            while (op == -1)
            {
                Console.Clear();
                Console.WriteLine("Escolha uma escola!!!");
                for (int i = 0; i < branches.Count; i++)
                {
                    Console.WriteLine($"{i} - {branches[i].Name}");
                }

                string line = Console.ReadLine();
                if (int.TryParse(line, out int result) == false)
                {
                    continue;
                }

                op = result;
                if (op >= branches.Count)
                {
                    continue;
                }

                selectedBranch = branches[op];
                break;
            }

            string filePath = dialogService.OpenSheetDialog();
            if (filePath == string.Empty)
            {
                return;
            }

            ISheet sheet = OpenExcelFile(filePath);
            ExemplaryService.ReadSheetToGetExemplaries(sheet);
            ExemplaryService.AddNewToDatabase(selectedBranch).Wait();
        }

        private static ISheet OpenExcelFile(string path)
        {
            Stream excelFile = new FileStream(path, FileMode.Open);
            XSSFWorkbook sheets = new XSSFWorkbook(excelFile);
            return sheets.GetSheetAt(0);
        }
    }
}
