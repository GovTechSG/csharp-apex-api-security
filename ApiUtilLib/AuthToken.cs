using System.Collections.Generic;

namespace ApiUtilLib
{
    public class AuthToken
    {
        internal AuthToken(string token, List<string> baseStringList)
        {
            Token = token;
            BaseStringList = baseStringList;
        }

        public string Token { get; }

        public List<string> BaseStringList { get; }

        public string BaseString => string.Join(", ", BaseStringList.ToArray());
    }
}
