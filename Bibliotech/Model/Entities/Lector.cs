using Bibliotech.Model.Entities.Enums;
using System;

namespace Bibliotech.Model.Entities
{
    public class Lector
    {
        public int IdLector { get; set; }
        public int IdBranch { get; set; }
        public Address Address { get; set; }
        public int UserRegistration { get; set; }
        public string Name { get; set; }
        public string Responsible { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? Phone { get; set; }
        public Status Status { get; set; }

        public Lector()
        {
            IdLector = -1;
            Address = new Address();
        }
    }
}
