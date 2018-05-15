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
using NpgsqlTypes;
using System.Data.Sql;
using TestPostgreInWpf.DataC;

namespace TestPostgreInWpf
{
   
/// <summary>
    /// Interaction logic for Connection.xaml
    /// Вікно для налаштування конешн
    /// </summary>
    public partial class Connection : Window
    {
        public Connection()
        {
            InitializeComponent();           
        }
/// <summary>
/// Кнопка для збереження конфігурацій
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConnectionSettings con_data = new ConnectionSettings();
            MainWindow.conS.Ip_address = Ip_address.Text;
            MainWindow.conS.Db_name = Db_name.Text;
            MainWindow.conS.Port = Port.Text;
            MainWindow.conS.user = User.Text;
            MainWindow.conS.Password = Password.Text;
            ConnectionSettings new_connection = new ConnectionSettings();
            new_connection.PrintInFile(Ip_address.Text, Port.Text,Db_name.Text, User.Text,Password.Text);
            MessageBox.Show("Settings have saved","Ok");
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();//перезапуск програми
        }
/// <summary>
/// Кнопка для закриття програми
/// перезагрузка
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {            
            this.Visibility = Visibility.Hidden;
            Application.Current.Shutdown();            
        }
/// <summary>
/// перегрузка методу на закривання вікна активного даного!
/// </summary>
/// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)//event close window
        {
            Application.Current.Shutdown();
        }
    }
}
