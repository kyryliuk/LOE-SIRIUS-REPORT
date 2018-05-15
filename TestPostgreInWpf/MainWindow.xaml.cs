///Ну здарова
///Спочатку писав на .net framework 4.6 но так як пк там з xp sp1~3 і канає тільки .net framework 4.0 то все переробляв( Ну але побачив різницю між версіями
///Кароч це прога клієнт для сервера бд
///Бд використовується в даному випадку PostgreSql
///Так як інформації про івенти праціників в бд знаходиться досить багато , пішов шляхом використання кожен раз окремих запитів
///Там кароч комп старуй)
///Основні виключення описав , але можуть виникати нові , тому потрібно стежити тестувати ...

using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TestPostgreInWpf.DataC;
using System.Windows.Threading;

namespace TestPostgreInWpf
{
 /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Ну нармас тут на хр вінду , постгрес самої старої версії але як не дивно норм ше працює. Досить швидко. 
    /// </summary>  
    public partial class MainWindow :System.Windows.Window
    {
        #region Глобальні дані
        public static List<DataEmployers> listread;                          //глобальна колекція працівників, з неї ми формуємо файл xls
        public static string direction;                                      // напрямок руху 0-зайшов 1-вийшов  
        public static ConnectionSettings conS = new ConnectionSettings();    // Глобальна зміна в якій зберігаються налаштування зєднання
        public static NpgsqlConnection con;                                  // конфіга для налаштування конекшн стрінга для postgresql 
        #endregion
        #region Ініціалізація вікон
        Connection Connection_Window = new Connection();                     
        Loading_window loading_Window=new Loading_window();
        DataGridWindow DataGridWindow;
        About about;
        HelpWindow helpWindow;
        EmployersDT_Set Set_Emp_times;
        #endregion
        List<DataEmployers> Employers = new List<DataEmployers>();
        DispatcherTimer dT = new DispatcherTimer();//таймер     
        List<string> newlist;//Колекція для ініціалізації і відправки даних в ліст бокс
        DateTime dt1, dt2;//Початкові і кінцева дата
        TimeSpan delta;//кількість днів між початковою і кінцевою датою
        /// <summary>
        /// конструктор головного вікна
        /// ініціалізуємо деякі дані
        /// </summary>
        public MainWindow()
        {           
            InitializeComponent();            
            SetConnection();//встановлення зєднання
            direction = "0";            
            ChoiseQuery.SelectedIndex = 0;           
            ChoiseQuery.Items.Add("Для всіх відділів");
            ChoiseQuery.Items.Add("Для вказаного відділу");
            ChoiseQuery.Items.Add("Для окремого працівника");
            ChoiseQuery.Items.Add("Для працівників індивідуального графіка");
            IntelliSenseTextBox.TextChanged += new TextChangedEventHandler(IntelliSenseTextBox_TextChanged);            
        }
/// <summary>
        /// Зчитує інформацію з файла конфігурації
        /// </summary>
        public void ReadFromConfig()
        {           
            string input = null;
            string[] tmps1 = null;
            try
            {
                using (StreamReader sr = File.OpenText("config.txt"))
                {
                    input = sr.ReadLine();
                    tmps1 = input.Split(' ');
                }
            }
            catch { MessageBox.Show("Файл не знайдено","OK"); }     
            conS.Ip_address = tmps1[0].ToString();
            conS.Port = tmps1[1].ToString();
            conS.Db_name = tmps1[2].ToString();
            conS.user = tmps1[3].ToString();
            conS.Password = tmps1[4].ToString();
        }
/// <summary>
        /// Вікно для налаштування підключення
        /// Кнопка виклику з головного вікна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {           
            Connection_Window.Show();           
        }
/// <summary>
     /// Комбо бокс в якому залежно від вибору ініціалізуються елементи через sql запит
     /// Виводить дані в ліст бокс
     /// </summary>
     /// <param name="sender"></param>
     /// <param name="e"></param>
        private void ChoiseQuery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            newlist = new List<string>();
            Thickness marginmain = Query1.Margin;//муть(на біндінги перевязати!)
            marginmain.Top = 385;
            Thickness margin = Query1.Margin;
            margin.Top = 256;            
            if (ChoiseQuery.SelectedIndex == 0) { Check_Events.IsEnabled = false; Check_redirect.IsEnabled = true; List_box_intellishell.ItemsSource = null; List_box_intellishell.Visibility = Visibility.Hidden; IntelliSenseTextBox.Visibility = Visibility.Hidden; Window.Height = 335; Query1.Margin=margin; }
            if (ChoiseQuery.SelectedIndex == 1) {newlist = RunQueryReturn("select name from departments", 1); Check_Events.IsEnabled = false; Check_redirect.IsEnabled = true; List_box_intellishell.ItemsSource = newlist; List_box_intellishell.Visibility = Visibility.Visible; IntelliSenseTextBox.Visibility = Visibility.Visible; Window.Height = 460.5; Query1.Margin = marginmain; }
            if (ChoiseQuery.SelectedIndex == 2) {newlist = RunQueryReturn("select first_name,last_name,middle_name,table_no from workers", 2); Check_Events.IsEnabled = true; Check_redirect.IsEnabled = true; List_box_intellishell.ItemsSource = newlist; List_box_intellishell.Visibility = Visibility.Visible; IntelliSenseTextBox.Visibility = Visibility.Visible; Window.Height = 460.5; Query1.Margin = marginmain; }
            if (ChoiseQuery.SelectedIndex == 3) { List_box_intellishell.Visibility = Visibility.Hidden; IntelliSenseTextBox.Visibility = Visibility.Hidden; Check_Events.IsEnabled = false; Window.Height = 335; Query1.Margin = margin; Check_redirect.IsEnabled = false; }           
        }
