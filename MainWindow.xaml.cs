using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeMergeRobot
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> files = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            this.TxtAddress.Text = dialog.SelectedPath.Trim();
            
            this.files = GetFiles(this.TxtAddress.Text);
        }

        public void WriteFile(List<string> files)
        {
            FileStream file;
            try
            {
                file = new FileStream($"file-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.txt", FileMode.OpenOrCreate);

                foreach (var item in files)
                {
                    var content = File.ReadAllText(item);
                    content = content.Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");

                    byte[] data = Encoding.Default.GetBytes(content);

                    file.Position = file.Length;
                    
                    file.Write(data, 0, data.Length);

                }

                file.Close();
                file.Dispose();
            }
            catch (IOException e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        public bool IsExcludeFolder(string path)
        {
            List<string> folders = new List<string>
            {
                "build",
                "lib",
                "bin",
                "obj",
                "FmContent",
                "Debug",
                "Release"
            };

            foreach (var item in folders)
            {
                if (path.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsExcludeFiles(string file)
        {
            List<string> files = new List<string>
            {
                "all.js",
                "all.min.js"
            };

            files.Where(f => f.Contains(file));
            foreach (var item in files)
            {
                if (file.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> GetFiles(string dirPath) //参数dirPath为指定的目录
        {
            if (dirPath.Trim().Length<1)
            {
                System.Windows.MessageBox.Show("请输入地址");
                return null;

            }

            DirectoryInfo dir = new DirectoryInfo(dirPath);
            List<string> result = new List<string>();
            try
            {

                var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories)
                       .Where(s => s.EndsWith(".cs") || s.EndsWith(".js") || s.EndsWith(".json"));

                if (files.Count() > 1)
                {
                    foreach (var f in files) //查找文件
                    {
                        if (!IsExcludeFiles(f) && !IsExcludeFolder(f))
                        {
                            string file = System.IO.Path.Combine(dir.ToString(), f);
                            ListFiles.Items.Add(file);
                            result.Add(file);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }

            return result;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count < 1 && !string.IsNullOrEmpty(TxtAddress.Text.Trim()))
            {
                files = GetFiles(TxtAddress.Text.Trim());
            }

            if (files.Count>1)
            {
                WriteFile(files);

                System.Windows.MessageBox.Show("导入成功");

            }

        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            ListFiles.Items.Clear();
            if (!string.IsNullOrEmpty(TxtAddress.Text.Trim()))
            {
                files = GetFiles(TxtAddress.Text.Trim());
            }

        }
    }
}
