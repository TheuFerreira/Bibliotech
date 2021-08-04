using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotech.Model.Entities
{
    public class Lector
    {
        private int idLector;

        private int idBranch;

        private int idAddress;

        private int userRegistration;

        private string name;

        private string responsible;

        private DateTime? birthDate;

        private long? phone;

        private int status;

        public int IdLector { get => idLector; set => idLector = value; }
        public int IdBranch { get => idBranch; set => idBranch = value; }
        public int IdAddress { get => idAddress; set => idAddress = value; }
        public int UserRegistration { get => userRegistration; set => userRegistration = value; }
        public string Name { get => name; set => name = value; }
        public string Responsible { get => responsible; set => responsible = value; }
        public DateTime? BirthDate { get => birthDate; set => birthDate = value; }
        public long? Phone { get => phone; set => phone = value; }
        public int Status { get => status; set => status = value; }
    }
}
