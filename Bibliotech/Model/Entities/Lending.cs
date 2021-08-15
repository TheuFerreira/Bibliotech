using System;
using System.Collections.Generic;

namespace Bibliotech.Model.Entities
{
    public class Lending
    {
        public int IdLending { get; set; }
        public Lector Lector { get; set; }
        public User User { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public List<Exemplary> Exemplaries { get; set; }
    }
}