///<summary>
        ///Метод для ініціалізації List_box_intellishell 
        ///Отримує стрічку sql запиту і вибір відображення даних
        ///</summary>       
        public List<string> RunQueryReturn(string sql_query, int choise)
        {
            List<string> listr = new List<string>();
           // List_box_data.Items.Clear();
            NpgsqlCommand com = new NpgsqlCommand(sql_query, con);
            NpgsqlDataReader reader1;
            reader1 = com.ExecuteReader();
            while (reader1.Read())
            {
                try
                {
                    if (choise == 2) {  listr.Add(reader1[0] + " " + reader1[1] + " " + reader1[2] + " " + reader1[3]); }
                    if (choise == 1) { listr.Add(reader1[0].ToString()); }
                }
                catch { }
            }            
            reader1.Dispose();
            com.Dispose();
            return listr;
        }
/// <summary>
        /// Кнопка виконання 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dt1 = DateTime.Parse(Start_date.Text);
                dt2 = DateTime.Parse(End_date.Text);
            }
            catch { MessageBox.Show("Введіть дату");  }            
            try { delta = dt2 - dt1; }
            catch { MessageBox.Show("Дата введена невірно");}
            int count = delta.Days;
            List<DateTime> allDates = new List<DateTime>();
            for (int i = 0; i <= count; i++)
            {
                allDates.Add(new DateTime(dt1.Year, dt1.Month, dt1.Day).AddDays(i));
            }           
            listread = new List<DataEmployers>(count);
            Dictionary<int, List<string> >listread1= new Dictionary<int, List<string>>(count);
            List<string> tmp_list;
            //Для всіх з врахуванням повторного входу
            if (ChoiseQuery.SelectedIndex == 0 && Check_redirect.IsChecked==true)//when  redirect check
            {             
                for (int i = 0; i <= count; i++)//card id to list
                {
                    tmp_list = new List<string>();
                    NpgsqlCommand cmd_tmp1 = new NpgsqlCommand();
                    cmd_tmp1.CommandText = @"select distinct card from events where etimesirius>='" + allDates[i].ToShortDateString() + " 00:00' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and direction=" + MainWindow.direction + " and worker<>0";
                    cmd_tmp1.Connection = con;
                    NpgsqlDataReader reader1;
                    reader1 = cmd_tmp1.ExecuteReader();
                    while (reader1.Read())
                    {
                        tmp_list.Add(reader1[0].ToString());
                    }
                    listread1.Add(i,tmp_list);                    
                    cmd_tmp1.Dispose();
                    reader1.Close();
                }                          
                for (int i = 0; i <= count; i++)//main query
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.CommandText = @"select F.last_name, F.first_name, F.middle_name, F.card, F.worker, departments.id, F.btm,  F.table_no, departments.name from ( select E.card, E.worker, E.mtm, E.btm, workers.last_name, workers.first_name, workers.middle_name, workers.table_no, workers.department from ( select C.card, C.worker, C.mtm, D.btm from ( select A.card, A.worker, B.mtm from (select distinct card, worker from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker)A LEFT JOIN (select card, worker, min(etimeserver) as MTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius<='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + "and worker<>0 group by card, worker )B ON A.card = B.card AND A.worker = B.worker) C LEFT JOIN (select card, worker, min(etimeserver) as BTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker )D ON C.card = D.card AND C.worker = D.worker)" + "E LEFT JOIN workers ON E.worker = workers.id) F LEFT JOIN departments ON F.department = departments.id order by btm";
                    cmd.Connection = con;
                    NpgsqlDataReader reader;                   
                    reader = cmd.ExecuteReader();                    
                    while(reader.Read())
                    {
                        foreach(string s in listread1[i])
                        {
                            if (s == reader[3].ToString())
                            { goto breakout; }                         
                        }                        
                        listread.Add(new DataEmployers(reader[1].ToString(), reader[0].ToString(), reader[2].ToString(),/**/ reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                    breakout:
                      continue;
                    }
                    cmd.Dispose();
                    reader.Close();
                }                
                DataGridWindow=new DataGridWindow(listread);                
            }
            //Для всіх без врахування повторного входу
            if (ChoiseQuery.SelectedIndex == 0 && Check_redirect.IsChecked == false)//не враховується повторний вхід для всіх 
            {
                for (int i = 0; i <= count; i++)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand
                    {
                        CommandText = @"select F.last_name, F.first_name, F.middle_name, F.card, F.worker, departments.id, F.btm,  F.table_no, departments.name from ( select E.card, E.worker, E.mtm, E.btm, workers.last_name, workers.first_name, workers.middle_name, workers.table_no, workers.department from ( select C.card, C.worker, C.mtm, D.btm from ( select A.card, A.worker, B.mtm from (select distinct card, worker from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker)A LEFT JOIN (select card, worker, min(etimeserver) as MTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius<='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + "and worker<>0 group by card, worker )B ON A.card = B.card AND A.worker = B.worker) C LEFT JOIN (select card, worker, min(etimeserver) as BTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker )D ON C.card = D.card AND C.worker = D.worker)" + "E LEFT JOIN workers ON E.worker = workers.id) F LEFT JOIN departments ON F.department = departments.id order by btm",
                        Connection = con
                    };
                    NpgsqlDataReader reader;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listread.Add(new DataEmployers(reader[1].ToString(), reader[0].ToString(), reader[2].ToString(),/**/ reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                    }
                    cmd.Dispose();
                    reader.Close();
                }
                DataGridWindow = new DataGridWindow(listread);
            }
            //Для департаменту з врахуванням повторного входу
            if (ChoiseQuery.SelectedIndex == 1 && Check_redirect.IsChecked==true)
            {                //card id to list
                for (int i = 0; i <= count; i++)
                {
                    tmp_list = new List<string>();
                    NpgsqlCommand cmd_tmp1 = new NpgsqlCommand
                    {
                        CommandText = @"select distinct card from events where etimesirius>='" + allDates[i].ToShortDateString() + " 00:00' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and direction=" + MainWindow.direction + " and worker<>0",
                        Connection = con
                    };//query for departments
                    NpgsqlDataReader reader1;
                    reader1 = cmd_tmp1.ExecuteReader();
                    while (reader1.Read())
                    {
                        tmp_list.Add(reader1[0].ToString());
                    }
                    listread1.Add(i, tmp_list);
                    cmd_tmp1.Dispose();
                    reader1.Close();
                }
                for (int i = 0; i <= count; i++)
                {
                    NpgsqlCommand cmd1 = new NpgsqlCommand();
                    cmd1.CommandText = @"select F.last_name, F.first_name, F.middle_name, F.card, F.worker, departments.id, F.btm,  F.table_no, departments.name from ( select E.card, E.worker, E.mtm, E.btm, workers.last_name, workers.first_name, workers.middle_name, workers.table_no, workers.department from ( select C.card, C.worker, C.mtm, D.btm from ( select A.card, A.worker, B.mtm from (select distinct card, worker from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker)A LEFT JOIN (select card, worker, min(etimeserver) as MTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius<='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + "and worker<>0 group by card, worker )B ON A.card = B.card AND A.worker = B.worker) C LEFT JOIN (select card, worker, min(etimeserver) as BTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker )D ON C.card = D.card AND C.worker = D.worker)" + "E LEFT JOIN workers ON E.worker = workers.id) F LEFT JOIN departments ON F.department = departments.id where departments.name like '" + DepartamentParse(List_box_intellishell.SelectedItem.ToString()) + "' order by btm";
                    cmd1.Connection = con;
                    NpgsqlDataReader reader;
                    reader = cmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (string s in listread1[i])
                        {
                            if (s == reader[3].ToString())
                            { goto breakout1; }
                        }
                        listread.Add(new DataEmployers(reader[1].ToString(), reader[0].ToString(), reader[2].ToString(),/**/ reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                    breakout1:
                        continue;    
                    }
                    cmd1.Dispose();
                    reader.Close();
                }
                DataGridWindow = new DataGridWindow(listread);
            }
            //Для департаменту без повторного входу
            if (ChoiseQuery.SelectedIndex == 1 && Check_redirect.IsChecked == false )
            {
                for (int i = 0; i <= count; i++)
                {
                    NpgsqlCommand cmd1 = new NpgsqlCommand
                    {
                        CommandText = @"select F.last_name, F.first_name, F.middle_name, F.card, F.worker, departments.id, F.btm,  F.table_no, departments.name from ( select E.card, E.worker, E.mtm, E.btm, workers.last_name, workers.first_name, workers.middle_name, workers.table_no, workers.department from ( select C.card, C.worker, C.mtm, D.btm from ( select A.card, A.worker, B.mtm from (select distinct card, worker from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker)A LEFT JOIN (select card, worker, min(etimeserver) as MTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius<='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + "and worker<>0 group by card, worker )B ON A.card = B.card AND A.worker = B.worker) C LEFT JOIN (select card, worker, min(etimeserver) as BTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker )D ON C.card = D.card AND C.worker = D.worker)" + "E LEFT JOIN workers ON E.worker = workers.id) F LEFT JOIN departments ON F.department = departments.id where departments.name like '" + DepartamentParse(List_box_intellishell.SelectedItem.ToString()) + "' order by btm",
                        Connection = con
                    };
                    NpgsqlDataReader reader;
                    reader = cmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        listread.Add(new DataEmployers(reader[1].ToString(), reader[0].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                    }
                    cmd1.Dispose();
                    reader.Close();
                }
                DataGridWindow = new DataGridWindow(listread);
            }
            //Для одного працівника не всі події
            if (ChoiseQuery.SelectedIndex == 2 && Check_redirect.IsChecked==true && Check_Events.IsChecked==false)
            {
                for (int i = 0; i <= count; i++)
                {
                    tmp_list = new List<string>();
                    NpgsqlCommand cmd_tmp1 = new NpgsqlCommand();
                    cmd_tmp1.CommandText = @"select F.last_name, F.first_name, F.middle_name, F.card, F.worker, departments.id, F.btm,  F.table_no, departments.name from ( select E.card, E.worker, E.mtm, E.btm, workers.last_name, workers.first_name, workers.middle_name, workers.table_no, workers.department from ( select C.card, C.worker, C.mtm, D.btm from ( select A.card, A.worker, B.mtm from (select distinct card, worker from events where etimesirius>='" + allDates[i].ToShortDateString() + " 00:00' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker)A LEFT JOIN (select card, worker, min(etimeserver) as MTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " 00:00 ' and etimesirius<='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and direction=" + MainWindow.direction + "and worker<>0 group by card, worker )B ON A.card = B.card AND A.worker = B.worker) C LEFT JOIN (select card, worker, min(etimeserver) as BTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " 00:00' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker )D ON C.card = D.card AND C.worker = D.worker)" + "E LEFT JOIN workers ON E.worker = workers.id where workers.table_no='" + List_box_intellishell.SelectedItem.ToString().Split(' ').Last() + "') F LEFT JOIN departments ON F.department = departments.id order by btm";
                    cmd_tmp1.Connection = con;                   
                    NpgsqlDataReader reader1;
                    reader1 = cmd_tmp1.ExecuteReader();
                    while (reader1.Read())
                    {
                        tmp_list.Add(reader1[3].ToString());
                    }
                    listread1.Add(i, tmp_list);
                    cmd_tmp1.Dispose();
                    reader1.Close();
                }
                for (int i = 0; i <= count; i++)
                {
                    NpgsqlCommand cmd2 = new NpgsqlCommand();
                    cmd2.CommandText = @"select F.last_name, F.first_name, F.middle_name, F.card, F.worker, departments.id, F.btm,  F.table_no, departments.name from ( select E.card, E.worker, E.mtm, E.btm, workers.last_name, workers.first_name, workers.middle_name, workers.table_no, workers.department from ( select C.card, C.worker, C.mtm, D.btm from ( select A.card, A.worker, B.mtm from (select distinct card, worker from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker)A LEFT JOIN (select card, worker, min(etimeserver) as MTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius<='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + "and worker<>0 group by card, worker )B ON A.card = B.card AND A.worker = B.worker) C LEFT JOIN (select card, worker, min(etimeserver) as BTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + Time_start.Text + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + Time_end.Text + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker )D ON C.card = D.card AND C.worker = D.worker)" + "E LEFT JOIN workers ON E.worker = workers.id where workers.table_no='" + List_box_intellishell.SelectedItem.ToString().Split(' ').Last() + "') F LEFT JOIN departments ON F.department = departments.id order by btm";
                      cmd2.Connection = con;                   
                    NpgsqlDataReader reader;
                    reader = cmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (string s in listread1[i])
                        {
                            if (s == reader[3].ToString())
                            { goto breakout3; }
                        }
                        listread.Add(new DataEmployers(reader[1].ToString(), reader[0].ToString(), reader[2].ToString(),/**/ reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                        breakout3:
                        continue;
                    }
                    cmd2.Dispose();
                    reader.Close();
                }
                DataGridWindow = new DataGridWindow(listread);
            }
            //для одного працівника відображення усіх подій
            if (ChoiseQuery.SelectedIndex == 2 && Check_redirect.IsChecked == false && Check_Events.IsChecked == true )//checked all events
            {                
                    NpgsqlCommand cmd2 = new NpgsqlCommand();
                    cmd2.CommandText = @"select workers.last_name, workers.first_name, workers.middle_name, events.etimeserver ,events.direction  , events.controler, departments.name from events,workers, departments  where events.card=workers.card and departments.id=workers.department and workers.table_no=" + List_box_intellishell.SelectedItem.ToString().Split(' ').Last() +" and events.etimeserver>='"+Start_date.Text+" 00:00'and events.etimeserver<='"+End_date.Text+" 23:00'order by events.etimeserver";
                    cmd2.Connection = con;
                    NpgsqlDataReader reader;
                    reader = cmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        listread.Add(new DataEmployers(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(),reader[4].ToString() == "0" ? "Зайшов" : "Вийшов", reader[5].ToString() == "7" ? "На правий турнікет" : "На лівий турнікет", "",reader[3].ToString(), List_box_intellishell.SelectedItem.ToString().Split(' ').Last(), reader[6].ToString()));
                    }
                    cmd2.Dispose();
                    reader.Close();
                DataGridWindow = new DataGridWindow(listread);
            }
            if (ChoiseQuery.SelectedIndex == 2 && Check_redirect.IsChecked == false && Check_Events.IsChecked == false) { MessageBox.Show("Вибір для працівника з даними параметрами неможливий"); }
            if (ChoiseQuery.SelectedIndex == 2 && Check_redirect.IsChecked == true && Check_Events.IsChecked == true) { MessageBox.Show("Вибір для працівника з даними параметрами неможливий"); }
            //Для працівників з окремим графіком
            if (ChoiseQuery.SelectedIndex == 3)
            {
                List<WorkerIndivid> workers = new List<WorkerIndivid>();
                Set_Emp_times = new EmployersDT_Set();
                Set_Emp_times.initialALL();                                         //зчитуємо з файлика збережених працівників
                workers=Set_Emp_times.list_workers;
                foreach (var lst in workers)
                {
                    listread1 = new Dictionary<int, List<string>>(count);
                    for (int i = 0; i <= count; i++)//перевіряємо повторний вхід
                    {
                        tmp_list = new List<string>();
                        NpgsqlCommand cmd_tmp1 = new NpgsqlCommand();
                        cmd_tmp1.CommandText = @"select F.last_name, F.first_name, F.middle_name, F.card, F.worker, departments.id, F.btm,  F.table_no, departments.name from ( select E.card, E.worker, E.mtm, E.btm, workers.last_name, workers.first_name, workers.middle_name, workers.table_no, workers.department from ( select C.card, C.worker, C.mtm, D.btm from ( select A.card, A.worker, B.mtm from (select distinct card, worker from events where etimesirius>='" + allDates[i].ToShortDateString() + " 00:00' and etimesirius <='" + allDates[i].ToShortDateString() + " " + lst.Time_startW+ "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker)A LEFT JOIN (select card, worker, min(etimeserver) as MTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " 00:00 ' and etimesirius<='" + allDates[i].ToShortDateString() + " " + lst.Time_startW + "' and direction=" + MainWindow.direction + "and worker<>0 group by card, worker )B ON A.card = B.card AND A.worker = B.worker) C LEFT JOIN (select card, worker, min(etimeserver) as BTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " 00:00' and etimesirius <='" + allDates[i].ToShortDateString() + " " + lst.Time_startW + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker )D ON C.card = D.card AND C.worker = D.worker)" + "E LEFT JOIN workers ON E.worker = workers.id where workers.table_no='" +lst.Table_Number + "') F LEFT JOIN departments ON F.department = departments.id order by btm";
                        cmd_tmp1.Connection = con;
                        NpgsqlDataReader reader1;
                        reader1 = cmd_tmp1.ExecuteReader();
                        while (reader1.Read())
                        {
                            tmp_list.Add(reader1[3].ToString());
                        }
                        listread1.Add(i, value: tmp_list);
                        cmd_tmp1.Dispose();
                        reader1.Close();
                    }
                    for (int i = 0; i <= count; i++)
                    {
                        NpgsqlCommand cmd2 = new NpgsqlCommand();
                        cmd2.CommandText = @"select F.last_name, F.first_name, F.middle_name, F.card, F.worker, departments.id, F.btm,  F.table_no, departments.name from ( select E.card, E.worker, E.mtm, E.btm, workers.last_name, workers.first_name, workers.middle_name, workers.table_no, workers.department from ( select C.card, C.worker, C.mtm, D.btm from ( select A.card, A.worker, B.mtm from (select distinct card, worker from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + lst.Time_startW + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + lst.Time_endW + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker)A LEFT JOIN (select card, worker, min(etimeserver) as MTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + lst.Time_startW + "' and etimesirius<='" + allDates[i].ToShortDateString() + " " + lst.Time_endW + "' and direction=" + MainWindow.direction + "and worker<>0 group by card, worker )B ON A.card = B.card AND A.worker = B.worker) C LEFT JOIN (select card, worker, min(etimeserver) as BTM from events where etimesirius>='" + allDates[i].ToShortDateString() + " " + lst.Time_startW + "' and etimesirius <='" + allDates[i].ToShortDateString() + " " + lst.Time_endW + "' and direction=" + MainWindow.direction + " and worker<>0 group by card, worker )D ON C.card = D.card AND C.worker = D.worker)" + "E LEFT JOIN workers ON E.worker = workers.id where workers.table_no='" + lst.Table_Number + "') F LEFT JOIN departments ON F.department = departments.id order by btm";
                        cmd2.Connection = con;
                        NpgsqlDataReader reader;
                        reader = cmd2.ExecuteReader();
                        while (reader.Read())
                        {
                            foreach (string s in listread1[i])
                            {
                                if (s == reader[3].ToString())
                                { goto breakout3; }
                            }
                            listread.Add(new DataEmployers(reader[1].ToString(), reader[0].ToString(), reader[2].ToString(),/**/ reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                            breakout3:
                            continue;
                        }
                        cmd2.Dispose();
                        reader.Close();
                    }
                }
                DataGridWindow = new DataGridWindow(listread);
            }
        }   
/// <summary>
        /// Радіо батон для напрямку руху
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enter_Checked(object sender, RoutedEventArgs e)
        {
            if(Enter.IsChecked==true)
            {
            direction="0";
            }
            else
            if (Exit.IsChecked == true)
            {
                direction = "1";
            }
        }
/// <summary>
        /// Функція для првильного відображення апострофа для sql запиту
        /// </summary>
        /// <param name="selected_text"></param>
        /// <returns></returns>
        public string DepartamentParse(string selected_text)
        {           
            string new_text;
            string el = "'";
            bool f = selected_text.Contains(el);
            if (f==true)
            {               
               new_text = selected_text.Replace(el, "''").ToString();
               return new_text;
            }
            else return selected_text;
        }
/// <summary>
        /// Кнопка для виклику вікна EmployersDT_Set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_EmpSetTimes(object sender, RoutedEventArgs e)//Set employers inidividual times for work
        {
            Set_Emp_times = new EmployersDT_Set();
            Set_Emp_times.Show();
            Set_Emp_times.initialALL();
        }
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)//event close window
        {            
           System.Windows.Application.Current.Shutdown();         
        }
