using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPostgreInWpf.DataC
{
    [Serializable]
  public  class WorkerIndivid:Workers
    {
        public string Time_startW { get; set; }
        public string Time_endW { get; set; }
        public WorkerIndivid() { }
        public WorkerIndivid(string First_name, string Last_name, string Middle_name, string Table_no, string Department_name,string timeS,string timeE)
        {
            this.First_Name =First_name;
            this.Last_Name = Last_name;
            this.Middle_NameP = Middle_name;
            this.Table_Number = Table_no;
            this.Department_Name = Department_name;
            this.Time_startW = timeS;
            this.Time_endW = timeE;
        }
    }
}
