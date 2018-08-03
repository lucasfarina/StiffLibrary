using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StiffLibrary
{
    public static class IOManager
    {
        public static String[] GetFile(string path)
        {
            List<String> lines = new List<string>();
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                while (sr.Peek() > -1)
                {
                    lines.Add(sr.ReadLine());
                }
                sr.Close();
                sr.Dispose();
            }
            return lines.ToArray();
        }

        public static bool WriteFile(string path, String[] lines, bool append = false)
        {
            bool success = false;
            StreamWriter sw = new StreamWriter(path, append);
            foreach(String line in lines)
            {
                sw.WriteLine(line);
            }
            sw.Close();
            sw.Dispose();
            success = true;
            return success;
        }

        public static void EncryptFile(string path)
        {
            File.Encrypt(path);
        }

        public static void DecryptFile(string path)
        {
            File.Decrypt(path);
        }
    }
}
