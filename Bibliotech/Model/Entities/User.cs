using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotech.Model.Entities
{
    public class User
    { 
        public int IdUser { get; set; }
        public int IdTypeUser { get; set; }
        public string IdBranch { get; set; }
        public string Name { get; set; }
        public string NameBranch { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string Telephone { get; set; }
        public int IdAddress { get; set; }
        public int Status { get; set; }

        public User()
        {
            IdUser = -1;
        }
        public User(int idUser, int idTypeUser, string name, string nameBranch)
        {
            IdUser = idUser;
            IdTypeUser = idTypeUser;
            Name = name;
            NameBranch = nameBranch;
        }

    }
}
