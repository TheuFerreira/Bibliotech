using Bibliotech.Model.Entities.Enums;
using System.ComponentModel;

namespace Bibliotech.Model.Entities
{
    public class School : INotifyPropertyChanged
    {
        public int Id_branch { get; set; }
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

        public School()
        {
            Id_branch = -1;
            Address = new Address();
        }

        public School(int id_branch, string name)
        {
            Id_branch = id_branch;
            Name = name;
            Status = Status.Active;
        }

        public School(string name, Address address, long telephone)
        {
            Name = name;
            Address = address;
            Telephone = telephone;
            Status = Status.Active;
        }

        public School(int id_branch, string name, Address address, long telephone)
        {
            Id_branch = id_branch;
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
