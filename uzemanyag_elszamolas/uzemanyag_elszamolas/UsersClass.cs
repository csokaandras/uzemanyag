using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uzemanyag_elszamolas
{
    class UsersClass
    {
        public int ID;
        public string Name;
        public string Password;
        public int Perm;
        
        public UsersClass(int id, string name, string pass, int perm)
        {
            ID = id;
            Name = name;
            Password = pass;
            Perm = perm;
        }
    }
}
