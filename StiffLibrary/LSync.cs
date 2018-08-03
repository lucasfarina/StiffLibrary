//INCOMPLETE!
//This was an Idea to a class that would sync two folders in both ways, kind of my own DSyncronizer, but with delete options in a dual-way-sync

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StiffLibrary
{
    public class LSync
    {
        //==================================================================================VARIABLES
        //private
        private string path1, path2;
        private FileSystemWatcher fwSource, fwDest;

        //Properties
        public string Source
        {
            get { return path1; }
            set { path1 = value; }
        }

        public string Destination
        {
            get { return path2; }
            set { path2 = value; }
        }

        //Constructor
        public LSync(string source, string destination)
        {
            Source = source;
            Destination = destination;

            UpdateAllFolderBiDirectional();

            //-------------Initializing Source File Watcher
            fwSource = new FileSystemWatcher(Source);

            fwSource.IncludeSubdirectories = true;
            fwSource.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;

            fwSource.Changed += new FileSystemEventHandler(fwSource_Changed);
            fwSource.Renamed += new RenamedEventHandler(fwSource_Renamed);
            fwSource.Deleted += new FileSystemEventHandler(fwSource_Deleted);
            fwSource.Created += new FileSystemEventHandler(fwSource_Created);

            fwSource.EnableRaisingEvents = true;

            //-------------Initializing Destination File Watcher
            fwDest = new FileSystemWatcher(Destination);

            fwDest.IncludeSubdirectories = true;
            fwDest.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;

            fwDest.Changed += new FileSystemEventHandler(fwDest_Changed);
            fwDest.Renamed += new RenamedEventHandler(fwDest_Renamed);
            fwDest.Deleted += new FileSystemEventHandler(fwDest_Deleted);
            fwDest.Created += new FileSystemEventHandler(fwDest_Created);

            fwDest.EnableRaisingEvents = true;
        }

        //==================================================================================EVENTS

        //-----------------------------SOURCE WATCHER EVENTS
        private void fwSource_Created(object sender, FileSystemEventArgs e)
        {

        }

        private void fwSource_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void fwSource_Deleted(object sender, FileSystemEventArgs e)
        {

        }

        private void fwSource_Renamed(object sender, RenamedEventArgs e)
        {

        }

        //-----------------------------DESTINATION WATCHER EVENTS
        private void fwDest_Created(object sender, FileSystemEventArgs e)
        {

        }

        private void fwDest_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void fwDest_Deleted(object sender, FileSystemEventArgs e)
        {

        }

        private void fwDest_Renamed(object sender, RenamedEventArgs e)
        {

        }

        //==================================================================================FUNCTIONS

        private void UpdateAllFolderBiDirectional()
        {
            DirectoryInfo mySource = new DirectoryInfo(path1);
            DirectoryInfo myDest = new DirectoryInfo(path2);

            DirectoryInfo[] SourceDirectiories = GetDirectiories(mySource).ToArray();
            DirectoryInfo[] DestinationDirectiories = GetDirectiories(myDest).ToArray();

            UpdateFilesBiDirectional(mySource, myDest);
            UpdateFilesBiDirectional(myDest, mySource);

            foreach (DirectoryInfo dir in SourceDirectiories)//Checar Pastas
            {
                bool folderJaExiste = false;
                string dirRootName = dir.FullName.Replace(mySource.FullName, "");
                foreach (DirectoryInfo dir2 in DestinationDirectiories)
                {
                    if (dir.Name == dir2.Name)//Se Folder Ja Existe
                    {
                        folderJaExiste = true;
                        UpdateFilesBiDirectional(dir, dir2);
                    }
                }

                if (folderJaExiste == false)
                {
                    //Create Folder
                    Directory.CreateDirectory(Path.Combine(myDest.FullName, dir.Name));
                }
            }

            foreach (DirectoryInfo dir in DestinationDirectiories)//Checar Pastas
            {
                bool folderJaExiste = false;
                string dirRootName = dir.FullName.Replace(mySource.FullName, "");
                foreach (DirectoryInfo dir2 in SourceDirectiories)
                {
                    if (dir.Name == dir2.Name)//Se Folder Ja Existe
                    {
                        folderJaExiste = true;
                        UpdateFilesBiDirectional(dir, dir2);
                    }
                }

                if (folderJaExiste == false)
                {
                    //Create Folder
                    Directory.CreateDirectory(Path.Combine(myDest.FullName, dir.Name));
                }
            }
        }

        private void UpdateFilesBiDirectional(DirectoryInfo dir, DirectoryInfo dir2)
        {
            foreach (FileInfo file in dir.GetFiles())//Checar Files dentro
            {
                bool fileJaExiste = false;
                DateTime fileWriteTime = File.GetLastWriteTime(file.FullName);
                DateTime fileCreateTime = File.GetCreationTime(file.FullName);

                foreach (FileInfo file2 in dir2.GetFiles())
                {
                    if (file.Name == file2.Name)//Se File Ja Existe
                    {
                        fileJaExiste = true;
                        DateTime file2WriteTime = File.GetLastWriteTime(file2.FullName);
                        //DateTime.Compare(time1, time2) Qual foi antes? -1=time1, 0=igual, 1=time2
                        //ChecarQualMaisNovo, se igual, qual maior.
                        switch (DateTime.Compare(fileWriteTime, file2WriteTime))
                        {
                            case -1:
                                //InverseOverwriteFile
                                File.Copy(file2.FullName, file.FullName, true);
                                break;
                            case 0:
                                //Igual
                                if (file.Length > file2.Length)
                                    File.Copy(file.FullName, file2.FullName, true);
                                else if (file.Length < file2.Length)
                                    File.Copy(file2.FullName, file.FullName, true);
                                break;
                            case 1:
                                //OverwriteFile
                                File.Copy(file.FullName, file2.FullName, true);
                                break;
                        }
                    }
                }

                if (fileJaExiste == false)
                {
                    File.Copy(file.FullName, dir2.FullName + "/" + file.Name, true);
                }
            }
        }

        private List<DirectoryInfo> GetDirectiories(DirectoryInfo myDir)
        {
            List<DirectoryInfo> listToPopulate = new List<DirectoryInfo>();
            foreach (DirectoryInfo dir in myDir.GetDirectories())
            {
                listToPopulate.Add(dir);
                listToPopulate.AddRange(GetDirectiories(dir));
            }

            return listToPopulate;
        }
    }
}
