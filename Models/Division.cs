using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniusAccruals.Models
{
    public class Division
    {
        public int LINK { get; set; }
        public string C_Name { get; set; }

        public Division(int link, string name)
        {
            this.LINK = link;
            this.C_Name = name;
        }
    }
}
