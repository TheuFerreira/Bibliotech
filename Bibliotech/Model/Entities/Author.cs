using Bibliotech.Model.Entities.Enums;

namespace Bibliotech.Model.Entities
{
    public class Author
    {
        public int IdAuthor { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }

        public Author()
        {
            IdAuthor = -1;
            Status = Status.Active;
        }

        public override bool Equals(object obj)
        {
            return obj is Author author &&
                   IdAuthor == author.IdAuthor;
        }

        public override int GetHashCode()
        {
            return -854221301 + IdAuthor.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