/// <summary>
/// кнопка з меню для виклику вікна допомоги
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            helpWindow = new HelpWindow();
            helpWindow.Show();
        }
/// <summary>
/// Кнопка з меню для виклику вікна
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            about = new About();
            about.Show();
        }
/// <summary>
        /// Функція для змінення тексту в IntelliSenseTextBox
        /// Використовується contains для пошуку всіх схожостей в списку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntelliSenseTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {           
            string typedString = IntelliSenseTextBox.Text;
            List<string> autolist = new List<string>();
            autolist.Clear();
            foreach (string item in newlist)//
            {
                if (item.Contains(typedString))
                { autolist.Add(item); }
            }
            if (autolist.Count > 0) { List_box_intellishell.ItemsSource = autolist; List_box_intellishell.Visibility = Visibility.Visible; }
            else if (IntelliSenseTextBox.Text.Equals(""))
            { List_box_intellishell.Visibility = Visibility.Collapsed; List_box_intellishell.ItemsSource = null; }
            else { List_box_intellishell.Visibility = Visibility.Collapsed; List_box_intellishell.ItemsSource = null; }
        }
 /// <summary>
        /// Функція для вибраного елементу з List_box_intellishell
        /// Передає вибраний тест в IntelliSenseTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_box_intellishell_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (List_box_intellishell.ItemsSource != null)
            //{
            //    List_box_intellishell.Visibility = Visibility.Collapsed;
            //    IntelliSenseTextBox.TextChanged -= new TextChangedEventHandler(IntelliSenseTextBox_TextChanged);
            //    if (List_box_intellishell.SelectedIndex != -1)
            //    {
            //        IntelliSenseTextBox.Text = List_box_intellishell.SelectedItem.ToString();
            //    }
            //    IntelliSenseTextBox.TextChanged += new TextChangedEventHandler(IntelliSenseTextBox_TextChanged);
            //}
        }
