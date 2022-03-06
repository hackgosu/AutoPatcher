using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPatcher
{
    public class FileUtil
    {

        public static string[] GetSearchFile(String _strPath)
        {
            string[] files = { "", };
            try
            {
                files = Directory.GetFiles(_strPath, "*.*", SearchOption.AllDirectories);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return files;
        }
    }
}
