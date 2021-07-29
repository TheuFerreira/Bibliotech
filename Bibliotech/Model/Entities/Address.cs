namespace Bibliotech.Model.Entities
{
    public class Address
    {
        public int IdAddress { get; set; }
        public string City { get; set; }
        public string Neighborhood { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }

        public Address()
        {
            IdAddress = -1;
        }

        public Address(string city, string neighborhood, string street, string number, string complement)
        {
            City = city;
            Neighborhood = neighborhood;
            Street = street;
            Number = number;
            Complement = complement;
        }

        public Address(int idAddress, string city, string neighborhood, string street, string number, string complement)
        {
            IdAddress = idAddress;
            City = city;
            Neighborhood = neighborhood;
            Street = street;
            Number = number;
            Complement = complement;
        }
    }
}
