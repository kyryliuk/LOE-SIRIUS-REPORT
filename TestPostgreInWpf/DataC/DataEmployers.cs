using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPostgreInWpf.DataC
{
    [Serializable]
   public  class DataEmployers:Workers
    {      
       public string EmpDate { get; set; }       
       public string card { get; set; }
       public string worker { get; set;}
       public DataEmployers() { }
       public DataEmployers(string Last_name ,string First_name, string middle_name, string card,string worker ,string id,string data,string table_n,string namedep)
       {
           this.Last_Name = Last_name;
           this.First_Name = First_name;
           this.Middle_NameP = middle_name;
           this.card = card;
           this.worker = worker;
           this.EmpDate = data;
           this.Table_Number = table_n;
           this.Department_Name = namedep;
       }
    }
}
