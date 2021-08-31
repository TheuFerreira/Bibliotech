using System.Collections.Generic;

namespace ReadExcel.Model.Entities
{
    public class Author
    {
        public int IdAuthor { get; set; }
        public string Name { get; set; }

        public Author(int idAuthor, string name)
        {
            IdAuthor = idAuthor;
            Name = name;
        }

        public Author(string name)
        {
            IdAuthor = -1;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is Author author &&
                   Name == author.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}
