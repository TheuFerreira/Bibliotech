using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotech.Model.Entities
{
    class Address
    {
        int id_address;
        String city;
        String neighborhood;
        String street;
        String number;
        String complement;

        public Address()
        {

        }

        public Address(string city, string neighborhood, string street, string number, string complement)
        {
            this.city = city;
            this.neighborhood = neighborhood;
            this.street = street;
            this.number = number;
            this.complement = complement;
        }

        public Address(int id_address, string city, string neighborhood, string street, string number, string complement)
        {
            this.id_address = id_address;
            this.city = city;
            this.neighborhood = neighborhood;
            this.street = street;
            this.number = number;
            this.complement = complement;
        }

        public int Id_address { get => id_address; set => id_address = value; }
        public string City { get => city; set => city = value; }
        public string Neighborhood { get => neighborhood; set => neighborhood = value; }
        public string Street { get => street; set => street = value; }
        public string Number { get => number; set => number = value; }
        public string Complement { get => complement; set => complement = value; }
    }
}
