using Bibliotech.Model.Entities.Enums;
using System.Collections.Generic;

namespace Bibliotech.Model.Entities
{
    public class Book
    {
        public int IdBook { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public Author Author { get; set; } = new Author();
        public List<Author> Authors { get; set; }
        public string PublishingCompany { get; set; }
        public string Gender { get; set; }
        public string Edition { get; set; }
        public int? Pages { get; set; }
        public int? YearPublication { get; set; }
        public string Language { get; set; }
        public string Volume { get; set; }
        public string Collection { get; set; }
        public Status Status { get; set; }
        public int idExemplary { get; set; }

        public Book()
        {
            IdBook = -1;

            Authors = new List<Author>();
        }

        public string GetAuthorsToString()
        {
            string text = string.Empty;
            for (int i = 0; i < Authors.Count; i++)
            {
                Author author = Authors[i];
                text += author.Name;

                if (i < Authors.Count - 1)
                {
                    text += ", ";
                }
            }

            return text;
        }
    }
}
