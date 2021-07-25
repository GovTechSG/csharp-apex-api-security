//using System;
using System.Collections.Generic;

namespace ApiUtilLib
{
    public class AuthToken
    {
        private readonly string _token;

        internal AuthToken(string token, List<string> baseStringList)
        {
            _token = token;
            BaseStringList = baseStringList;
        }

        public string Token => _token;

        public List<string> BaseStringList { get; }

        public string BaseString => string.Join(", ", BaseStringList.ToArray());
    }
}
