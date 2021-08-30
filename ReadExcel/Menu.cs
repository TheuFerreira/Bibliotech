using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ReadExcel.Services;
using System;
using System.IO;

namespace ReadExcel
{
    public class Menu
    {
        public static AuthorService AuthorService { get; set; }
        public static BookService BookService {  get; set; }

        public static void Render()
        {
            AuthorService = new AuthorService();
            AuthorService.LoadAuthors().Wait();

            BookService = new BookService();
            BookService.LoadBooks().Wait();

            int option = -1;
            while (option != 0)
            {
                option = -1;
                Console.Clear();

                Console.WriteLine("Escolha uma das Opções: ");
                Console.WriteLine("1 - Adicionar Novos Leitores");
                Console.WriteLine("2 - Adicionar Novos Livros");
                Console.WriteLine("0 - Sair");

                string line = Console.ReadLine();
                if (int.TryParse(line, out int value))
                    option = value;
            
                switch (option)
                {
                    case 1:
                        AddNewAuthors();
                        break;
                    case 2:
                        AddNewBooks();
                        break;
                }
            }
        }

        private static void AddNewAuthors()
        {
            DialogService dialogService = new DialogService();
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
            DialogService dialogService = new DialogService();
            string filePath = dialogService.OpenSheetDialog();
            if (filePath == string.Empty)
            {
                return;
            }

            ISheet sheet = OpenExcelFile(filePath);
            BookService.ReadSheetToGetBooks(sheet);
            BookService.AddNewToDatabase().Wait();
        }

        private static ISheet OpenExcelFile(string path)
        {
            Stream excelFile = new FileStream(path, FileMode.Open);
            XSSFWorkbook sheets = new XSSFWorkbook(excelFile);
            return sheets.GetSheetAt(0);
        }
    }
}
