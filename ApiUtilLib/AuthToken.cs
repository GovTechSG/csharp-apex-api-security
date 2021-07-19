using System;
using System.Collections.Generic;

namespace ApexUtilLib
{
    public class AuthToken
    {
        public string _token;
        public List<string> _list;

        public AuthToken(string token, List<string> baseStringList)
        {
            _token = token;
            _list = baseStringList;
        }

        public string Token
        {
            get
            {
                return _token;
            }
        }

        public List<string> BaseStringList
        {
            get
            {
                return _list;
            }
        }

        public string BaseString
        {
            get
            {
                return String.Join(", ", _list.ToArray());
            }
        }
    }
}
