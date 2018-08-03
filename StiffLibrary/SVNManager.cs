using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StiffLibrary
{
    public static class SVNManager
    {
        //FUNFA
        public static void UpdatePath(string url, string path)
        {
            Process.Start("svn.exe", @"info " + url);
            Process.Start("svn.exe", @"update [-r rev] " + path);
        }
        public static void OpenSVN()
        {
            Process.Start("cmd.exe", @"cls");
        }

        public static bool Add(string path)
        {
            bool ended = false;
            Process proc = SVNQuietCommand("add " + path);
            if (proc.Start())
            {
                proc.WaitForExit();
                ended = true;
            }
            return ended;
        }

        public static bool Commit(string path, string message)
        {
            bool ended = false;

            return ended;
        }

        public static bool Checkout(string url, string path)
        {
            bool ended = false;
            Process proc = SVNQuietCommand("co " + url + " " + path);
            if (proc.Start())
            {
                proc.WaitForExit();
                ended = true;
            }
            return ended;
        }

        public static Process SVNQuietCommand(string command)
        {
            Process proc = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "svn.exe";
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = command;
            proc.StartInfo = startInfo;

            return proc;
        }
    }
}
