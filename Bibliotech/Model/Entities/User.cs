using Bibliotech.Model.Entities.Enums;
using System;

namespace Bibliotech.Model.Entities
{
    public class User
    {
        public int IdUser { get; set; }
        public TypeUser TypeUser { get; set; }
        public Branch Branch { get; set; }
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
            Branch = new Branch();
            Address = new Address();
        }

        public User(int idUser, TypeUser typeUser, string name)
        {
            IdUser = idUser;
            TypeUser = typeUser;
            Name = name;
            Status = Status.Active;
        }

        public string GetBirthDate()
        {
            return BirthDate.HasValue ? BirthDate.Value.ToString("dd/MM/yyyy") : string.Empty;
        }

        public string GetTelephone()
        {
            return Telephone.ToString();
        }

        public bool IsController()
        {
            return TypeUser == TypeUser.Controller;
        }

        public bool IsUser()
        {
            return TypeUser == TypeUser.User;
        }

        public override bool Equals(object obj)
        {
            return obj is User user &&
                   IdUser == user.IdUser;
        }

        public override int GetHashCode()
        {
            return -2101027685 + IdUser.GetHashCode();
        }
    }
}
