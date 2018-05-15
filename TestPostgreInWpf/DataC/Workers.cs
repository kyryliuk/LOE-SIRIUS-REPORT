using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPostgreInWpf.DataC
{
   public abstract class  Workers
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Middle_NameP { get; set; }
        public string Table_Number { get; set; }
        public string Department_Name { get; set; }
        public Workers() { }
        public Workers(string First_name, string Last_name, string Middle_name, string Tabme_no, string Department_name)
        {
            this.First_Name = First_Name;
            this.Last_Name = Last_Name;
            this.Middle_NameP = Middle_NameP;
            this.Table_Number = Table_Number;
            this.Department_Name = Department_Name;
        }
    }
}
