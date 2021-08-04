using Bibliotech.Model.Entities.Enums;

namespace Bibliotech.Model.Entities
{
    public class Exemplary
    {
        public int IdExemplary { get; set; }
        public Book Book { get; set; }
        public Branch Branch { get; set; }
        public int IdIndex { get; set; }
        public Status Status { get; set; }

        public Exemplary()
        {
            IdExemplary = -1;
        }
    }
}
