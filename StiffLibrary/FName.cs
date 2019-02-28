using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StiffLibrary
{
    public struct FName
    {
        private const int _initialMapChunk = 50;
        private static int _mapChunk = _initialMapChunk;
        private static string[] _nameMap = new string[_mapChunk];
        private static int _mapNextId = 1;
        public static readonly FName None = new FName();

        private int _id;
        private string _name;

        public FName(string name = null)
        {
            string safeName = name.ToUpper();
            if(safeName != null || safeName == "None")
            {
                _name = safeName;
                int myId = Array.FindIndex<string>(_nameMap, x => x == safeName);
                if (myId >= 0)
                {
                    _id = myId;
                }
                else
                {
                    if ((_mapNextId) >= _nameMap.Length)
                    {
                        _mapChunk *= 2;
                        int newSize = _nameMap.Length + _mapChunk;
                        Array.Resize<string>(ref _nameMap, newSize);
                    }
                    _nameMap[_mapNextId] = safeName;
                    _id = _mapNextId;
                    _mapNextId++;
                }
            }
            else
            {
                _name = null;
                _id = 0;
            }
        }

        public override string ToString()
        {
            if(_id == 0)
            {
                return "NONE";
            }
            return _nameMap[_id];
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == this.GetType())
            {
                FName other = (FName)obj;
                return other._id == _id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static implicit operator FName (string s)
        {
            return new FName(s);
        }
    }
}
