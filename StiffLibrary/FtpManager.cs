using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StiffLibrary
{
    public class FtpManager
    {
        private NetworkCredential _credentials;
        private string _ftpRoot;
        private string _port;
        private bool _keepAlive;
        private bool _usePassive;
        private bool _useBinary;
        private bool _enableSsl;
        public FtpManager(string ftpRoot, string port, string userName, string password, bool keepAlive = false, bool usePassive = true, bool useBinary = false, bool enableSSL = false)
        {
            _ftpRoot = ftpRoot;
            _keepAlive = keepAlive;
            _port = port;
            _credentials = new NetworkCredential(userName, password);
            _usePassive = usePassive;
            _useBinary = useBinary;
            _enableSsl = enableSSL;
        }
        public string ftpRoot { get => _ftpRoot; }
        public string port { get => _port; }
        public NetworkCredential Credentials { get => _credentials; }
        public bool KeepAlive { get => _keepAlive; }
        public bool UsePassive { get => _usePassive; }
        public bool UseBinary { get => _useBinary; }
        public bool EnableSSl { get => _enableSsl; }

        public FtpWebResponse UploadFile(string PathFromRoot, byte[] fileContents)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpRoot + ":" + port + "/" + PathFromRoot);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.KeepAlive = KeepAlive;

            request.Credentials = Credentials;
            request.ContentLength = fileContents.Length;
            request.UsePassive = UsePassive;
            request.UseBinary = UseBinary;
            request.EnableSsl = EnableSSl;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            return (FtpWebResponse)request.GetResponse();
        }

        public FtpWebResponse DownloadFile(string PathFromRoot, out Stream responseStream)
        {
            responseStream = null;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpRoot + ":" + port + "/" + PathFromRoot);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.KeepAlive = KeepAlive;

            request.Credentials = Credentials;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            if (response.StatusCode == FtpStatusCode.OpeningData)
            {
                responseStream = response.GetResponseStream();
            }

            return response;
        }

        public FtpWebResponse DeleteFile(string PathFromRoot)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpRoot + ":" + port + "/" + PathFromRoot);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.KeepAlive = KeepAlive;

            request.Credentials = Credentials;

            return (FtpWebResponse)request.GetResponse();
        }

        public FtpWebResponse GetFiles(string PathFromRoot, out string[] collection)
        {
            collection = null;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpRoot + ":" + port + "/" + PathFromRoot);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.KeepAlive = KeepAlive;

            request.Credentials = Credentials;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            if (response.StatusCode == FtpStatusCode.OpeningData)
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                List<string> Linhas = new List<string>();
                while (!reader.EndOfStream)
                {
                    string Linha = reader.ReadLine();
                    if (Linha.Replace(".", "").Length != 0)
                    {
                        Linhas.Add(Linha);
                    }
                }
                collection = Linhas.ToArray();
            }
            return response;
        }
    }

}
