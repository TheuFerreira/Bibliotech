using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotech.Model.Entities
{
    class School
    {
        int id_branch;
        String name;
        int id_address;
        String telephone;
        int status;

        public School()
        {

        }
        public School(string name, int id_address, string telephone, int status)
        {
            this.name = name;
            this.id_address = id_address;
            this.telephone = telephone;
            this.status = status;
        }

        public School(int id_branch, string name, int id_address, string telephone, int status)
        {
            this.id_branch = id_branch;
            this.name = name;
            this.id_address = id_address;
            this.telephone = telephone;
            this.status = status;
        }

        public int Id_branch { get => id_branch; set => id_branch = value; }
        public string Name { get => name; set => name = value; }
        public int Id_address { get => id_address; set => id_address = value; }
        public string Telephone { get => telephone; set => telephone = value; }
        public int Status { get => status; set => status = value; }
    }
}
