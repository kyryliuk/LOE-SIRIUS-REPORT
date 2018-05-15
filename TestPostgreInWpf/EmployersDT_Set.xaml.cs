using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;
using TestPostgreInWpf.DataC;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace TestPostgreInWpf
{
/// <summary>
    /// Interaction logic for EmployersDT_Set.xaml
    /// Вікно для налаштування індивідуального графіка 
    /// </summary>
    public partial class EmployersDT_Set : Window
    {
        XmlSerializer formatter = new XmlSerializer(typeof(List<WorkerIndivid>));//серіалізатор типу обєктів індивідуального графіка
        List<string> list;
        public List<WorkerIndivid> list_workers = new List<WorkerIndivid>();    //контейнер обєктів для індивідуального графіка   
        public EmployersDT_Set()
        {
            InitializeComponent();           
        }                     
/// <summary>
/// кнопочка для додавання нового працівника
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void Add_Employer_Click(object sender, RoutedEventArgs e)
        {
            View_Employers.ItemsSource = null;
            NpgsqlCommand com = new NpgsqlCommand("select first_name,last_name,middle_name,table_no, departments.name from workers,departments where workers.department=departments.id and table_no="+listboxIntellishell.SelectedItem.ToString().Split(' ').Last(), MainWindow.con);
            NpgsqlDataReader reader;
            reader = com.ExecuteReader();
            while (reader.Read())
            {
                list_workers.Add(new WorkerIndivid(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), Time_startE.Text, Time_EndE.Text));
            }            
            View_Employers.ItemsSource = list_workers;
            reader.Close();        
        }
/// <summary>
/// Функція для ініціалізації 
/// виконується запит і стягується в ліст бокс
/// </summary>
        public void initialALL()
        {            
            NpgsqlCommand com = new NpgsqlCommand("select first_name,last_name,middle_name,table_no from workers", connection: MainWindow.con);
            NpgsqlDataReader reader3;
            reader3 = com.ExecuteReader();
            list = new List<string>();
            while (reader3.Read())
            {
                try
                {                    
                    list.Add(reader3[0] + " " + reader3[1] + " " + reader3[2] + " " + reader3[3]);
                }
                catch { }
            }
            listboxIntellishell.ItemsSource = list;
            reader3.Dispose();
            try
            {
                using (FileStream fs = new FileStream("workers.xml", FileMode.Open))
                {
                    List<WorkerIndivid> newListWorkers = (List<WorkerIndivid>)formatter.Deserialize(fs);
                    View_Employers.ItemsSource = newListWorkers;
                    list_workers = newListWorkers;
                }
            }
            catch { MessageBox.Show("Файл workers.xml відсутній"); }
        }
        protected override void OnClosing(CancelEventArgs e) => base.OnClosing(e);
/// <summary>
/// кнопка для зберігання даних в xml
/// завжди стираємо спочатку файл , як би б чистимо)
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("workers.xml"))
            {
                File.Delete("workers.xml");
            }
            using (FileStream fs = new FileStream("workers.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, list_workers);
                System.Windows.MessageBox.Show("Збережено");
            }
        }
/// <summary>
/// кнопка для видалення вказаної стрічки
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            list_workers.RemoveAt(View_Employers.SelectedIndex);
            View_Employers.ItemsSource = null;
            View_Employers.ItemsSource = list_workers;
        }
/// <summary>
/// Пошук по лістбоксу інтелшелі)
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void Find_tableN_TextChanged(object sender, TextChangedEventArgs e)
        {
            string typedString = Find_tableN.Text;
            List<string> autolist = new List<string>();
            autolist.Clear();
            foreach (string item in list)//
            {
                if (item.Contains(typedString))
                { autolist.Add(item); }
            }
            if (autolist.Count > 0) { listboxIntellishell.ItemsSource = autolist; listboxIntellishell.Visibility = Visibility.Visible; }
            else if (Find_tableN.Text.Equals(""))
            { listboxIntellishell.Visibility = Visibility.Collapsed; listboxIntellishell.ItemsSource = null; }
            else { listboxIntellishell.Visibility = Visibility.Collapsed; listboxIntellishell.ItemsSource = null; }
        }
/// <summary>
/// кнопка для закриття даного вікна
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
