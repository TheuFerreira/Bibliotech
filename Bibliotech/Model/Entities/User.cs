using Bibliotech.Model.Entities.Enums;
using System;

namespace Bibliotech.Model.Entities
{
    public class User
    {
        public int IdUser { get; set; }
        public TypeUser TypeUser { get; set; }
        public School Branch { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? Telephone { get; set; }
        public Address Address { get; set; }
        public Status Status { get; set; }

        public User()
        {
            IdUser = -1;
        }

        public User(int idUser, TypeUser typeUser, string name)
        {
            IdUser = idUser;
            TypeUser = typeUser;
            Name = name;
            Status = Status.Active;
        }

    }
}
