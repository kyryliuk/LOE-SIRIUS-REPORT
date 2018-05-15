using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestPostgreInWpf;
using System.IO;
using System.Xml.Serialization;

namespace TestPostgreInWpf.DataC
{
    [Serializable]
    public  class ConnectionSettings 
    {
        public string Ip_address { get; set; }
        public string Port { get; set; }
        public string Db_name { get; set; }
        public string user { get; set; }
        public string Password { get; set; }
        public ConnectionSettings() { }
        public ConnectionSettings(string Ip_address, string Port, string Db_name, string user, string Password)
        {
            Ip_address = this.Ip_address;
            Port = this.Port;
            Db_name = this.Db_name;
            user = this.user;
            Password = this.Password;
        }
/// <summary>
/// запіхаєм в файл конфігів
/// </summary>
/// <param name="Ip_address"></param>
/// <param name="Port"></param>
/// <param name="Db_name"></param>
/// <param name="user"></param>
/// <param name="Password"></param>
        public void PrintInFile(string Ip_address, string Port, string Db_name, string user, string Password)
        {
            using (StreamWriter writer = File.CreateText("config.txt"))
            {
                writer.Flush();
                writer.WriteLine(Ip_address+" "+Port+" "+Db_name+" "+user+" "+Password+" ");
            }
        }
/// <summary>
 /// ф-я для десеріалізації 
 /// </summary>
 /// <param name="co"></param> 
        public static void Desereal(ConnectionSettings co)
        {                        
               XmlSerializer formatter = new XmlSerializer(typeof(ConnectionSettings));
            using (FileStream fs = new FileStream("ConnectionsSettings.xml", FileMode.OpenOrCreate))
            {
                co = (ConnectionSettings)formatter.Deserialize(fs);                  
            }
        }
/// <summary>
/// Функція для серіалізації класу
/// </summary>
        public static void Serial()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ConnectionSettings));
            using (FileStream fs = new FileStream("ConnectionsSettings.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, MainWindow.conS);
                System.Windows.MessageBox.Show("Объект сериализован");
            }
        }

    }
   
}
