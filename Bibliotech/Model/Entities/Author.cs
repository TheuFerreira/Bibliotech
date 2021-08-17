using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        public Author(string name)
        {
            Name = name;
            Status = Status.Active;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
