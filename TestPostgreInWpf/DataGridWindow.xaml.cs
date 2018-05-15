using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using TestPostgreInWpf.DataC;
using TestPostgreInWpf;
using System.ComponentModel;

namespace TestPostgreInWpf
{
    /// <summary>
    /// Interaction logic for DataGridWindow.xaml
    /// Вікно для відображення інформації з запиту
    /// </summary>
    public partial class DataGridWindow : System.Windows.Window
    {
        public DataGridWindow()
        {
            InitializeComponent();           
        }
/// <summary>
/// переозначення конструктора для ініціалізації дата грід 
/// отримуємо listread з головного вікна
/// </summary>
/// <param name="dataEmployers"></param>
        public DataGridWindow(List<DataEmployers> dataEmployers)
        {
            InitializeComponent();
            DataGrid1.ItemsSource = dataEmployers;
            this.Show();
        }
        public DataGridWindow(List<WorkerIndivid> workerIndivids)
        {
            InitializeComponent();
            DataGrid1.ItemsSource = workerIndivids;
            this.Show();
        }
/// <summary>
/// Кнопка для збереження даних в xls файл
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<DataEmployers>));
            if (File.Exists("graftmp.xml"))
            {
                File.Delete("graftmp.xml");
            }
            using (FileStream fs = new FileStream("graftmp.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs,TestPostgreInWpf.MainWindow.listread);             
            }
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Zvit"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Text documents (.xlsx)|*.xlsx"; // Filter files by extension
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                if (File.Exists("graftmp.xml")) // Checking XMl File is Exist or Not
                {
                    FileInfo fi = new FileInfo("graftmp.xml");
                   // string XlFile = fi.DirectoryName + "\\" + fi.Name.Replace(fi.Extension, ".xls");
                    System.Data.DataTable dt = CreateDataTableFromXml("graftmp.xml");
                    ExportDataTableToExcel(dt, filename);
                    System.Windows.MessageBox.Show("Виконано");
                }
            }
            
        }
/// <summary>
        /// Генерує таблицю з xml файлу , яку ми використовуємо в створені xls документу
        /// </summary>
        /// <param name="XmlFile"></param>
        /// <returns></returns>
        private System.Data.DataTable CreateDataTableFromXml(string XmlFile)
        {
            System.Data.DataTable Dt = new System.Data.DataTable();
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(XmlFile);
                Dt.Load(ds.CreateDataReader());
            }
            catch
            {

            }
            return Dt;
        }
/// <summary>
        /// Експорт дата тейблу в xls
        /// </summary>
        /// <param name="table"></param>
        /// <param name="Xlfile"></param>
        private void ExportDataTableToExcel(System.Data.DataTable table, string Xlfile)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook book = excel.Application.Workbooks.Add(Type.Missing);
            excel.Visible = false;
            excel.DisplayAlerts = false;
            Worksheet excelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)book.ActiveSheet;
            excelWorkSheet.Name = "LOE";
            for (int i = 1; i < table.Columns.Count + 1; i++) // Creating Header Column In Excel
            {
                excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
            }
            for (int j = 0; j < table.Rows.Count; j++) // Exporting Rows in Excel
            {
                for (int k = 0; k < table.Columns.Count; k++)
                {
                    excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                }
            }
            book.SaveAs(Xlfile);
            book.Close(true);
            excel.Quit();
            Marshal.ReleaseComObject(book);
            Marshal.ReleaseComObject(book);
            Marshal.ReleaseComObject(excel);
        }
/// <summary>
        /// Перегрузка на закриття вікна
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            DataGrid1.ItemsSource = null;
            this.Hide();
        }
    }
}
