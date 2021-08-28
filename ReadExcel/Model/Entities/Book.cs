using System.Collections.Generic;

namespace ReadExcel.Model.Entities
{
    public class Book
    {
        public int IdBook { get; set; }
        public int IdExemplary { get; set; }
        public int Index { get; set; }
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
    }
}
