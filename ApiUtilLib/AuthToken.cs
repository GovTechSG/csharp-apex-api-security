using System;
using System.Collections.Generic;

namespace ApiUtilLib
{
    public class AuthToken
    {
        private string _token;
        private List<string> _list;

        internal AuthToken(string token, List<string> baseStringList)
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
