using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SAPatcher
{
    public class SecurityUtil
    {
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




    }
}
