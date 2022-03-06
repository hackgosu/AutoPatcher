using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SAPatcher
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String defaultURL = "http://192.168.176.129/";
        private const String jsonFileName = "patcher.json";
        private const String defaultPATH = "stoneage";
        private String jsonResult = String.Empty;
        private int fileCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            new Thread(() =>
            {
                Console.WriteLine(String.Format("defaultURL = \"{0}{1}\"", defaultURL, jsonFileName));
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(String.Format("{0}{1}", defaultURL, jsonFileName));
                httpWebRequest.Method = "GET";
                using (HttpWebResponse webResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {

                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {

                        jsonResult = sr.ReadToEnd();
                        List<FileProperty> fileList = JsonConvert.DeserializeObject<List<FileProperty>>(jsonResult);
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            patcherProgressBar.Maximum = fileList.Count;
                        }));

                        String myPath = Directory.GetCurrentDirectory();
                        List<FileProperty> downloadFiles = new List<FileProperty>();
                        foreach (FileProperty file in fileList)
                        {
                            String checkFile = String.Format("{0}{1}", myPath, file.fileName.Replace("/", "\\"));
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                patcherProgressBar.Value = patcherProgressBar.Value + 1;
                            }));
                            if (!File.Exists(checkFile))
                            {
                                
                                downloadFiles.Add(file);
                                continue;
                            }
                            if (SecurityUtil.ComputeMD5Hash(checkFile).Equals(file.fileCheckSum))
                            {
                                continue;
                            }
                            else
                            {
                                downloadFiles.Add(file);
                            }
                            
                        }
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            patcherProgressBar.Maximum = downloadFiles.Count;
                            patcherProgressBar.Value = 0;
                        }));
                        foreach(FileProperty fileProperty in downloadFiles)
                        {
                            using (var client = new WebClient())
                            {
                                Console.WriteLine(String.Format("{0}{1}", defaultURL, fileProperty.fileName));
                                client.DownloadFile(String.Format("{0}{1}{2}",defaultURL, defaultPATH,fileProperty.fileName),String.Format("{0}{1}",myPath,fileProperty.fileName.Replace("/","\\")));
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    patcherProgressBar.Value = patcherProgressBar.Value + 1;
                                }));
                            }
                        }

                    }
                }


            }).Start();

        }
    }
}
