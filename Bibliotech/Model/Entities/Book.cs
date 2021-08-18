using Bibliotech.Model.Entities.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Bibliotech.Model.Entities
{
    public class Book
    {
        private DAO.DAOAuthor DAOAuthor;
        public int IdBook { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
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

        public Book()
        {
            IdBook = -1;
        }
        public Book(string title, string subtitle, List<Author> authors, string publishingCompany, string gender,
            string edition, int? pages, int? year, string language, string volume, string collection)
        {
            Title = title;
            Subtitle = subtitle;
            PublishingCompany = publishingCompany;
            Authors = authors;
            Gender = gender;
            Edition = edition;
            Pages = pages;
            YearPublication = year;
            Language = language;
            Volume = volume;
            Collection = collection;
            Status = Status.Active;
        }
       
        /*
        public override string ToString()
        {
            string authors = string.Empty;
            foreach (Author author in Authors)
            {
                authors += $"{author.Name}";

                if (author != Authors.ElementAt(Authors.Count - 1))
                {
                    authors += ", ";
                }
            }

            return authors;
        }
        */
    }
}