/// <summary>
/// кнопка в меню *налаштування*
/// розриває або встановлює зєднання
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void MenuItem_Click_connection_enter(object sender, RoutedEventArgs e)
        {
            if (Enter_Connection_form.Header.ToString() == "Розірвати з'єднання")
            {
                con.Close();
                Enter_Connection_form.Header = "Встановити з'єднання";
                Set_Connection_form.IsEnabled = true;
            }
            else
                       if (Enter_Connection_form.Header.ToString() == "Встановити з'єднання")
            {
                try
                {
                    con = new NpgsqlConnection("Server=" + conS.Ip_address + ";Port=" + conS.Port + ";User Id=" + conS.user + ";Password=" + conS.Password + ";Database=" + conS.Db_name + ";");
                    con.Open();
                }
                catch { Connection_Window.Show(); }
                if (con.State == System.Data.ConnectionState.Open)
                {
                    Enter_Connection_form.Header = "Розірвати з'єднання";
                 //   Set_Connection_form.IsEnabled = false;
                }
            }
        }
/// <summary>
        /// функція для налаштування зєднання
        /// тут знаходиться конекшн стрінг
        /// </summary>
        public  void SetConnection()
        {            
            ReadFromConfig();
            try
            {
                con = new NpgsqlConnection("Server=" + conS.Ip_address + ";Port=" + conS.Port + ";User Id=" + conS.user + ";Password=" + conS.Password + ";Database=" + conS.Db_name + ";");
                con.Open();                
            }
            catch { MessageBox.Show("З'єднання з базою даних не встановлено , спробуйте змінити налаштування підключення","ОК"); Connection_Window.ShowDialog(); }
            if (con.State == System.Data.ConnectionState.Open)
            {                
                Enter_Connection_form.Header = "Розірвати з'єднання";
            }            
                //Thread.Sleep(5000);
               // loading_Window.Hide();
        }           
    }
}
