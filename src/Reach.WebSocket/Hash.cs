using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Reach.WebSocket
{
    internal static class Hash
    {
        private static string Crypt(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string GetHashId(string input)
        {
            MD5 md5Hash = MD5.Create();
            return Crypt(md5Hash, input);
        }
    }
}
