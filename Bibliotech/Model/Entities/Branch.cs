using Bibliotech.Model.Entities.Enums;
using System.ComponentModel;

namespace Bibliotech.Model.Entities
{
    public class Branch : INotifyPropertyChanged
    {
        public int IdBranch { get; set; }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public Address Address { get; set; }
        public long? Telephone { get; set; }
        public Status Status { get; set; }

        private string name;

        public Branch()
        {
            IdBranch = -1;
            Address = new Address();
        }

        public Branch(int idBranch, string name)
        {
            IdBranch = idBranch;
            Name = name;
            Status = Status.Active;
        }

        public Branch(string name, Address address, long telephone)
        {
            Name = name;
            Address = address;
            Telephone = telephone;
            Status = Status.Active;
        }

        public Branch(int idBranch, string name, Address address, long telephone)
        {
            IdBranch = idBranch;
            Name = name;
            Address = address;
            Telephone = telephone;
            Status = Status.Active;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
