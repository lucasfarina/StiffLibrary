using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace StiffLibrary
{
    public class MySqlManager
    {

        public MySqlManager(string Server ="",string Port="3306", string Database = "", string Username = "", string Password = "", bool IntegratedSecurity = false)
        {
            _server = Server;
            _port = Port;
            _database = Database;
            _username = Username;
            _password = Password;
            _integratedSecurity = IntegratedSecurity;
        }

        private MySqlConnection conn;
        private MySqlCommand cmd;

        private string _server;
        private string _port;
        private string _database;
        private string _username;
        private string _password;
        private bool _integratedSecurity;
        private bool _isConnOpen;
        private string _command = "";

        public string Server { get { return _server; } set { _server = value; } }
        public string Port { get { return _port; } set { _port = value; } }
        public string Database { get { return _database; } set { _database = value; } }
        public string Username { get { return _username; } set { _username = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public bool IntegratedSecurity { get { return _integratedSecurity; } set { _integratedSecurity = value; } }
        public bool ConnOpen { get { return _isConnOpen; } set { _isConnOpen = value; } }
        public string Command
        {
            get { return _command; }
            set
            {
                _command = value;
                if(ConnOpen == true)
                {
                    cmd = new MySqlCommand(_command, conn);
                    //cmd = conn.CreateCommand();
                    //cmd.CommandText = _command;
                }
                else
                {
                    _command = "Error! Connection is closed!";
                }
            }
        }
        public bool CmdOk
        {
            get
            {
                bool answer = false;
                if(cmd != null && Command != "Error! Connection is closed!" && Command != string.Empty) { answer = true; }
                return answer;
            }
        }

        private bool IsConnOk
        {
            get
            {
                bool answer = false;
                if(!string.IsNullOrEmpty(_server) && !string.IsNullOrEmpty(_port) && !string.IsNullOrEmpty(_database) && (_integratedSecurity == true || (_integratedSecurity == false && !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))))
                {
                    answer = true;
                }
                return answer;
            }
        }

        public int OpenConnection(string ConnServer = "",string ConnPort="3306", string ConnDatabase = "", string ConnUsername = "", string ConnPassword = "", bool? ConnIntegratedSecurity = null)
        {
            int exitCode = 1;
            if(ConnServer != string.Empty) { Server = ConnServer; }
            if (ConnPort != "3306") { Port = ConnPort; }
            if (ConnDatabase != string.Empty) { Database = ConnDatabase; }
            if (ConnUsername != string.Empty) { Username = ConnUsername; }
            if (ConnPassword != string.Empty) { Password = ConnPassword; }
            if (ConnIntegratedSecurity != null) { IntegratedSecurity = (bool)ConnIntegratedSecurity; }

            if (IsConnOk && !ConnOpen)
            {
                try
                {
                    string connString = "server=" + Server + ";port=" + Port + ";database=" + Database + ";" + (IntegratedSecurity ? "Integrated Security=true" : "uid=" + Username + ";pwd=" + Password);
                    conn = new MySqlConnection(connString);
                    conn.Open();
                    ConnOpen = true;
                    exitCode = 0;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return exitCode;
        }

        public string ExecuteNonQuery()
        {
            string error = string.Empty;
            try
            {
                if(IsConnOk && ConnOpen && CmdOk)
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch(MySqlException exception)
            {
                Console.WriteLine(exception.ToString());
                error = exception.ToString();
                //conn.Close();
                //ConnOpen = false;
            }
            finally
            {
                //conn.Close();
                //ConnOpen = false;
            }
            return error;
        }

        public void CloseConnection()
        {
            if (ConnOpen == true)
            {
                conn.Close();
                ConnOpen = false;
            }
        }

        public List<object[]> ExecuteReading()
        {
            List<object[]> rows = new List<object[]>();
            try
            {
                if (IsConnOk && ConnOpen && CmdOk)
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] row = new object[reader.FieldCount];
                            int fieldCount = reader.GetValues(row);
                            rows.Add(row);
                        }
                        reader.Close();
                    }
                }
            }
            catch (MySqlException exception)
            {
                Console.WriteLine(exception.ToString());
                //conn.Close();
                //ConnOpen = false;
            }
            finally
            {
                //conn.Close();
                //ConnOpen = false;
            }

            return rows;
        }
    }
}
