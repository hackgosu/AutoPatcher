using Force.Crc32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace PatcherEditor
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<FileProperty> fileList = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fileList = new List<FileProperty>();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fileList.Clear();
            myListView.Items.Clear();
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                Thread fileProcessingThread = new Thread(() =>
                {
                    int i = 0;

                    String[] files = GetSearchFile(fbd.SelectedPath);
                    fbd.Dispose();
                    foreach (String file in files)
                    {
                        i++;
                        FileProperty property = new FileProperty();
                        property.fileName = file.Replace(fbd.SelectedPath,"").Replace("\\","/"); // 폴더 명으로 변경 할 것
                        FileInfo info = new FileInfo(file);

                        property.fileCheckSum = ComputeMD5Hash(file);
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            this.Title = String.Format("Patcher Editor : 파일 갯수 {0}개",i);
                            myListView.Items.Add(property);
                        }));

                        fileList.Add(property);
                        //Console.WriteLine("{0},{1},{2}",property.fileName,property.fileSize,property.fileCheckSum);
                        //GC.Collect();
                    }
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.Title = String.Format("{0} : 최종완료", this.Title);
                    }));
                });
                fileProcessingThread.Start();
            }


        }
        public static string ComputeMD5Hash(string FilePath)
        {
            return ComputeHash(FilePath, new MD5CryptoServiceProvider());
        }
        public static string ComputeHash(string FilePath, HashAlgorithm Algorithm)
        {
            FileStream FileStream = File.OpenRead(FilePath);
            try
            {
                byte[] HashResult = Algorithm.ComputeHash(FileStream);
                string ResultString = BitConverter.ToString(HashResult).Replace("-", "");
                return ResultString;
            }
            finally
            {
                FileStream.Close();
            }
        }


        

        public string[] GetSearchFile(String _strPath)
        {
            string[] files = { "", };
            try
            {
                files = Directory.GetFiles(_strPath, "*.*", SearchOption.AllDirectories);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return files;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var json = JsonConvert.SerializeObject(fileList);
            File.WriteAllText("./patcher.json",json);

        }
    }
}
