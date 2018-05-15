using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TestPostgreInWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int min_spl_time = 4000;
        protected  override void OnStartup(StartupEventArgs e)
        {
            Loading_window splash = new Loading_window();
            splash.Show();
            Stopwatch timer = new Stopwatch();
            timer.Start();          
            base.OnStartup(e);
            MainWindow main = new MainWindow();
            timer.Stop();
            splash.Close();
            main.Activate();
        }
        public static Task Delay(double milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) =>
            {
                tcs.TrySetResult(true);
            };
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();
            return tcs.Task;
        }
    }
}
