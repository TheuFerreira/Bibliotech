using Bibliotech.Model.Entities.Enums;
using System.ComponentModel;

namespace Bibliotech.Model.Entities
{
    public class Exemplary : INotifyPropertyChanged
    {
        public int IdExemplary { get; set; }
        public Book Book { get; set; }
        public Branch Branch { get; set; }
        public int IdIndex { get; set; }
        public Status Status
        {
            get => status;
            set
            {
                status = value;
                PropertyChange("Status");
            }
        }

        private Status status;

        public Exemplary()
        {
            IdExemplary = -1;
        }

        public bool IsLost()
        {
            return Status == Status.Lost;
        }

        public bool IsInactive()
        {
            return Status == Status.Inactive;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
