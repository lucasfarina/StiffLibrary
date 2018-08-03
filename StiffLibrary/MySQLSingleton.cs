using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StiffLibrary
{
    public class MySQLWrapper
    {
        private string _server, _port, _database, _username, _password;
        bool _integratedSecurity;
        private MySqlManager _mySqlManager;

        public MySQLWrapper(string server, string port, string database, string username, string password, bool integratedSecurity)
        {
            _server = server;
            _port = port;
            _database = database;
            _username = username;
            _password = password;
            _integratedSecurity = integratedSecurity;
        }

        public MySqlManager GetManager
        {
            get {
                if (_mySqlManager == null)
                {
                    _mySqlManager = new MySqlManager(_server, _port, _database, _username, _password, _integratedSecurity);
                    _mySqlManager.OpenConnection();
                }
                else if (!_mySqlManager.ConnOpen)
                {
                    _mySqlManager = new MySqlManager(_server, _port, _database, _username, _password, _integratedSecurity);
                    _mySqlManager.OpenConnection();
                }
                return _mySqlManager;
            }
        }
    }
}
